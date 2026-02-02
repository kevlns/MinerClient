using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Miner.Config.Editor
{
    /// <summary>
    /// Addressables 配置工具设置
    /// </summary>
    [System.Serializable]
    public class AddressablesConfigSettings
    {
        [Tooltip("配置文件所在的目录路径")]
        public string configBytesPath = "Assets/LubanConfig/Bytes";

        [Tooltip("Addressables 分组名称")]
        public string groupName = "Config-HF";

        [Tooltip("Addressables 地址前缀（用于拼接完整地址）")]
        public string addressPrefix = "Assets/LubanConfig/Bytes/";
    }

    /// <summary>
    /// Luban 配置文件 Addressables 自动配置工具
    /// </summary>
    public static class AddressablesConfigTool
    {
        private const string SETTINGS_KEY = "AddressablesConfigTool_Settings";
        private static AddressablesConfigSettings _settings;

        internal static AddressablesConfigSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    LoadSettings();
                }
                return _settings;
            }
        }

        private static void LoadSettings()
        {
            var json = EditorPrefs.GetString(SETTINGS_KEY, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                _settings = new AddressablesConfigSettings();
            }
            else
            {
                _settings = JsonUtility.FromJson<AddressablesConfigSettings>(json);
            }
        }

        private static void SaveSettings()
        {
            var json = JsonUtility.ToJson(_settings, true);
            EditorPrefs.SetString(SETTINGS_KEY, json);
        }

        /// <summary>
        /// 重新加载设置（用于设置窗口修改后刷新）
        /// </summary>
        public static void ReloadSettings()
        {
            _settings = null;
        }

        [MenuItem("Tools/Luban/设置")]
        public static void OpenSettings()
        {
            AddressablesConfigSettingsWindow.ShowWindow();
        }

        [MenuItem("Tools/Luban/配置 Addressables")]
        public static void SetupAddressablesForConfigFiles()
        {
            // 1. 获取或创建 Addressables Settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[配置工具] Addressables 设置未找到，请先初始化 Addressables 系统！");
                return;
            }

            // 2. 查找或创建目标分组
            var group = FindOrCreateGroup(settings, Settings.groupName);
            if (group == null)
            {
                Debug.LogError($"[配置工具] 无法创建或找到分组：{Settings.groupName}");
                return;
            }

            // 3. 查找所有 .bytes 文件
            if (!Directory.Exists(Settings.configBytesPath))
            {
                Debug.LogError($"[配置工具] 配置目录不存在：{Settings.configBytesPath}");
                return;
            }

            var bytesFiles = Directory.GetFiles(Settings.configBytesPath, "*.bytes", SearchOption.AllDirectories);
            if (bytesFiles.Length == 0)
            {
                Debug.LogWarning($"[配置工具] 在 {Settings.configBytesPath} 中未找到任何 .bytes 文件");
                return;
            }

            // 4. 处理每个文件
            int successCount = 0;
            int skipCount = 0;

            foreach (var filePath in bytesFiles)
            {
                var assetPath = filePath.Replace("\\", "/");
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning($"[配置工具] 无法获取资源 GUID：{assetPath}");
                    continue;
                }

                // 检查是否已经在 Addressables 中
                var entry = settings.FindAssetEntry(guid);
                if (entry != null)
                {
                    // 如果已存在，检查是否在正确的组中
                    if (entry.parentGroup.Name != Settings.groupName)
                    {
                        // 移动到目标组
                        settings.MoveEntry(entry, group);
                        Debug.Log($"[配置工具] 移动资源到 {Settings.groupName}：{entry.address}");
                    }
                    else
                    {
                        skipCount++;
                        continue;
                    }
                }
                else
                {
                    // 创建新条目
                    entry = settings.CreateOrMoveEntry(guid, group, false, false);
                }

                // 设置 Address（使用文件名，不含扩展名）
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var address = $"{Settings.addressPrefix}{fileName}.bytes";
                entry.SetAddress(address, false);

                successCount++;
                Debug.Log($"[配置工具] 已配置：{assetPath} -> Address：{address}");
            }

            // 5. 保存设置
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"[配置工具] 完成！成功配置 {successCount} 个文件，跳过 {skipCount} 个文件");
            EditorUtility.DisplayDialog("配置完成", 
                $"成功配置 {successCount} 个配置文件到 Addressables 分组 [{Settings.groupName}]\n跳过已存在的 {skipCount} 个文件", 
                "确定");
        }

        /// <summary>
        /// 查找或创建指定名称的分组
        /// </summary>
        private static AddressableAssetGroup FindOrCreateGroup(AddressableAssetSettings settings, string groupName)
        {
            // 尝试查找现有分组
            var group = settings.groups.FirstOrDefault(g => g.Name == groupName);
            if (group != null)
            {
                Debug.Log($"[配置工具] 找到现有分组：{groupName}");
                return group;
            }

            // 创建新分组
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            
            // 配置分组设置
            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema != null)
            {
                // 使用打包模式：Pack Together（所有资源打包到一起）
                schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
                
                // 启用压缩：LZ4 提供快速解压速度，适合运行时频繁加载的配置
                schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                
                // 包含到构建中
                schema.IncludeInBuild = true;
                
                // 尝试设置 AssetBundle Provider 为 TextDataProvider（如果属性存在）
                try
                {
                    var providerType = typeof(UnityEngine.ResourceManagement.ResourceProviders.TextDataProvider);
                    var bundleProviderField = schema.GetType().GetProperty("BundleAssetProvider");
                    if (bundleProviderField != null)
                    {
                        bundleProviderField.SetValue(schema, providerType);
                        Debug.Log($"[配置工具] 已设置 TextDataProvider");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[配置工具] 无法设置 TextDataProvider（可能不支持）: {ex.Message}");
                }
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, group, true);
            Debug.Log($"[配置工具] 创建新分组：{groupName}");
            
            return group;
        }

        [MenuItem("Tools/Luban/清理配置")]
        public static void ClearConfigAddressablesEntries()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[配置工具] Addressables 设置未找到！");
                return;
            }

            var group = settings.groups.FirstOrDefault(g => g.Name == Settings.groupName);
            if (group == null)
            {
                Debug.LogWarning($"[配置工具] 未找到分组：{Settings.groupName}");
                return;
            }

            if (!EditorUtility.DisplayDialog("确认清理", 
                $"确定要清空分组 [{Settings.groupName}] 中的所有配置条目吗？", 
                "确定", "取消"))
            {
                return;
            }

            // 获取所有条目的副本（避免在遍历时修改集合）
            var entries = group.entries.ToList();
            int removeCount = 0;

            foreach (var entry in entries)
            {
                settings.RemoveAssetEntry(entry.guid);
                removeCount++;
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"[配置工具] 已清除 {removeCount} 个配置条目");
            EditorUtility.DisplayDialog("清理完成", $"已清除 {removeCount} 个配置条目", "确定");
        }

        [MenuItem("Tools/Luban/优化分组设置")]
        public static void OptimizeGroupSettings()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[配置工具] Addressables 设置未找到！");
                return;
            }

            var group = settings.groups.FirstOrDefault(g => g.Name == Settings.groupName);
            if (group == null)
            {
                Debug.LogWarning($"[配置工具] 未找到分组：{Settings.groupName}");
                return;
            }

            var schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null)
            {
                Debug.LogError($"[配置工具] 分组 [{Settings.groupName}] 缺少 BundledAssetGroupSchema");
                return;
            }

            // 应用最优配置
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
            schema.IncludeInBuild = true;
            
            // 尝试设置 AssetBundle Provider 为 TextDataProvider
            try
            {
                var providerType = typeof(UnityEngine.ResourceManagement.ResourceProviders.TextDataProvider);
                var bundleProviderField = schema.GetType().GetProperty("BundleAssetProvider");
                if (bundleProviderField != null)
                {
                    bundleProviderField.SetValue(schema, providerType);
                    Debug.Log($"[配置工具] 已设置 TextDataProvider");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[配置工具] 无法设置 TextDataProvider: {ex.Message}");
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, group, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"[配置工具] 已优化分组设置：{Settings.groupName}");
            EditorUtility.DisplayDialog("优化完成", 
                $"已为分组 [{Settings.groupName}] 应用最优配置：\n\n" +
                $"• LZ4 压缩（快速解压）\n" +
                $"• Pack Together 打包模式\n" +
                $"• 包含在构建中\n" +
                $"• TextDataProvider（如果支持）", 
                "确定");
        }
    }

    internal class AddressablesConfigAutoPostprocessor : AssetPostprocessor
    {
        private static bool _queued;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (_queued)
            {
                return;
            }

            if (importedAssets == null || importedAssets.Length == 0)
            {
                return;
            }

            var configPath = AddressablesConfigTool.Settings.configBytesPath.Replace("\\", "/");
            var hasBytes = importedAssets.Any(path =>
            {
                var p = path.Replace("\\", "/");
                return p.StartsWith(configPath, System.StringComparison.OrdinalIgnoreCase)
                       && p.EndsWith(".bytes", System.StringComparison.OrdinalIgnoreCase);
            });

            if (!hasBytes)
            {
                return;
            }

            _queued = true;
            EditorApplication.delayCall += () =>
            {
                _queued = false;
                AddressablesConfigTool.SetupAddressablesForConfigFiles();

                // 自动构建 Addressables（确保热更配置可用）
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings != null)
                {
                    AddressableAssetSettings.BuildPlayerContent();
                    Debug.Log("[配置工具] Addressables 构建完成");
                }
                else
                {
                    Debug.LogWarning("[配置工具] Addressables 设置未找到，跳过自动构建");
                }
            };
        }
    }
}

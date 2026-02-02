using UnityEditor;
using UnityEngine;

namespace Miner.Config.Editor
{
    /// <summary>
    /// Addressables 配置工具设置窗口
    /// </summary>
    public class AddressablesConfigSettingsWindow : EditorWindow
    {
        private const string SETTINGS_KEY = "AddressablesConfigTool_Settings";
        private AddressablesConfigSettings _settings;
        private Vector2 _scrollPosition;

        public static void ShowWindow()
        {
            var window = GetWindow<AddressablesConfigSettingsWindow>("Luban Config Settings");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
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

        private void SaveSettings()
        {
            var json = JsonUtility.ToJson(_settings, true);
            EditorPrefs.SetString(SETTINGS_KEY, json);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Luban 配置工具设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "这些设置用于配置 Addressables 自动化工具的行为。\n" +
                "修改后点击「保存设置」按钮生效。",
                MessageType.Info);

            EditorGUILayout.Space(10);

            // 配置文件路径
            EditorGUILayout.LabelField("配置文件路径", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("配置 .bytes 文件所在的目录路径（相对于 Assets）", EditorStyles.wordWrappedMiniLabel);
            _settings.configBytesPath = EditorGUILayout.TextField("Config Bytes Path", _settings.configBytesPath);
            
            // 浏览按钮
            if (GUILayout.Button("浏览...", GUILayout.Width(80)))
            {
                var path = EditorUtility.OpenFolderPanel("选择配置文件目录", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    // 转换为相对路径
                    var projectPath = Application.dataPath.Replace("/Assets", "");
                    if (path.StartsWith(projectPath))
                    {
                        _settings.configBytesPath = path.Substring(projectPath.Length + 1).Replace("\\", "/");
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Addressables 分组名称
            EditorGUILayout.LabelField("Addressables 分组", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("配置文件将被添加到的 Addressables 分组名称", EditorStyles.wordWrappedMiniLabel);
            _settings.groupName = EditorGUILayout.TextField("Group Name", _settings.groupName);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Address 前缀
            EditorGUILayout.LabelField("Address 前缀", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Addressables 资源地址的前缀，用于运行时加载", EditorStyles.wordWrappedMiniLabel);
            _settings.addressPrefix = EditorGUILayout.TextField("Address Prefix", _settings.addressPrefix);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                $"示例：配置文件 tbsampledata.bytes\n" +
                $"完整 Address：{_settings.addressPrefix}tbsampledata.bytes",
                MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            // 按钮区域
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("重置为默认值", GUILayout.Width(120), GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("确认重置", "确定要重置所有设置为默认值吗？", "确定", "取消"))
                {
                    _settings = new AddressablesConfigSettings();
                    SaveSettings();
                    ShowNotification(new GUIContent("已重置为默认值"));
                }
            }

            if (GUILayout.Button("保存设置", GUILayout.Width(120), GUILayout.Height(25)))
            {
                SaveSettings();
                AddressablesConfigTool.ReloadSettings();
                Close();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }
    }
}

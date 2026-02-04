using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Vant.UI.UIComponents;
using Van.System.Guide;
using Vant.Core;

namespace Miner.UI
{
    public class TestViewSkin
    {
        public GameObject gameObject;
        public UnityEngine.UI.Text title;
        public UnityEngine.UI.Button button;

        public TestViewSkin(GameObject gameObject)
        {
            Replace(gameObject);
        }
        public void Replace(GameObject gameObject)
        {
            this.gameObject = gameObject;
            var generator = gameObject.GetComponent<ReferenceContainerGenerator>();
            title = generator.Get<UnityEngine.UI.Text>("title");
            button = generator.Get<UnityEngine.UI.Button>("button");
        }

        public void Dispose()
        {
            ClickableTargetManager.UnregisterTarget("TestViewSkin/button");
            gameObject = null;
            title = null;
            button = null;
        }
    }
}

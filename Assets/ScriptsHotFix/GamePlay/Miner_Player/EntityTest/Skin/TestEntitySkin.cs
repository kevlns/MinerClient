using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Vant.UI.UIComponents;
using Van.System.Guide;

namespace Miner.GamePlay
{
    public class TestEntitySkin
    {
        public GameObject gameObject;
        public UnityEngine.Transform trans;

        public TestEntitySkin(GameObject gameObject)
        {
            Replace(gameObject);
        }
        public void Replace(GameObject gameObject)
        {
            this.gameObject = gameObject;
            var generator = gameObject.GetComponent<ReferenceContainerGenerator>();
            trans = generator.Get<UnityEngine.Transform>("trans");
        }
        public void Dispose()
        {
            gameObject = null;
            trans = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DOTSInMars.UI
{
    [Serializable]
    internal class BuildingButton : MonoBehaviour
    {
        public Button Button;
        public GameObject HighlightObject;

        internal void Dehighlight()
        {
            HighlightObject.SetActive(false);
        }

        internal void Highlight()
        {
            HighlightObject.SetActive(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DOTSInMars.Narrator
{
    [Serializable]
    public class Narration
    {
        public string Text;
        public AudioClip AudioClip;
        public NarrationType Type;
    }

    public class NarrationsDictionaryBehaviour : MonoBehaviour
    {
        [SerializeField] private List<Narration> _narrations;

        public Narration GetNarration(NarrationType type)
        {
            var correctTypes = _narrations.Where(n => n.Type == type);
            if (correctTypes.Count() == 0)
            {
                return null;
            }
            return correctTypes.ElementAt(UnityEngine.Random.Range(0, correctTypes.Count()));
        }
    }
}

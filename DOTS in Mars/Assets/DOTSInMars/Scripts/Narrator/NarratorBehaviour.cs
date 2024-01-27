using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DOTSInMars.Narrator
{
    public class NarratorBehaviour : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] NarrationsDictionaryBehaviour _narrations;


        public Narration GetNarration(NarrationType type)
        {
            return _narrations.GetNarration(type);
        }

        public bool Announce(AudioClip clip)
        {
            if (_audioSource.isPlaying)
            {
                return false;
            }
            _audioSource.clip = clip;
            _audioSource.Play();
            return true;
        }
    }
}

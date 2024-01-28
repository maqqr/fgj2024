using DOTSInMars.Buildings;
using DOTSInMars.Narrator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DOTSInMars
{
    public class Cannon : MonoBehaviour
    {
        public GameObject CannonBallPrefab;
        private BuildingSystem _buildingSystem;

        public List<float> queue = new List<float>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.5f);
            _buildingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BuildingSystem>();
            _buildingSystem.DepositedFinalItem += DepositedFinalItem;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                DepositedFinalItem();
            }

            for (int i = 0; i < queue.Count; i++)
            {
                queue[i] -= Time.deltaTime;
                if (queue[i] <= 0.0f)
                {
                    Shoot();
                    queue.RemoveAt(i);
                }
            }
        }

        private void DepositedFinalItem()
        {
            queue.Add(1.5f);
        }

        private void Shoot()
        {
            GetComponent<ParticleSystem>().Emit(30);
            Instantiate(CannonBallPrefab, transform.position, transform.rotation);
        }
    }
}
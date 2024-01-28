using DOTSInMars.Buildings;
using DOTSInMars.Narrator;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DOTSInMars
{
    public class Cannon : MonoBehaviour
    {
        private BuildingSystem _buildingSystem;

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
            
        }

        private void DepositedFinalItem()
        {
            GetComponent<ParticleSystem>().Emit(30);
        }
    }
}
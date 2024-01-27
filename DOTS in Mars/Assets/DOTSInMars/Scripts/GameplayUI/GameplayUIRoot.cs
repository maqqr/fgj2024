using DOTSInMars.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DOTSInMars.UI
{

    public class GameplayUIRoot : MonoBehaviour
    {
        [SerializeField] private BuildingButton _firstButton;

        private void Start()
        {
            _firstButton.Button.onClick.AddListener(OnFirstButtonClick);
        }


        private void OnFirstButtonClick()
        {
            var spawner = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BuildingSpawnerSystem>();

            spawner.RegisterRaycasting();
        }
    }

}
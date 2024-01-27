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

        private BuildingSpawnerSystem _spawner;
        private BuildingButton _selectedButton;

        private void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            _firstButton.Button.onClick.AddListener(OnFirstButtonClick);
            yield return new WaitForSeconds(0.5f);
            _spawner = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BuildingSpawnerSystem>();
            _spawner.BuildingSet += ResetBuildingPlacement;
        }

        private void ResetBuildingPlacement()
        {
            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();

            }
            _selectedButton = null;
        }

        private void OnFirstButtonClick()
        {
            if (_spawner == null) return;

            _selectedButton = _firstButton;
            _selectedButton.Highlight();
            _spawner.RegisterRaycasting();
        }
    }

}
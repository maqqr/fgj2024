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
        [SerializeField] private Button _background;
        [SerializeField] private BuildingButton _mineButton;
        [SerializeField] private BuildingButton _refineryButton;
        [SerializeField] private BuildingButton _manufacturerButton;
        [SerializeField] private BuildingButton _conveyorButton;

        private BuildingSpawnerSystem _spawner;
        private BuildingButton _selectedButton;

        private void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            _background.onClick.AddListener(BackgroundClicked);
            _mineButton.Button.onClick.AddListener(OnMineButtonClick);
            _refineryButton.Button.onClick.AddListener(OnRefineryButtonClick);
            _manufacturerButton.Button.onClick.AddListener(OnManufacturerButtonClick);
            _conveyorButton.Button.onClick.AddListener(OnConveyorButtonClick);
            yield return new WaitForSeconds(0.5f);
            _spawner = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BuildingSpawnerSystem>();
            _spawner.BuildingSet += ResetBuildingPlacement;
        }

        private void BackgroundClicked()
        {
            _spawner.OnClick();
        }

        private void OnConveyorButtonClick()
        {
            if (_spawner == null) return;

            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();
            }
            _selectedButton = _conveyorButton;
            _selectedButton.Highlight();
            _spawner.RegisterRaycasting(BuildingType.Conveyor);
        }

        private void OnManufacturerButtonClick()
        {
            if (_spawner == null) return;

            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();
            }
            _selectedButton = _manufacturerButton;
            _selectedButton.Highlight();
            _spawner.RegisterRaycasting(BuildingType.Manufacturer);
        }

        private void OnRefineryButtonClick()
        {
            if (_spawner == null) return;

            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();
            }
            _selectedButton = _refineryButton;
            _selectedButton.Highlight();
            _spawner.RegisterRaycasting(BuildingType.Refinery);
        }


        private void OnMineButtonClick()
        {
            if (_spawner == null) return;

            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();
            }
            _selectedButton = _mineButton;
            _selectedButton.Highlight();
            _spawner.RegisterRaycasting(BuildingType.Miner);
        }
        private void ResetBuildingPlacement()
        {
            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();

            }
            _selectedButton = null;
        }
    }

}
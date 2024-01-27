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
using static System.Net.Mime.MediaTypeNames;

namespace DOTSInMars.UI
{

    public class GameplayUIRoot : MonoBehaviour
    {
        [SerializeField] private Button _background;
        [SerializeField] private BuildingButton _mineButton;
        [SerializeField] private BuildingButton _refineryButton;
        [SerializeField] private BuildingButton _manufacturerButton;
        [SerializeField] private BuildingButton _conveyorButton;
        [SerializeField] private GameObject _parentForDisplay;
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private NarratorBehaviour _narrator;

        private BuildingSystem _buildingSystem;
        private BuildingSpawnerSystem _spawner;
        private BuildingButton _selectedButton;
        private int _points = 0;

        private void Start()
        {
            _parentForDisplay.SetActive(false);
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
            _spawner.BuildingSet += BuildingPlaced;
            _spawner.PlayerIdlesWithBuilding += BuildingIdling;
            _buildingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BuildingSystem>();
            _buildingSystem.DepositedFinalItem += DepositedFinalItem;
        }

        private void BuildingIdling(BuildingType type)
        {
            var narration = type switch {
                BuildingType.Manufacturer => NarrationType.ManufacturerPlacementIdling,
                BuildingType.Refinery => NarrationType.RefineryPlacementIdling,
                _ => NarrationType.BuildingPlacementIdling
            };
            HandleNarration(narration);
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
        private void BuildingPlaced(BuildingType type)
        {
            if (_selectedButton != null)
            {
                _selectedButton.Dehighlight();

            }
            _selectedButton = null;

            var narration = type switch
            {
                BuildingType.Manufacturer => NarrationType.ManufacturerDone,
                _ => NarrationType.IdleChatter,
            };
            HandleNarration(narration);
        }

        private void DepositedFinalItem()
        {
            _points++;
            _scoreText.text = _points.ToString();

            HandleNarration(NarrationType.DepositedValuableItems, 2.5f);
        }

        private void HandleNarration(NarrationType narration, float duration = 2.5f)
        {
            StartCoroutine(StartAnnouncements(narration, duration));
        }


        private IEnumerator StartAnnouncements(NarrationType narration, float duration)
        {
            var narrationFound = _narrator.GetNarration(narration);
            if (narrationFound != null)
            {
                if (!string.IsNullOrEmpty(narrationFound.Text))
                {
                    _parentForDisplay.gameObject.SetActive(true);
                    _displayText.text = narrationFound.Text;
                }
                _narrator.Announce(narrationFound.AudioClip);
                yield return new WaitForSeconds(duration);
                _parentForDisplay.gameObject.SetActive(false);
            }
        }
    }

}
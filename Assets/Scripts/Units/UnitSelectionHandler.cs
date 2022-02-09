using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RTS.Networking;
using Mirror;

namespace RTS.UnitNamespace
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private RTSPlayer player;
        private Camera mainCamera;
        private List<Unit> selectedUnits = new List<Unit>();
        private Vector2 startPosition;

        public List<Unit> SelectedUnits { get { return selectedUnits; } }

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;

            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            //player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            Unit.AuthorityOnUnitDespawned += OnAuthorityOnUnitDespawned;
            GameOverHandler.ClientOnGameOver += ClientOnGameOverHandler;
        }


        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= OnAuthorityOnUnitDespawned;
            GameOverHandler.ClientOnGameOver -= ClientOnGameOverHandler;
        }

        // Update is called once per frame
        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Start selection area
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

                if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

                if (!unit.hasAuthority) return;

                selectedUnits.Add(unit);

                SetSelectUnits();

                return;
            }

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2f);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2f);


            foreach (Unit unit in player.MyUnits)
            {
                if (selectedUnits.Contains(unit)) continue;

                Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }

        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            float areaWidth = mousePosition.x - startPosition.x;
            float areaHeight = mousePosition.y - startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2f, areaHeight / 2f);
        }

        private void SetSelectUnits()
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {

                foreach (Unit selectedUnit in selectedUnits)
                {
                    selectedUnit.Deselect();
                }

                selectedUnits.Clear();
            }

            unitSelectionArea.gameObject.SetActive(true);

            startPosition = Mouse.current.position.ReadValue();

            UpdateSelectionArea();
        }

        private void OnAuthorityOnUnitDespawned(Unit obj)
        {
            selectedUnits.Remove(obj);
        }

        private void ClientOnGameOverHandler(string obj)
        {
            enabled = false;
        }
    }
}
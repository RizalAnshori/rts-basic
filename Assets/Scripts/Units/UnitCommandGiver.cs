using RTS.Combat;
using RTS.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.UnitNamespace
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            GameOverHandler.ClientOnGameOver += ClientOnGameOverHandler;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientOnGameOverHandler;
        }


        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
            {
                if(target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(target);
                return;
            }
            TryMove(hit.point);
        }

        private void TryTarget(Targetable target)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.gameObject);
            }
        }

        private void TryMove(Vector3 point)
        {
            foreach(Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.UnitMovement.CmdMove(point);
            }
        }


        private void ClientOnGameOverHandler(string obj)
        {
            enabled = false;
        }
    }
}
using Mirror;
using RTS.Combat;
using RTS.Networking;
using RTS.UnitNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;
        [SerializeField] private TMP_Text remainingUnitsText;
        [SerializeField] private Image unitProgressImage;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7;
        [SerializeField] private float unitSpawnDuration = 5f;

        [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))] private int queuedUnits;
        [SyncVar] private float unitTimer;

        private float progressImageVelocity;

        private void Update()
        {
            if(isServer)
            {
                ProduceUnits();
            }

            if(isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server
        public override void OnStartServer()
        {
            health.ServerOnDie += OnServerOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie += OnServerOnDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (queuedUnits == 0) return;

            unitTimer += Time.deltaTime;

            if (unitTimer < unitSpawnDuration) return;

            GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);

            NetworkServer.Spawn(unitInstance, connectionToClient);

            Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPoint.position.y;

            UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
            unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
            queuedUnits--;
            unitTimer = 0;
        }

        [Server]
        private void OnServerOnDie()
        {
            NetworkServer.Destroy(this.gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (queuedUnits == maxUnitQueue) return;
            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
            if (player.Resources < unitPrefab.ResourceCost) return;
            queuedUnits++;
            player.Resources -= unitPrefab.ResourceCost;
        }
        #endregion

        #region Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left) { return; }

            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        private void ClientHandleQueuedUnitsUpdated(int oldValue, int newValue)
        {
            remainingUnitsText.text = newValue.ToString();
        }

        private void UpdateTimerDisplay()
        {
            float newProgress = unitTimer / unitSpawnDuration;
            if(newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount,newProgress,ref progressImageVelocity, 0.1f);
            }
        }
        #endregion
    }
}
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS.Building
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        #region NormalVar
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;
        #endregion

        #region SyncVar
        #endregion

        #region Server
        [Command]
        private void CmdSpawnUnit()
        {
            GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

            NetworkServer.Spawn(unitInstance, connectionToClient);
        }
        #endregion

        #region Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left) { return; }

            if (!hasAuthority) return;

            CmdSpawnUnit();
        }
        #endregion
    }
}
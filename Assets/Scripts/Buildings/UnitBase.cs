using Mirror;
using RTS.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawned;

        #region Server
        public override void OnStartServer()
        {
            health.ServerOnDie += OnServerOnDIe;

            ServerOnBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= OnServerOnDIe;

            ServerOnBaseDespawned?.Invoke(this);
        }

        [Server]
        private void OnServerOnDIe()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

            NetworkServer.Destroy(this.gameObject);
        }
        #endregion

        #region Client
        #endregion
    }
}
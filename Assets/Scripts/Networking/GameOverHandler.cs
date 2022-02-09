using Mirror;
using RTS.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action ServerOnGameOver;
        public static event Action<string> ClientOnGameOver;

        [SerializeField]private List<UnitBase> unitBases = new List<UnitBase>();

        #region Server
        public override void OnStartServer()
        {
            UnitBase.ServerOnBaseSpawned += OnServerOnBaseSpawned;
            UnitBase.ServerOnBaseDespawned += OnServerOnBaseDespawned;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= OnServerOnBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= OnServerOnBaseDespawned;
        }

        [Server]
        private void OnServerOnBaseSpawned(UnitBase obj)
        {
            unitBases.Add(obj);
        }

        [Server]
        private void OnServerOnBaseDespawned(UnitBase obj)
        {
            unitBases.Remove(obj);

            if (unitBases.Count != 1) return;

            int playerId = unitBases[0].connectionToClient.connectionId;
            RpcGameOver($"Player {playerId}");

            Debug.Log("Game Over");

            ServerOnGameOver?.Invoke();
        }
        #endregion

        #region Client
        [ClientRpc] //the way server can call client method
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }
        #endregion
    }
}
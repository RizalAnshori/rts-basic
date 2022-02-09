using Mirror;
using RTS.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [SyncVar(hook = nameof(OnHealthUpdated))] private int currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdate;

        #region Server
        public override void OnStartServer()
        {
            currentHealth = maxHealth;

            UnitBase.ServerOnPlayerDie += ServerOnPlayerDieHandler;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerOnPlayerDieHandler;
        }

        [Server]
        private void ServerOnPlayerDieHandler(int obj)
        {
            if (connectionToClient.connectionId != obj) return;

            DealDamage(currentHealth);
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (currentHealth == 0) return;

            currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

            if (currentHealth != 0) return;

            ServerOnDie?.Invoke();

            Debug.Log("Died");
        }
        #endregion

        #region Client
        private void OnHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdate?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}
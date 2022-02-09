using Mirror;
using RTS.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.UnitNamespace
{
    public class Unit : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private Health health;
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject selectedHighlightObj;
        [SerializeField] private int resourceCost = 10;

        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public UnitMovement UnitMovement { get { return unitMovement; } }
        public Targeter Targeter { get { return targeter; } }
        public int ResourceCost { get { return resourceCost; } }

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;
        #endregion

        #region SyncVar
        #endregion

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += OnServerOnDie;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= OnServerOnDie;
        }

        [Server]
        private void OnServerOnDie()
        {
            NetworkServer.Destroy(this.gameObject);
        }
        #endregion

        #region Client
        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            onSelected?.Invoke();
            selectedHighlightObj.SetActive(true);
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            onDeselected?.Invoke();
            selectedHighlightObj.SetActive(false);
        }

        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;

            AuthorityOnUnitDespawned?.Invoke(this);
        }
        #endregion
    }
}
using Mirror;
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
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private GameObject selectedHighlightObj;

        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;

        public UnitMovement UnitMovement { get { return unitMovement; } }

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
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnUnitDespawned?.Invoke(this);
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

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"Called in : {connectionToClient.identity.gameObject.name}");
            if (!isClientOnly || !hasAuthority) return;

            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isClientOnly || !hasAuthority) return;

            AuthorityOnUnitDespawned?.Invoke(this);
        }
        #endregion
    }
}
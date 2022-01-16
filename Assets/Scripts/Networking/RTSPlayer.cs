using Mirror;
using RTS.UnitNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private List<Unit> myUnits = new List<Unit>();

        public List<Unit> MyUnits { get { return myUnits; } }
        #endregion

        #region SyncVar
        #endregion

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            Unit.ServerOnUnitSpawned += ServerOnUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerOnUnitDespawned;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.ServerOnUnitSpawned -= ServerOnUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerOnUnitDespawned;
        }

        private void ServerOnUnitSpawned(Unit obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myUnits.Add(obj);
        }

        private void ServerOnUnitDespawned(Unit obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myUnits.Remove(obj);
        }
        #endregion

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isClientOnly) return;

            Unit.AuthorityOnUnitSpawned += AuthorityOnUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityOnUnitDespawned;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!isClientOnly) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityOnUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityOnUnitDespawned;
        }

        private void AuthorityOnUnitDespawned(Unit obj)
        {
            if (!hasAuthority) return;

            myUnits.Add(obj);
        }

        private void AuthorityOnUnitSpawned(Unit obj)
        {
            if (!hasAuthority) return;

            myUnits.Remove(obj);
        }
        #endregion
    }
}
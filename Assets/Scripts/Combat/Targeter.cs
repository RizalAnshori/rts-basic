using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private Targetable target;

        public Targetable Target { get { return target; } }
        #endregion

        #region SyncVar
        #endregion

        #region Server
        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerOnGameOverHandler;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerOnGameOverHandler;
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) return;

            this.target = target;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        [Server]
        private void ServerOnGameOverHandler()
        {
            ClearTarget();
        }
        #endregion

        #region Client
        #endregion
    }
}
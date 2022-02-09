using Mirror;
using RTS.Combat;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RTS.UnitNamespace
{
    public class UnitMovement : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private Targeter targeter;
        [SerializeField] private float chaseRange = 10f;

        //private Camera mainCamera;
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
    

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.Target;

            if(target != null)
            {
                if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else if(agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            if (!agent.hasPath) return;

            if (agent.remainingDistance > agent.stoppingDistance) return;

            agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            ServerMove(position);
        }

        [Server]
        public void ServerMove(Vector3 position)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return;
            }

            agent.SetDestination(hit.position);
        }

        [Server]
        private void ServerOnGameOverHandler()
        {
            agent.ResetPath();
        }
        #endregion

        #region Client
        //public override void OnStartAuthority()
        //{
        //    base.OnStartAuthority();

        //    mainCamera = Camera.main;
        //}

        //[ClientCallback]
        //private void Update()
        //{
        //    if (!hasAuthority)
        //    {
        //        return;
        //    }

        //    if (!Mouse.current.rightButton.wasPressedThisFrame)
        //    {
        //        return;
        //    }

        //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        //    CmdMove(hit.point);
        //}
        #endregion
    }
}
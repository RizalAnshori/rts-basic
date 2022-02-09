using Mirror;
using RTS.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.UnitNamespace
{
    public class UnitFiring : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float rotationSpeed = 20f;

        private float lastFireTime;
        #endregion

        #region SyncVar
        #endregion

        #region Server
        [ServerCallback]
        private void Update()
        {
            if (targeter.Target == null) return;

            if (!CanFireAtTarget()) return;

            Quaternion targetRotation = Quaternion.LookRotation(targeter.Target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if(Time.time > (1/fireRate)+lastFireTime)
            {
                Quaternion projectileRotation = Quaternion.LookRotation(targeter.Target.AimAtPoint.position - projectileSpawnPoint.position);

                GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position,projectileRotation);

                NetworkServer.Spawn(projectileInstance, connectionToClient);

                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.Target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
        }
        #endregion

        #region Client
        #endregion
    }
}
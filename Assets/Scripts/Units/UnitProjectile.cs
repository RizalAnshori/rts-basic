using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTS.Combat;

namespace RTS.UnitNamespace
{
    public class UnitProjectile : NetworkBehaviour
    {
        #region NormalVar
        [SerializeField] private Rigidbody rb;
        [SerializeField] private int damageToDeal = 20;
        [SerializeField] private float destroyAfterSeconds = 5f;
        [SerializeField] private float launchForce = 10f;
        #endregion

        #region SyncVar
        #endregion

        #region Server
        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) return;
            }

            if(other.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(damageToDeal);
            }

            DestroySelf();
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region Client
        private void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }
        #endregion
    }
}
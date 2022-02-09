using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint;

        public Transform AimAtPoint { get { return aimAtPoint; } }
    }
}
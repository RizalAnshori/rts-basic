using Mirror;
using RTS.Buildings;
using RTS.UnitNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        public event Action<int> ClientOnResourcesUpdated;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Building[] buildings;
        [SerializeField] private List<Unit> myUnits = new List<Unit>();
        [SerializeField] private List<Building> myBuildings = new List<Building>();
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private float buildingRangeLimit = 5f;

        [SyncVar(hook =nameof(ClientHandleResourceUpdated ))] private int resources = 500;
        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = true;
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] private string displayName;

        private Color teamColor = new Color();

        public List<Unit> MyUnits { get { return myUnits; } }
        public List<Building> MyBuildings { get { return myBuildings; } }
        public int Resources { get { return resources; } [Server]set { resources = value; } }
        public Transform CameraTransform { get { return cameraTransform; } }
        public Color TeamColor { get { return teamColor; } [Server]set { teamColor = value; } }
        public bool IsPartyOwner { get { return isPartyOwner; } [Server]set { isPartyOwner = value; } }
        public string DisplayName { get { return displayName; } [Server]set { displayName = value; } }

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            {
                return false;
            }

            foreach (var building in myBuildings)
            {
                if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
                {
                    return true; 
                }
            }

            return false;
        }

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            Unit.ServerOnUnitSpawned += ServerOnUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerOnUnitDespawned;
            Building.ServerOnBuildingSpawned += ServerOnBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerOnBuildingDespawned;

            DontDestroyOnLoad(gameObject);
        }
        
        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.ServerOnUnitSpawned -= ServerOnUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerOnUnitDespawned;
            Building.ServerOnBuildingSpawned -= ServerOnBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerOnBuildingDespawned;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!isPartyOwner) return;

            ((RTSNetworkManager)NetworkManager.singleton).StartGame();
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
        {
            Building buildingToPlace = null;

            foreach(Building building in buildings)
            {
                if(building.Id == buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (buildingToPlace == null) return;

            if (resources < buildingToPlace.Price) return;

            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

            if (!CanPlaceBuilding(buildingCollider, point)) return;

            GameObject buildingToPlaceInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

            NetworkServer.Spawn(buildingToPlaceInstance, connectionToClient);

            Resources -= buildingToPlace.Price;
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

        private void ServerOnBuildingSpawned(Building obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myBuildings.Add(obj);
        }

        private void ServerOnBuildingDespawned(Building obj)
        {
            if (obj.connectionToClient.connectionId != connectionToClient.connectionId) return;
            myBuildings.Remove(obj);
        }

        #endregion

        #region 

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.AuthorityOnUnitSpawned += AuthorityOnUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityOnUnitDespawned;
            Building.AuthorityOnBuildingSpawned += AuthorityOnBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityOnBuildingDespawned;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) return;

            DontDestroyOnLoad(gameObject);

            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            ClientOnInfoUpdated?.Invoke();

            if (!isClientOnly) return;

            ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

            if (!hasAuthority) return;

            Unit.AuthorityOnUnitSpawned -= AuthorityOnUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityOnUnitDespawned;
            Building.AuthorityOnBuildingSpawned -= AuthorityOnBuildingSpawned;
            Building.AuthorityOnBuildingDespawned -= AuthorityOnBuildingDespawned;
        }

        private void AuthorityOnUnitSpawned(Unit obj)
        {
            myUnits.Add(obj);
        }

        private void AuthorityOnUnitDespawned(Unit obj)
        {
            myUnits.Remove(obj);
        }

        private void AuthorityOnBuildingSpawned(Building obj)
        {
            myBuildings.Add(obj);
        }

        private void AuthorityOnBuildingDespawned(Building obj)
        {
            myBuildings.Remove(obj);
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
        {
            if (!hasAuthority) return;

            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
        }

        private void ClientHandleResourceUpdated(int oldValue, int newValue)
        {
            ClientOnResourcesUpdated?.Invoke(newValue);
        }

        private void ClientHandleDisplayNameUpdated(string oldDisplayNam, string newDisplayName)
        {
            ClientOnInfoUpdated?.Invoke();
        }
        #endregion
    }
}
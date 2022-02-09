using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTS.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] GameOverHandler gameOverHandlerPrefab;

        private bool isGameInProgress = false;

        public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

        #region Server
        public override void OnServerConnect(NetworkConnection conn)
        {
            if (!isGameInProgress) return;

            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            var player = conn.identity.GetComponent<RTSPlayer>();

            Players.Remove(player);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) return;

            isGameInProgress = true;

            ServerChangeScene("Scene_Map_01");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

            Players.Add(player);

            player.DisplayName=$"Player {Players.Count}";

            player.TeamColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            player.IsPartyOwner = Players.Count == 1;

            //GameObject unitySpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

            //NetworkServer.Spawn(unitySpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string newSceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

                foreach(var player in Players)
                {
                    GameObject unityBaseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);

                    NetworkServer.Spawn(unityBaseInstance, player.connectionToClient);
                }
            }
        }
        #endregion

        #region Client
        public override void OnClientConnect()
        {
            base.OnClientConnect();

            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear(); 
        }
        #endregion
    }
}
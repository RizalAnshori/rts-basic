using Mirror;
using RTS.Combat;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();
        health.ServerOnDie += ServerOnDieHandler;
        GameOverHandler.ServerOnGameOver += ServerOnGameOverHandler;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerOnDieHandler;
        GameOverHandler.ServerOnGameOver -= ServerOnGameOverHandler;
    }

    private void ServerOnDieHandler()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    private void ServerOnGameOverHandler()
    {
        enabled = false;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            player.Resources += resourcesPerInterval;
            timer += interval;
        }
    }
}

using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText;

    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientOnResourcesUpdatedHandler(player.Resources);

        player.ClientOnResourcesUpdated += ClientOnResourcesUpdatedHandler;
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientOnResourcesUpdatedHandler;
    }

    private void ClientOnResourcesUpdatedHandler(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}

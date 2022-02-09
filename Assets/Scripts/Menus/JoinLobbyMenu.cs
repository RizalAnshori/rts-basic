using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Button joinButton;

    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += ClientConnectedHandler;
        RTSNetworkManager.ClientOnDisconnected += ClientDisconnectedHandler;
    }

    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= ClientConnectedHandler;
        RTSNetworkManager.ClientOnDisconnected -= ClientDisconnectedHandler;
    }

    public void Join()
    {
        string address = addressInput.text;
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void ClientConnectedHandler()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void ClientDisconnectedHandler()
    {
        joinButton.interactable = true;
    }

}

using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4]; 

    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += ClientOnConnectedHandler;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityOnPartyOwnerStateUpdatedHandler;
        RTSPlayer.ClientOnInfoUpdated += ClientOnInfoUpdatedHandler;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= ClientOnConnectedHandler;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityOnPartyOwnerStateUpdatedHandler;
        RTSPlayer.ClientOnInfoUpdated -= ClientOnInfoUpdatedHandler;
    }

    private void ClientOnInfoUpdatedHandler()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

        for(int i = 0; i< playerNameTexts.Length; i++)
        {
            if(i<players.Count)
            {
                playerNameTexts[i].text = players[i].DisplayName;
            }
            else
            {
                playerNameTexts[i].text = "Waiting For Player ......";
            }
        }

        startGameButton.interactable = players.Count > 1;
    }

    private void ClientOnConnectedHandler()
    {
        lobbyUI.SetActive(true);
    }

    private void AuthorityOnPartyOwnerStateUpdatedHandler(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}

using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_Text debugText;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!useSteam) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);
        debugText.text += $"\nHost Lobby Called using stem = {useSteam}";

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            debugText.text += "\nLobby Created Called";
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        debugText.text += "\n On Lobby Created Called";

        if(callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            debugText.text += "\n On Lobby Created Error";
            return;
        }

        debugText.text += "\n On Lobby Created No Error";

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAddress",SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;


        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        landingPagePanel.SetActive(false);
    }
}

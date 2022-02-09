using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent;
    [SerializeField] private TMP_Text winnerNameText;

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += OnClientOnGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= OnClientOnGameOver;
    }

    private void OnClientOnGameOver(string obj)
    {
        winnerNameText.text = $"{obj} Has Won!";

        gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}

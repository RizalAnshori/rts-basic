using Mirror;
using RTS.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers;

    [SyncVar(hook =nameof(TeamColorUpdatedHandler))] private Color teamColor = new Color();

    #region Server
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamColor = player.TeamColor;
    }
    #endregion

    #region Client
    private void TeamColorUpdatedHandler(Color oldColor, Color newColor)
    {
        foreach(var renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }
    #endregion
}

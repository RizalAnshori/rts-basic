using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCamTransform;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Controls controls;
    private Vector2 previousInput;

    #region Server
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        playerCamTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += OnMoveCamera;
        controls.Player.MoveCamera.canceled += OnMoveCamera;

        controls.Enable();
    }

    private void OnMoveCamera(UnityEngine.InputSystem.InputAction.CallbackContext inputValue)
    {
        previousInput = inputValue.ReadValue<Vector2>();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) return;

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCamTransform.position;
        if(previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;
            Vector2 cursorPos = Mouse.current.position.ReadValue();

            if (cursorPos.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if(cursorPos.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPos.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPos.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCamTransform.position = pos;
    }
    #endregion
}

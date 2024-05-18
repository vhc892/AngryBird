using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;
    public static Vector2 mousePosition;
    public static bool wasLeftMouseButtonPressed;
    public static bool wasLeftMouseButtonReleased;
    public static bool isLeftMousePressed;

    private InputAction mousePositionAction;
    private InputAction mouseAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mousePositionAction = playerInput.actions["MousePosition"];
        mouseAction = playerInput.actions["Mouse"];
    }
    private void Update()
    {
        mousePosition = mousePositionAction.ReadValue<Vector2>();
        wasLeftMouseButtonPressed = mouseAction.WasPressedThisFrame();
        wasLeftMouseButtonReleased = mouseAction.WasReleasedThisFrame();
        isLeftMousePressed = mouseAction.IsPressed();

    }
}

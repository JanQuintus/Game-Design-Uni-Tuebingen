using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class ToolTester : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public ATool Tool;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        inputActions.Default.Shoot.performed += Shoot;
        inputActions.Default.RightClick.performed += RightClick;
    }

    private void OnDisable()
    {
        inputActions.Default.Shoot.performed -= Shoot;
        inputActions.Default.RightClick.performed -= RightClick;
    }

    private void Shoot(CallbackContext ctx) => Tool.Shoot();
    private void RightClick(CallbackContext ctx) => Tool.Shoot(true);

}

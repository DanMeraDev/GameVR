using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public InputActionAsset inputAsset;
    private InputAction inputAction;
    private InputActionMap inputActionMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inputAsset != null) return;
        inputActionMap = inputAsset.FindActionMap("Player");
        inputAction = inputActionMap.FindAction("Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (inputAction.WasPressedThisFrame())
        {
            Invoke("isPressed", 3f);
        }        
    }
    void isPressed()
    {
        if (inputAction.WasPressedThisFrame()) SceneMessenger.LoadMenu();
    }
}

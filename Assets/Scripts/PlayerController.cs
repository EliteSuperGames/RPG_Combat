using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;

    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
    }

    // Update is called once per frame
    void OnEnable()
    {
        playerControls.Enable();
    }

    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;
        // Debug.Log(x + "," + z);
    }
}

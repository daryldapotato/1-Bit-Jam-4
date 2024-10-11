using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;

    public CharacterController2D characterController;

    private float horizontalInput = 0f;
    private bool jumpInput = false;

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpInput = true;
    }

    private void FixedUpdate()
    {
        characterController.Move(horizontalInput * moveSpeed * Time.fixedDeltaTime, jumpInput);
        jumpInput = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private MovementController controller;

    [SerializeField] private float moveSpeed = 40f;
    private float horizontalMove = 0f;
    private bool isJumping = false;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, isJumping);
        isJumping = false;
    }
}

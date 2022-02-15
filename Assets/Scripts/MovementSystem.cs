using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    CharacterController controller;
    Rigidbody rb;
    Vector3 moveDirection;
    float speed = 10f;
    float jumpSpeed = 10f;
    float gravity = 9.81f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {

        //if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        //moveDirection.y -= gravity* speed * Time.deltaTime;

        rb.velocity = moveDirection;

        //transform.Translate(moveDirection * Time.deltaTime);
        //controller.Move(moveDirection * Time.deltaTime);
    }


}

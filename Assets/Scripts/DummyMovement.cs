using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMovement : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDirection;
    float speed = 10f;
    float jumpSpeed = 10f;
    float gravity = 9.81f;

    public float hMove;
    public float vMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(RandomPattern());

    }

    void Update()
    {

        //if (controller.isGrounded)
        {
            moveDirection = new Vector3(hMove, 0, vMove);

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
    IEnumerator RandomPattern()
    {
        while (true)
        {
            hMove = Random.Range(-1.0f, 1.0f);
            vMove = Random.Range(-1.0f, 1.0f);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }

    public float RandomSpeed()
    {
        return Random.Range(-1.0f, 1.0f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMovement : MonoBehaviour
{
    public bool grounded;
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

            //if (Input.GetButton("Jump"))
            //    moveDirection.y = jumpSpeed;

        }

        float distToGround = gameObject.transform.localScale.y / 2;

        if (!Physics.Raycast(transform.position, -Vector3.up, distToGround+ 0.1f))
        {
            moveDirection.y -= Mathf.Pow(gravity,2) * Time.deltaTime;
            grounded = false;
        }
        else
        {
            grounded = true;
        }

        Debug.DrawRay(transform.position + Vector3.down * (distToGround+0.1f), Vector3.up*3, Color.blue) ;


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

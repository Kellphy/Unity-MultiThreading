using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMovement : MonoBehaviour
{
    public bool grounded;
    Rigidbody rb;
    Vector3 moveDirection;
    float speed = 10f;
    float jumpSpeed = 1f;
    float gravity = 5f;

    public float hMove;
    public float vMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(RandomPattern());
    }

    void FixedUpdate()
    {
        float distToGround = gameObject.transform.localScale.y / 2;

        if (!Physics.Raycast(transform.position, -Vector3.up, distToGround+ 0.1f))
        {
            moveDirection.y -= gravity * Time.deltaTime;
            grounded = false;
        }
        else
        {
            grounded = true;
        }

        Debug.DrawRay(transform.position + Vector3.down * (distToGround+0.1f), Vector3.up*3, Color.blue) ;

        if (grounded)
        {
            moveDirection = new Vector3(hMove, 0, vMove);

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

            moveDirection *= speed;
        }
        #region Test Bounds
        float bounds = 75f;
        if (transform.position.x + 0.5f > bounds || transform.position.x + 0.5f < -bounds)
        {
            moveDirection.x *= -1;
            transform.position = new Vector3() { x = (bounds -1)* Mathf.Sign(transform.position.x), y = transform.position.y, z = transform.position.z };
        }
        if (transform.position.z + 0.5f > bounds || transform.position.z + 0.5f < -bounds)
        {
            moveDirection.z *= -1;
            transform.position = new Vector3() { x = transform.position.x, y = transform.position.y, z = (bounds - 1) * Mathf.Sign(transform.position.z) };
        }
        if(transform.position.y < 1f)
        {
            transform.position = new Vector3() { x = transform.position.x, y = 1f, z = transform.position.z };
        }
        #endregion
        rb.velocity = moveDirection;
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

    public float ObjectSpeed()
    {
        return Random.Range(-1.0f, 1.0f);
    }

}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlayer : NetworkBehaviour
{
    Rigidbody rb;

    Vector3 moveDirection;

    float speed = 10f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) return;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed;
        rb.velocity = moveDirection;
    }
}

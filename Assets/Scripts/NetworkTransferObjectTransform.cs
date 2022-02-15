using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransferObjectTransform : NetworkBehaviour
{
    [SyncVar] Vector3 currentPosition;
    [SyncVar] Quaternion currentRotation;

    Vector3 moveDirection;
    public float hMove;
    public float vMove;
    float speed = 10f;
    Rigidbody rb;

    private void Awake()
    {
        currentPosition = transform.position;
        currentRotation = transform.rotation;
        if (isServer)
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(RandomPattern());
        }

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

    [ClientRpc]
    private void RpcSyncPositionWithClients(Vector3 positionToSync)
    {
        currentPosition = positionToSync;
    }

    [ClientRpc]
    private void RpcSyncRotationWithClients(Quaternion rotationToSync)
    {
        currentRotation = rotationToSync;
    }

    private void Update()
    {
        if (isServer)
        {
            moveDirection = new Vector3(hMove, 0, vMove);

            moveDirection *= speed;

            rb.velocity = moveDirection;

            RpcSyncPositionWithClients(this.transform.position);
            RpcSyncRotationWithClients(this.transform.rotation);
        }
    }

    private void LateUpdate()
    {
        if (!isServer)
        {
            this.transform.position = currentPosition;
            this.transform.rotation = currentRotation;
        }
    }
}

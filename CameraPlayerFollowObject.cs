using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollowObject : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        if (player.position.y > transform.position.y)
            transform.position = player.position;
    }
}

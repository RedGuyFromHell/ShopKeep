using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool isPlayerStationary = false;
    public Transform player;

    Vector3 lastPlayerPos;
    private void Start()
    {
        this.transform.position = player.position + new Vector3(0, 0, -10);
        lastPlayerPos = player.position;
    }

    float lastX = 0;
    private void Update()
    {
        if (player.position != lastPlayerPos && !isPlayerStationary)
        {
            if (player.position.x >= -2.5f && player.position.x <= 7)
            {
                this.transform.position = player.position + new Vector3(0, 0, -10);
                lastX = player.position.x;
            }
            else
                this.transform.position = new Vector3(lastX, player.position.y, -10f);

            lastPlayerPos = player.position;
        }
    }
}

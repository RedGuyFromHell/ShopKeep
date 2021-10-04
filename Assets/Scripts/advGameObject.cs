using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advGameObject : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x > 10.0f)
        {
            Destroy(gameObject);
        }
    }
}

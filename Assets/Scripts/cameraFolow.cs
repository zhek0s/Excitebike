using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFolow : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}

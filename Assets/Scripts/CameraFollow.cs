using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CameraFollow : MonoBehaviour
{
    private float cameraX, cameraY; 
    private Vector3 offset = new Vector3(0f, 0f, 10f);
    

    [SerializeField]
    private Transform target;

    private void Start()
    {
        transform.position = new Vector3(0f, 5f, -10f);
        cameraX = transform.position.x;
        cameraY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // camera should follow player in the z direction only
        transform.position = new Vector3(cameraX, cameraY, target.position.z - offset.z);
    }
}

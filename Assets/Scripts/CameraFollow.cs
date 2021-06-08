using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    private Vector3 velocity;
    private Vector3 targetPosition;
    private void Start()
    {
        
    }

    private void Update()
    {
        if(target != null)
        {
            targetPosition = target.position;
            targetPosition.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.2f);
        }
    }
}

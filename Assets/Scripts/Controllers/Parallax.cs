using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    Vector2 startPosition;
    float startZ;

    Vector2 travel => (Vector2)target.transform.position - startPosition;

    float distanceFromTarget => transform.position.z - target.position.z;
    float clippingPlane => cam.transform.position.z + (distanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane);
    float parallaxFactor => Mathf.Abs(distanceFromTarget) / clippingPlane;

    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    void Update()
    {
        //DEBUGGING
        Debug.Log("Distance From Target: " + distanceFromTarget);
        Debug.Log("Parallax Factor: " + parallaxFactor);
        Debug.Log("Travel: " + travel);

        Vector2 newPos = startPosition + travel * parallaxFactor;

        Debug.Log("New Position: " + newPos);

        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}

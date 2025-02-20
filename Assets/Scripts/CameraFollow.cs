using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // The target for the camera to follow (your character)
    public float smoothSpeed = 0.125f;  // Smoothness of the camera movement
    public Vector3 offset;        // Offset between the camera and the target

    public float minY = 0f;       // Minimum Y value the camera can go
    public float maxY = 10f;      // Maximum Y value the camera can go

    public float minX = 0f;       // Minimum Y value the camera can go
    public float maxX = 10f;      // Maximum Y value the camera can go

    void LateUpdate()
    {
        // Calculate the desired position based on the target's position and the offset
        Vector3 desiredPosition = target.position + offset;

        // Clamp the X and Y position between the minY and maxY constraints
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
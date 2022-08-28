using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Lock and hide cursor?
    public bool lockCursor;

    // Speed of camera movement
    public float mouseSensitivity = 10;

    // Initial distance from the target
    public float distanceFromTarget = 5;

    public float minDistanceFromTarget = 2;

    public float maxDistanceFromTarget = 10;

    // Min and max pitch angle allowed when focused on a target
    public float targetedPitchMin = 10;
    public float targetedPitchMax = 85;

    // Min and max pitch angle allowed when moving freely
    private float globalPitchMin = -85;
    private float globalPitchMax = 85;

    // How fast camera zooms in or out when scrolling
    public float scrollScale = 0.3f;

    // Smoothing of rotation movement
    public float rotationSmoothTime = 0.12f;

    // Max movement speed
    public float maxSpeed = 10f;

    // Smoothing of forward/sideways movement
    public float movementSmoothTime = 0.12f;

    // Camera crosshair
    public GameObject crosshair;


    // Target for the camera to focus on
    private Transform target;

    // Rotation variables to create smoothing effect
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    // Y axis rotation
    private float yaw;

    // X axis rotation
    private float pitch;

    // Movement variables
    private Vector3 movementSmoothVelocity;
    private float forward;
    private float sideways;

    private void Start()
    {
        if (lockCursor == true)
        {
            // Lock cursor to the screen and hide it from user
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Use LateUpdate so that it's called after all other update methods;
    // this way the target's position will have already been set when camera updates
    private void LateUpdate()
    {
        // Set our yaw and pitch based on the position of the mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(
            pitch, 
            (target != null) ? targetedPitchMin : globalPitchMin,
            (target != null) ? targetedPitchMax : globalPitchMax
        );


        // Get the input for moving back and forth and sideways
        forward = Input.GetAxis("Vertical");
        sideways = Input.GetAxis("Horizontal");


        // Remove target if we use WASD to move
        if (Mathf.Abs(forward) > float.Epsilon)
            RemoveTarget();

        if (Mathf.Abs(sideways) > float.Epsilon)
            RemoveTarget();


        // Change the distance from target based on the mouse scrolling
        distanceFromTarget = Mathf.Min(
            maxDistanceFromTarget, 
            Mathf.Max(
                minDistanceFromTarget,
                distanceFromTarget - Input.mouseScrollDelta.y * scrollScale
            )
        );

        // Smooth the rotation of the camera towards our current
        // yaw and pitch using the rotationSmoothTime
        currentRotation = Vector3.SmoothDamp(
            currentRotation,
            new Vector3(pitch, yaw),
            ref rotationSmoothVelocity,
            rotationSmoothTime
        );


        // Finally set the rotation
        transform.eulerAngles = currentRotation;

        // If we have a target, set our position according to it
        if (target != null)
        {
            // Set the position based on the distanceFromTarget
            transform.position = target.position - transform.forward * distanceFromTarget;
            return;
        }

        // Otherwise figure out our direction based on the local position
        Vector3 movementDir = (transform.forward * forward) + (transform.right * sideways);

        // And smooth our way towards that direction
        transform.position = Vector3.SmoothDamp(
            transform.position,
            transform.position + movementDir,
            ref movementSmoothVelocity,
            movementSmoothTime,
            maxSpeed
        );

        // Also ensure the crosshair is active when we have no targets
        crosshair.SetActive(true);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        crosshair.SetActive(false);
    }

    public void RemoveTarget()
    {
        target = null;
        crosshair.SetActive(true);
    }
}

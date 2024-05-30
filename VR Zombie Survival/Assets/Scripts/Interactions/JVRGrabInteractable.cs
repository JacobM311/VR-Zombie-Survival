using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class JVRGrabInteractable : MonoBehaviour
{
    [SerializeField]
    private List<Transform> LeftHandAttachTransforms;
    [SerializeField]
    private List<Transform> RightHandAttachTransforms;

    private JVRDirectInteractor _currentInteractor;
    private Transform _attachTransform;
    public bool hoverable = true;
    private bool grabbing = false;
    private bool swappable = true;

    private Transform target;
    private Rigidbody rb;

    public float positionLerpSpeed = 0.1f;
    public float rotationLerpSpeed = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void Hovered(JVRDirectInteractor interactor)
    {
        if (interactor != null)
        {
            _currentInteractor = interactor;
            _currentInteractor.TriggerHover(this);

            if (interactor.grabAction.action.ReadValue<float>() > .8f)
            {
                interactor.canDetectGrabbable = false;
                SelectEntering(interactor);
            }
        }
        else { Debug.Log("Interactor not found."); }
    }

    private void SelectEntering(JVRDirectInteractor interactor)
    {
        _currentInteractor.TriggerSelectEntering(this);

        if (interactor.handedness == JVRDirectInteractor.Handedness.Right)
        {
            _attachTransform = GetClosestTransform(interactor.transform.position, RightHandAttachTransforms);
        }
        else if (interactor.handedness == JVRDirectInteractor.Handedness.Left)
        {
            _attachTransform = GetClosestTransform(interactor.transform.position, LeftHandAttachTransforms);
        }
        else { Debug.Log("Direct Interactor Handedness not set."); }

        SelectEntered(interactor);
    }

    private void SelectEntered(JVRDirectInteractor interactor)
    {
        grabbing = true;
        _currentInteractor.TriggerSelectEntered(this);
    }

    private void SelectExited()
    {
        grabbing = false;
        _currentInteractor.canDetectGrabbable = true;
        _currentInteractor = null;
        _attachTransform = null;
    }

    void Update()
    {
        if (!grabbing) { return; }

        if (_currentInteractor.grabAction.action.ReadValue<float>() < .01f)
        {
            SelectExited();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (grabbing)
        {
            PerformGrab();
        }
        
    }

    private void PerformGrab()
    {
        // Calculate the target position with offset
        Vector3 targetPositionWithOffset = _currentInteractor.transform.TransformPoint(_attachTransform.localPosition);
        Quaternion targetRotationWithOffset = _currentInteractor.transform.rotation * _attachTransform.localRotation;

        // Calculate the velocity needed to move the object to the target position
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPositionWithOffset, positionLerpSpeed);
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime / 200f;

        // Calculate the target rotation with offset
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotationWithOffset, rotationLerpSpeed);
        Quaternion rotationDifference = newRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        // Prevent weird rotations when rotating past 180 degrees
        if (angleInDegrees > 180) angleInDegrees -= 360;

        // Calculate the angular velocity needed to move the object to the target rotation
        Vector3 angularVelocity = (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.fixedDeltaTime / 200f;
        rb.angularVelocity = angularVelocity;
    }

    private Transform GetClosestTransform(Vector3 interactorPosition, List<Transform> possibleTransforms)
    {
        Transform closestTransform = null;
        float minDistance = float.MaxValue;

        foreach (Transform transform in possibleTransforms)
        {
            float distance = Vector3.Distance(interactorPosition, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTransform = transform;
            }
        }

        return closestTransform;
    }
}
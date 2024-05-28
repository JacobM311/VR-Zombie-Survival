using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class JVRGrabInteractable : MonoBehaviour
{
    [SerializeField]
    private List<Transform> LeftHandAttachTransforms;
    [SerializeField]
    private List<Transform> RightHandAttachTransforms;

    private Transform _attachTransform;
    private bool hoverable = true;
    private bool isMoving = false;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private Vector3 _attachOffsetPosition;
    private Quaternion _attachOffsetRotation;

    private Rigidbody _rigidbody;

    private float velocityDamping = 0.9f;
    private float velocityScale = 1f;
    private float angularVelocityDamping = 0.9f;
    private float angularVelocityScale = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing.");
        }
    }

    public void OnHovered(JVRDirectInteractor interactor)
    {
        if (!hoverable) { return; }

        if (interactor != null)
        {
            if (interactor.grabAction.action.ReadValue<float>() > .8f)
            {
                OnSelectEntering(interactor);
            }
        }
        else
        {
            Debug.Log("Interactor not found.");
        }
    }

    private void OnSelectEntering(JVRDirectInteractor interactor)
    {
        if (interactor.handedness == JVRDirectInteractor.Handedness.Right)
        {
            _attachTransform = GetClosestTransform(interactor.transform.position, RightHandAttachTransforms);
        }
        else if (interactor.handedness == JVRDirectInteractor.Handedness.Left)
        {
            _attachTransform = GetClosestTransform(interactor.transform.position, LeftHandAttachTransforms);
        }
        else { Debug.Log("Direct Interactor Handedness not set."); }

        _attachOffsetPosition = transform.position - _attachTransform.position;
        _attachOffsetRotation = Quaternion.Inverse(_attachTransform.rotation) * transform.rotation;

        StartMoving(interactor);
    }

    private void StartMoving(JVRDirectInteractor interactor)
    {
        targetPosition = interactor.transform.position;
        targetRotation = interactor.transform.rotation;

        isMoving = true;
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            PerformVelocityTrackingUpdate(Time.fixedDeltaTime);
        }
    }

    private void PerformVelocityTrackingUpdate(float deltaTime)
    {
        if (deltaTime < Mathf.Epsilon)
            return;

        // Do linear velocity tracking
        if (_rigidbody != null)
        {
            _rigidbody.velocity *= (1f - velocityDamping);
            var positionDelta = targetPosition - transform.position;
            var velocity = positionDelta / deltaTime;
            _rigidbody.velocity += (velocity * velocityScale);

            // Do angular velocity tracking
            _rigidbody.angularVelocity *= (1f - angularVelocityDamping);
            var rotationDelta = targetRotation * Quaternion.Inverse(transform.rotation);
            rotationDelta.ToAngleAxis(out var angleInDegrees, out var rotationAxis);
            if (angleInDegrees > 180f)
                angleInDegrees -= 360f;

            if (Mathf.Abs(angleInDegrees) > Mathf.Epsilon)
            {
                var angularVelocity = (rotationAxis * (angleInDegrees * Mathf.Deg2Rad)) / deltaTime;
                _rigidbody.angularVelocity += (angularVelocity * angularVelocityScale);
            }
        }

        Debug.Log("moving");

        // Check if the object has reached the target position and rotation
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f &&
            Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
        {
            
        }
    }
}
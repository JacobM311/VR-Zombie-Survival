using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class JVRGrabInteractable : MonoBehaviour
{
    [SerializeField]
    private List<Transform> LeftHandAttachTransforms;
    [SerializeField]
    private List<Transform> RightHandAttachTransforms;

    private JVRDirectInteractor _interactor;
    private Transform _attachTransform;
    public bool hoverable = true;
    private bool isMoving = false;

    private Transform target;
    private Rigidbody rb;

    public float positionLerpSpeed = 0.1f;
    public float rotationLerpSpeed = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    public void OnHovered(JVRDirectInteractor interactor)
    {
        if (interactor != null)
        {
            _interactor = interactor;
            target = _interactor.transform;
            if (_interactor.grabAction.action.ReadValue<float>() > .8f)
            {
                OnSelectEntering();
            }
        }
        else
        {
            Debug.Log("Interactor not found.");
        }
    }

    private void OnSelectEntering()
    {
        hoverable = false;

        if (_interactor.handedness == JVRDirectInteractor.Handedness.Right)
        {
            _attachTransform = GetClosestTransform(_interactor.transform.position, RightHandAttachTransforms);
        }
        else if (_interactor.handedness == JVRDirectInteractor.Handedness.Left)
        {
            _attachTransform = GetClosestTransform(_interactor.transform.position, LeftHandAttachTransforms);
        }
        else { Debug.Log("Direct Interactor Handedness not set."); }

        isMoving = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isMoving)
        {
            PerformEaseIn();
        }
        
    }

    private void PerformEaseIn()
    {
        // Calculate the target position with offset
        Vector3 targetPositionWithOffset = _interactor.transform.TransformPoint(_attachTransform.localPosition);
        Quaternion targetRotationWithOffset = _interactor.transform.rotation * _attachTransform.localRotation;

        Vector3 newPosition = Vector3.Lerp(transform.position, targetPositionWithOffset, positionLerpSpeed);
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime;

        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotationWithOffset, rotationLerpSpeed);
        Quaternion rotationDifference = newRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        if (angleInDegrees > 180) angleInDegrees -= 360;

        Vector3 angularVelocity = (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.fixedDeltaTime;
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
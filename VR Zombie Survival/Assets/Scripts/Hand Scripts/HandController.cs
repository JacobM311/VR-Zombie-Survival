using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public Quaternion rotationOffset;
    public float positionLerpSpeed = 0.1f;
    public float rotationLerpSpeed = 0.1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetPositionWithOffset;
        Quaternion targetRotationWithOffset;

        targetPositionWithOffset = target.TransformPoint(rotationOffset * Vector3.zero);
        targetRotationWithOffset = target.rotation * rotationOffset;

        Vector3 newPosition = Vector3.Lerp(transform.position, targetPositionWithOffset, positionLerpSpeed);
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime;

        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotationWithOffset, rotationLerpSpeed);
        Quaternion rotationDifference = newRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
        if (angleInDegree > 180) angleInDegree -= 360;

        Vector3 angularVelocity = (rotationAxis * angleInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
        rb.angularVelocity = angularVelocity;
    }
}
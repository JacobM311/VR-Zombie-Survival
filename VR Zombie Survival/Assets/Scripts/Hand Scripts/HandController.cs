using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public Quaternion rotationOffset;
    public float positionLerpSpeed = 0.1f;
    public float rotationLerpSpeed = 0.1f;

    void Update()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, target.position, positionLerpSpeed);
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime;

        // Update rotation with lag
        Quaternion targetRotWithOffset = target.rotation * rotationOffset;
        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotWithOffset, rotationLerpSpeed);
        Quaternion rotationDifference = newRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;
        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
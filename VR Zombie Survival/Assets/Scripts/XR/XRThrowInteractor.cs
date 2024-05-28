using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



namespace XRThrowInteractor
{
    public class XRThrowInteractor : MonoBehaviour
    {
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;
        private Rigidbody rb;
        private bool isThrown = false;

        public enum SpeedType
        {
            Constant,
            Dynamic
        }
        public SpeedType speedType = SpeedType.Dynamic;
        public float minVelocityThreshold = 0.1f; // Below this velocity, speed will be minimum
        public float maxVelocityThreshold = 15f;   // Above this velocity, speed will be maximum
        public float minSpeed = 0.1f;// Define the minimum and maximum speed values
        public float maxSpeed = 2f;

        public float speed;

        private List<Quaternion> recentRotations = new List<Quaternion>();
        private List<Vector3> recentPositions = new List<Vector3>();
        private int frameCount = 15;
        private bool rightHand;
        public bool ThrowingKnife;

        void Start()
        {
            rightHandInteractor.selectEntered.AddListener(OnGrabbed);
            leftHandInteractor.selectEntered.AddListener(OnGrabbed);
            rightHandInteractor.selectExited.AddListener(OnReleased);
            leftHandInteractor.selectExited.AddListener(OnReleased);
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (rightHand)
            {
                recentPositions.Add(rightHandInteractor.transform.position);
                recentRotations.Add(rightHandInteractor.transform.rotation);
            }
            else
            {
                recentPositions.Add(leftHandInteractor.transform.position);
                recentRotations.Add(leftHandInteractor.transform.rotation);
            }

            if (recentPositions.Count > frameCount)
            {
                recentPositions.RemoveAt(0);
                recentRotations.RemoveAt(0);
            }
        }

        private float CalculateSpeedBasedOnVelocity(float velocityMagnitude)
        {
            if (velocityMagnitude < minVelocityThreshold)
            {
                return minSpeed;
            }
            else if (velocityMagnitude > maxVelocityThreshold)
            {
                return maxSpeed;
            }
            else
            {
                // Linearly interpolate the speed value based on velocity magnitude
                float velocityRange = maxVelocityThreshold - minVelocityThreshold;
                float speedRange = maxSpeed - minSpeed;
                return minSpeed + (velocityMagnitude - minVelocityThreshold) / velocityRange * speedRange;
            }
        }

        private Vector3 CalculateAverageVelocity()
        {
            if (recentPositions.Count < 2)
                return Vector3.zero;

            // Calculate total distance moved over the last 15 frames
            Vector3 totalDelta15 = Vector3.zero;
            for (int i = 1; i < recentPositions.Count; i++)
            {
                totalDelta15 += recentPositions[i] - recentPositions[i - 1];
            }

            // Average distance moved per fixed frame for 15 frames
            Vector3 averageDelta15 = totalDelta15 / (recentPositions.Count - 1);
            Vector3 averageVelocity15 = averageDelta15 / Time.fixedDeltaTime;

            return averageVelocity15;
        }

        private Vector3 CalculateAverageAngularVelocity()
        {
            if (recentRotations.Count < 2)
                return Vector3.zero;

            Vector3 totalAngularDelta = Vector3.zero;
            for (int i = 1; i < recentRotations.Count; i++)
            {
                // Calculate the difference in rotation
                Quaternion deltaRotation = recentRotations[i] * Quaternion.Inverse(recentRotations[i - 1]);

                // Convert the delta rotation to angular velocity (in radians) and then to degrees if needed
                Vector3 angularDelta;
                float angle;
                deltaRotation.ToAngleAxis(out angle, out angularDelta);
                angularDelta = angularDelta.normalized * angle * Mathf.Deg2Rad; // Convert to radians

                // Accumulate total angular delta
                totalAngularDelta += angularDelta;
            }

            // Calculate average angular velocity
            Vector3 averageAngularVelocity = totalAngularDelta / (recentRotations.Count - 1) / Time.fixedDeltaTime;

            return averageAngularVelocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag != "Hand Model")
            {
                isThrown = false;
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform == this.transform)
            {
                recentPositions.Clear();
                recentRotations.Clear();
            }

            if (args.interactorObject == rightHandInteractor)
            {
                rightHand = true;
            }
            else
            {
                rightHand = false;
            }
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            if (args.interactableObject.transform == this.transform && GrabManager.Instance.GetGrabCount(this.gameObject) == 0)
            {
                Vector3 averageVelocity = CalculateAverageVelocity();
                Vector3 averageAngularVelocity = CalculateAverageAngularVelocity();


                if (speedType == SpeedType.Dynamic)
                {
                    float dynamicSpeed = CalculateSpeedBasedOnVelocity(averageVelocity.magnitude);
                    rb.velocity = averageVelocity * dynamicSpeed;
                }
                else
                {
                    rb.velocity = averageVelocity * speed;
                }

                rb.angularVelocity = averageAngularVelocity;
                isThrown = true;
                recentPositions.Clear();
                recentRotations.Clear();
            }
        }

        void FixedUpdate()
        {
            if (!ThrowingKnife) { return; }

            else if (rb.velocity.sqrMagnitude > .0001f && isThrown == true && GrabManager.Instance.GetGrabCount(this.gameObject) == 0)
            {
                Quaternion newRotation = Quaternion.LookRotation(rb.velocity);
                rb.rotation = Quaternion.Slerp(rb.rotation, newRotation, Time.fixedDeltaTime * 100);
            }
        }
    }

}


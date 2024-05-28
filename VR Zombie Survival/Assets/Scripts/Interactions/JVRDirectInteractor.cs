using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

[RequireComponent(typeof(SphereCollider))]
public class JVRDirectInteractor : MonoBehaviour
{
    public SphereCollider trigger;
    public InputActionProperty grabAction;

    public enum Handedness {
        Right,
        Left
    }
    public Handedness handedness;
    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("JVRGrabbable")) { return; }

        JVRGrabInteractable _interactable = other.GetComponent<JVRGrabInteractable>();

        _interactable.OnHovered(this);
    }
}

[CustomEditor(typeof(JVRDirectInteractor))]
public class JVRDirectInteractorEditor : Editor
{
    void OnEnable()
    {
        JVRDirectInteractor script = (JVRDirectInteractor)target;
        SphereCollider sphereCollider = script.GetComponent<SphereCollider>();
        if (sphereCollider != null && !sphereCollider.isTrigger)
        {
            sphereCollider.isTrigger = true;
        }

        script.trigger = script.GetComponent<SphereCollider>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}

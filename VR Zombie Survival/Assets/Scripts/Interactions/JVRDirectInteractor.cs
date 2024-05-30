using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

[RequireComponent(typeof(SphereCollider))]
public class JVRDirectInteractor : MonoBehaviour
{
    public enum Handedness
    {
        Right,
        Left
    }
    public Handedness handedness;
    public bool canDetectGrabbable = true;

    public SphereCollider trigger;
    public InputActionProperty grabAction;

    public UnityEvent<JVRGrabInteractable> OnHoverEvent;
    public UnityEvent<JVRGrabInteractable> OnSelectEnteringEvent;
    public UnityEvent<JVRGrabInteractable> OnSelectEnteredEvent;
    public UnityEvent<JVRGrabInteractable> OnPerformGrabEvent;

    // Start is called before the first frame update
    void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;

        if (OnHoverEvent == null)
            OnHoverEvent = new UnityEvent<JVRGrabInteractable>();

        if (OnSelectEnteringEvent == null)
            OnSelectEnteringEvent = new UnityEvent<JVRGrabInteractable>();

        if (OnSelectEnteredEvent == null)
            OnSelectEnteredEvent = new UnityEvent<JVRGrabInteractable>();

        if (OnPerformGrabEvent == null)
            OnPerformGrabEvent = new UnityEvent<JVRGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("JVRGrabbable")) { return; }

        JVRGrabInteractable _interactable = other.GetComponent<JVRGrabInteractable>();
        
        if (canDetectGrabbable)
        {
            _interactable.Hovered(this);
        }
    }

    public void TriggerHover(JVRGrabInteractable interactable)
    {
        OnHoverEvent?.Invoke(interactable);
    }

    public void TriggerSelectEntering(JVRGrabInteractable interactable)
    {
        OnSelectEnteringEvent?.Invoke(interactable);
    }

    public void TriggerSelectEntered(JVRGrabInteractable interactable)
    {
        OnSelectEnteredEvent?.Invoke(interactable);
    }

    public void TriggerPerformGrab(JVRGrabInteractable interactable)
    {
        OnPerformGrabEvent?.Invoke(interactable);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCustomGrabInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [SerializeField]
    private List<Transform> LeftHandAttachTransforms;
    [SerializeField]
    private List<Transform> RightHandAttachTransforms;
    [SerializeField]
    UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor LeftInteractor;
    [SerializeField]
    UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor RightInteractor;
    [SerializeField]
    GameObject LeftController;
    [SerializeField]
    GameObject RightController;

    private Transform m_OriginalAttachTransform;
    private GameObject objectToParentTo;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor check;
    private Rigidbody rb;
    private ConfigurableJoint joint;

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        m_OriginalAttachTransform = attachTransform;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        this.movementType = MovementType.Instantaneous;
        StartCoroutine(SetTypeToVelocity(.5f));
        if (args.interactorObject == LeftInteractor)
        {
            attachTransform = GetClosestTransform(LeftInteractor.transform.position, LeftHandAttachTransforms);
            attachTransform.GetComponent<TriggerAttachmentPointAnimation>()?.ActivateAnimation();
            objectToParentTo = LeftController;
            joint.connectedBody = LeftController.GetComponentInChildren<Rigidbody>();
        }
        if (args.interactorObject == RightInteractor)
        {
            attachTransform = GetClosestTransform(RightInteractor.transform.position, RightHandAttachTransforms);
            attachTransform.GetComponent<TriggerAttachmentPointAnimation>()?.ActivateAnimation();
            objectToParentTo = RightController;
            joint.connectedBody = RightController.GetComponentInChildren<Rigidbody>();
        }
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        joint.connectedBody = null;
        base.OnSelectExited(args);
        attachTransform.GetComponent<TriggerAttachmentPointAnimation>()?.DeactivateAnimation();
        this.transform.SetParent(null);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        check = args.interactorObject;
        base.OnSelectEntered(args);
        Debug.Log(check);
    } 

    protected override void Grab()
    {
        StartCoroutine(WaitToParent(.001f));
        base.Grab();
    }

    public IEnumerator SetTypeToVelocity(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        this.movementType = MovementType.VelocityTracking; ;
    }

    public IEnumerator WaitToParent(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        this.transform.SetParent(objectToParentTo.transform);
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

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
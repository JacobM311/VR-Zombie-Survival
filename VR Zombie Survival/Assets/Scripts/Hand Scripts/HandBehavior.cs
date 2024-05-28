using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandBehavior : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor directInteractor;
    public List<GameObject> handColliders = new List<GameObject>();
    private GameObject interactableObject;
    private ConfigurableJoint joint;
    private HandController handController;
    private Rigidbody rb;

    private void Start()
    {
        joint = GetComponentInChildren<ConfigurableJoint>();
        handController = GetComponentInChildren<HandController>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    private void OnEnable()
    {
        directInteractor.selectEntered.AddListener(OnGrabbed);
        directInteractor.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        interactableObject = args.interactableObject.transform.gameObject;

        joint.connectedBody = interactableObject.GetComponentInParent<Rigidbody>();
        Debug.Log(joint.connectedBody);

        //StartCoroutine(WaitToEnableJoint(.16f));

        //handController.enabled = false;
        //rb.useGravity = false;

        //joint.xMotion = ConfigurableJointMotion.Limited;
        //joint.yMotion = ConfigurableJointMotion.Limited;
        //joint.zMotion = ConfigurableJointMotion.Limited;
        //joint.angularXMotion = ConfigurableJointMotion.Locked;
        //joint.angularYMotion = ConfigurableJointMotion.Locked;
        //joint.angularZMotion = ConfigurableJointMotion.Locked;

        if (GrabManager.Instance.GetGrabCount(interactableObject) == 0)
        {
            SetLayerRecursively(interactableObject, LayerMask.NameToLayer("Grabbing"));
        }
        GrabManager.Instance.IncrementGrab(interactableObject);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        GrabManager.Instance.DecrementGrab(interactableObject);

        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        //joint.angularXMotion = ConfigurableJointMotion.Free;
        //joint.angularYMotion = ConfigurableJointMotion.Free;
        //joint.angularZMotion = ConfigurableJointMotion.Free;


        joint.connectedBody = null;

        if (GrabManager.Instance.GetGrabCount(interactableObject) == 0)
        {
            StartCoroutine(WaitToEnable(.4f));
        }
    }

    public IEnumerator WaitToEnableJoint(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        joint.zMotion = ConfigurableJointMotion.Limited;
    }

    public IEnumerator WaitToEnable(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (GrabManager.Instance.GetGrabCount(interactableObject) == 0)
        {
            SetLayerRecursively(interactableObject, LayerMask.NameToLayer("Default"));
        }
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

    private void OnDisable()
    {
        directInteractor.selectEntered.RemoveListener(OnGrabbed);
        directInteractor.selectExited.RemoveListener(OnReleased);
    }
}
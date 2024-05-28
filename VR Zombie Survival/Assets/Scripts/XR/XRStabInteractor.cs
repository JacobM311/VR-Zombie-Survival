using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRStabInteractor : MonoBehaviour
{
    public Collider bladeCollider;
    public Collider handleCollider;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;
    private ParticleSystem blood;
    private AudioSource audioSource;
    public List<AudioClip> woodHitClips;

    private bool shouldBeStabbing;
    private bool isGrabbed = true;

    private GameObject targetObject;
    private Rigidbody rb;
    private XRThrowInteractor.XRThrowInteractor throwInteractor;
    private LayerMask StabLayerMask;
    public bool isRecentlyThrown;
    public UIController ui;
    Quaternion originalRotation;
    Vector3 originalPosition;


    void Start()
    {
        rightHandInteractor.selectEntered.AddListener(OnGrabbed);
        leftHandInteractor.selectEntered.AddListener(OnGrabbed);
        rightHandInteractor.selectExited.AddListener(OnReleased);
        leftHandInteractor.selectExited.AddListener(OnReleased);

        StabLayerMask = LayerMask.GetMask("StabLayerMask");
        throwInteractor = GetComponent<XRThrowInteractor.XRThrowInteractor>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        blood = GetComponentInChildren<ParticleSystem>();
        blood.Stop();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        ui.numberOfKnives += 1;
        ui.knivesToBeReset.Add(this.gameObject);
    }

    void Update()
    {
        if (!isGrabbed)
        {
            PerformRaycastCheck();
        }
    }


    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform == this.transform)
        {
            rb.isKinematic = false;
            bladeCollider.isTrigger = false;
            handleCollider.isTrigger = false;
            isGrabbed = true;
            transform.parent = null;
            targetObject = null;
            shouldBeStabbing = true;
            blood.Stop();
        }
    }
    private void OnReleased(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform == this.transform)
        {
            isRecentlyThrown = true;
            StartCoroutine(StartThrowResetTimer(3));
            isGrabbed = false;
            rb.isKinematic = false;
        }
    }

    private IEnumerator StartThrowResetTimer(int time)
    {
        yield return new WaitForSeconds(time);
        isRecentlyThrown = false;
    }


    private void PerformRaycastCheck()
    {
        if (isGrabbed) return;
        if (!shouldBeStabbing) return;

        float rayLength = .3f;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, StabLayerMask))
        {
            if (hit.collider != bladeCollider && !hit.collider.isTrigger)
            {
                bladeCollider.isTrigger = true;
                
                targetObject = hit.collider.gameObject;
                transform.position = hit.point;
                shouldBeStabbing = true;

                if (rb.velocity.magnitude > .3 && shouldBeStabbing)
                {
                    Stab(targetObject);
                }
                
            }
        }
    }

    private void Stab(GameObject obj)
    {
        if (obj.tag == "Enemy")
        {
            AIEnemy enemy = obj.GetComponentInParent<AIEnemy>();

            transform.parent = obj.transform;
            enemy.HitCount();
        }

        if (obj.tag == "Head")
        {
            AIEnemy enemy = obj.GetComponentInParent<AIEnemy>();

            transform.parent = obj.transform;
            enemy.HeadShot();
        }

        if (obj.tag == "Wood")
        {
            audioSource.clip = woodHitClips[Random.Range(0, woodHitClips.Count)];
            audioSource.PlayOneShot(audioSource.clip);
        }
        this.rb.isKinematic = true;
        shouldBeStabbing = false;
    }

    public void ResetPosition()
    {

        
            transform.parent = null;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            rb.isKinematic = false;
            bladeCollider.isTrigger = false;
            handleCollider.isTrigger = false;
            blood.Stop();

            ui.ReportResetPosition();
            Debug.Log("reported");
        
    }
}

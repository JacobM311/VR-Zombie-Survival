using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyMele : MonoBehaviour
{
    private Animator animator;
    private bool hitRegistered;
    private PlayerController playerController;
    private GameObject xrRig;

    void Start()
    {
        xrRig = GameObject.Find("XR Origin");
        playerController = xrRig.GetComponent<PlayerController>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            hitRegistered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }

        // Check if the current state is an attack
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !hitRegistered)
        {
            playerController.Hit();
            hitRegistered = true;
        }
    }
}

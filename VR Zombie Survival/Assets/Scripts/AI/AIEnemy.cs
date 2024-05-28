using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;

public class AIEnemy : MonoBehaviour
{
    private List<Rigidbody> m_Rigidbodies = new List<Rigidbody>();
    private NavMeshAgent agent;
    private Animator animator;
    private bool ragDoll;
    private bool canAttack = true;
    public AudioSource audioSource;
    public List<AudioClip> idleAudio;
    public List<AudioClip> deathAudio;
    private GameObject billBoard;
    private UIController ui;
    private Transform player;
    public int hitCount;
    public float moveSpeed = .5f;

    [SerializeField]
    private GameObject xrRig;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        m_Rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        animator = GetComponent<Animator>();
        InvokeRepeating("PlayRandomIdleAudio", Random.Range(5f,8f), Random.Range(5f, 8f));
        xrRig = GameObject.Find("XR Origin");
        billBoard = GameObject.Find("Billboard UI");
        player = xrRig.transform;
        ui = billBoard.GetComponent<UIController>();
    }

    void Update()
    {
        if (!ragDoll)
        {
            agent.SetDestination(player.position);
        }

        if (hitCount >= 3 && !ragDoll)
        {
            ActivateRagdoll();
        }

        float distance = Vector3.Distance(agent.transform.position, player.position);

        if (distance < 3f && canAttack)
        {
            PlayRandomRangeAttackAnimation();
            canAttack = false;
        }
    }

    private void ActivateRagdoll()
    {
        ui.IncrementKills();
        ui.CheckBeatRound();
        ui.enemiesToBeDestroyed.Add(this.gameObject);
        agent.enabled = false;
        CancelInvoke("PlayRandomIdleAudio");
        ragDoll = true;
        PlayRandomDeathAudio();

        foreach (Rigidbody rb in m_Rigidbodies)
        {
            rb.isKinematic = false;

            Collider collider = rb.GetComponent<Collider>();

            if (collider.isTrigger == true)
            {
                collider.enabled = false;
            }
        }

        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void HitCount()
    {
        hitCount++;
    }

    public void HeadShot()
    {
        hitCount = 3;
    }

    private void PlayRandomRangeAttackAnimation()
    {
        List<string> list = new List<string> { "Play2", "Play3", "Play4", "Play5", "Play6", "Play7", };
        animator.SetTrigger(list[Random.Range(0, list.Count)]);
        StartCoroutine(AdjustAgentSpeedDuringAnimation(4.5f, 1f, 1f));
    }

    private void PlayRandomCloseAttackAnimation()
    {
        agent.speed = 0;
        List<string> list = new List<string> { "Play", "Play1", "Play8", "Play9" };
        animator.SetTrigger(list[Random.Range(0, list.Count)]);
        StartCoroutine(AdjustAgentSpeedDuringAnimation(6f, 3f, 1f));
    }

    private IEnumerator AdjustAgentSpeedDuringAnimation(float targetSpeed, float accelerationFactor, float decelerationFactor)
    {
        float originalSpeed = agent.speed;

        float currentLerpTime = 0f;
        while (agent.speed < targetSpeed)
        {
            currentLerpTime += Time.deltaTime * accelerationFactor;
            agent.speed = Mathf.Lerp(originalSpeed, targetSpeed, currentLerpTime);
            yield return null;
        }
        agent.speed = 4f;

        currentLerpTime = 0f;
        while (agent.speed > originalSpeed)
        {
            currentLerpTime += Time.deltaTime * decelerationFactor;
            agent.speed = Mathf.Lerp(targetSpeed, originalSpeed, currentLerpTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        agent.speed = originalSpeed;
        canAttack = true;
    }

    private void PlayRandomDeathAudio()
    {
        audioSource.clip = deathAudio[Random.Range(0, deathAudio.Count)];
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void PlayRandomIdleAudio()
    {
        audioSource.clip = idleAudio[Random.Range(0, deathAudio.Count)];
        audioSource.PlayOneShot(audioSource.clip);
    }
}

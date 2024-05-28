using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private int health = 10;
    private bool hasDied = false;
    private AudioSource audioSource;
    public AudioClip Wilhelm;
    public GameObject fadeToBlackObject;
    private Material fadeToBlackMaterial;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Renderer cubeRenderer = fadeToBlackObject.GetComponent<Renderer>();
        fadeToBlackMaterial = cubeRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FadeToBlack()
    {
        hasDied = true;
        float Timer = 0;
        while (Timer < 1)
        {
            Timer += Time.deltaTime / 3;
            fadeToBlackMaterial.SetFloat("_Alpha", Timer);
            yield return null;
        }
        fadeToBlackMaterial.SetFloat("_Alpha", 1);
    }

    public void Hit()
    {
        if (health > 0 && !hasDied)
        {
            health -= 2;
            Debug.Log(health);
        }
        else
        {
            StartCoroutine(FadeToBlack());
        }
    }
}

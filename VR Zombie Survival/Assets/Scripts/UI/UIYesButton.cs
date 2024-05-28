using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class UIYesButton : MonoBehaviour
{
    public UIController uiController;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;
    public List<GameObject> gridSpawnList;
    private bool isGrabbed;
    public PlayerSettings playerSettings;
    public GameObject noButton;
    private Collider trigger;
    private TMP_Text text;

    void Start()
    {
        rightHandInteractor.selectEntered.AddListener(OnGrabbed);
        leftHandInteractor.selectEntered.AddListener(OnGrabbed);
        rightHandInteractor.selectExited.AddListener(OnReleased);
        leftHandInteractor.selectExited.AddListener(OnReleased);
        trigger = GetComponent<Collider>();
        text = GetComponentInChildren<TMP_Text>();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Knife") { return; }

        XRStabInteractor stabIntractor = other.GetComponent<XRStabInteractor>();

        if (stabIntractor.isRecentlyThrown)
        {
            if (!isGrabbed)
            {
                trigger.enabled = false;
                text.text = "";
                noButton.SetActive(false);
                StartCoroutine(StartRound());
            }
        }
    }

    private IEnumerator StartRound()
    {
        int countDownTime = 3;

        while (countDownTime > 0)
        {
            uiController.SetUIText(countDownTime.ToString());
            yield return new WaitForSeconds(1);

            countDownTime--;
        }
        
        uiController.SetUIText("GO");

        foreach (GameObject knife in uiController.knivesToBeReset)
        {
            XRStabInteractor stabInteractor = knife.GetComponent<XRStabInteractor>(); 
            stabInteractor.ResetPosition();
        }

        foreach (GameObject enemy in uiController.enemiesToBeDestroyed)
        {
            Destroy(enemy);
        }

        foreach (GameObject grid in gridSpawnList)
        {
            GridSpawn gridSpawn = grid.GetComponent<GridSpawn>();
            gridSpawn.SpawnEnemies();
        }


        uiController.startRound = true;
        trigger.enabled = true;
        text.text = "Yes";
        gameObject.SetActive(false);
    }
}

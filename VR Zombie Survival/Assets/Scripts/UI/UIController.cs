using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_Text killsText;
    public TMP_Text rounds;
    public GameObject yesButton;
    public GameObject noButton;
    public int kills;
    private int round;
    private bool fadeToBlack;
    public PlayerSettings playerSettings;
    public bool hasDied;
    public List<GameObject> tutorialObjects;
    public List<GameObject> enemiesToBeDestroyed;
    public List<GameObject> knivesToBeReset;
    public int enemyCount;
    public bool startRound;
    private int spawns;
    public int numberOfSpawners;
    private int knifeResets;
    public int numberOfKnives;
    public bool knivesReset;
    public bool spawnsComplete;

    private void Start()
    {
        if (playerSettings.passedTutorial == true)
        {
            PassedTutorial(0);
        }

        if (playerSettings.passedTutorial == false)
        {
            if (playerSettings.deaths == 1) SetUIText("Throw the knives at the zombie.");

            if (playerSettings.deaths == 2) SetUIText("The knives, on the table, you throw them...");

            if (playerSettings.deaths >= 3) SetUIText("Just kill the zombie.");
        }
    }

    private void Update()
    {
        if (spawnsComplete && knivesReset)
        {
            spawns = 0;
            knifeResets = 0;
            startRound = false;
        }
    }

    public void PassedTutorial(float delay = 0f)
    {
        if (delay > 0)
        {
            StartCoroutine(PassedTutorialWithDelay(delay));
        }
        else
        {
            ApplyPassedTutorial();
        }
    }

    private IEnumerator PassedTutorialWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyPassedTutorial();
    }

    private void ApplyPassedTutorial()
    {
        foreach (GameObject obj in tutorialObjects)
        {
            Destroy(obj);
        }

        SetUIText("Ready?");
        noButton.SetActive(true);
        yesButton.SetActive(true);
    }

    public void SetUIText(string str)
    {
        messageText.text = str;
    }

    public void IncrementDeaths()
    { playerSettings.deaths += 1; }

    public void IncrementKills()
    {
        kills++;
        killsText.text = kills.ToString();

        if (kills == 1 && !playerSettings.passedTutorial)
        {
            playerSettings.passedTutorial = true;
            kills = 0;
            killsText.text = kills.ToString();
            PassedTutorial(5);
        }
    }

    public void IncrementRound()
    {
        round++;
        rounds.text = round.ToString();

        messageText.text = "Ready?";

        noButton.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(true);
    }

    public void CheckBeatRound()
    {
        if (kills == enemyCount)
        {
            Debug.Log("round");
            IncrementRound();
        }
    }

    public void ReportSpawnComplete()
    {
        spawns++;
        if (spawns == numberOfSpawners)
        {
            spawnsComplete = true;
        }
    }

    public void ReportResetPosition()
    {
        knifeResets++;
        if (knifeResets == numberOfKnives)
        {
            knivesReset = true;
        }
    }
}

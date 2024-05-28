using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UINoButton : MonoBehaviour
{
    private List<string> initialPhrases = new List<string> { "Well, let me know when you're ready.", "Or don't.", "I'll just wait here then...", "No rush, I've got all day.", "Maybe later then?", "Or maybe never?", "Cool, cool, cool, cool, no doubt, no doubt.", "Totally cool.", "Oh, ok then... I wasn't ready either.", "We can just sit in silence, I guess.", "That's fine. I didn't want to anyway.", "No worries, I'm sure you have your reasons.", "Press 'no' again if you're just testing buttons.", "It still works.", "Ah, a casual 'no' enjoyer, I see.", "A bold choice, indeed."};
    public UIController uiController;
    private int i = 0;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftHandInteractor;
    private bool isGrabbed;

    void Start()
    {
        rightHandInteractor.selectEntered.AddListener(OnGrabbed);
        leftHandInteractor.selectEntered.AddListener(OnGrabbed);
        rightHandInteractor.selectExited.AddListener(OnReleased);
        leftHandInteractor.selectExited.AddListener(OnReleased);
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
        if (other.gameObject.tag != "Weapon") { return; }

        XRStabInteractor stabIntractor = other.GetComponent<XRStabInteractor>();

        if (stabIntractor.isRecentlyThrown)
        {
            if (!isGrabbed)
            {
                uiController.SetUIText(initialPhrases[i]);
                i = (i + 1) % initialPhrases.Count;
            }
        }
    }
}

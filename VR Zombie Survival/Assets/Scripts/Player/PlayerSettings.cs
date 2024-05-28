using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettingsObject")]
public class PlayerSettings : ScriptableObject
{
    public int deaths;

    public bool passedTutorial;

    public bool startRound;
}
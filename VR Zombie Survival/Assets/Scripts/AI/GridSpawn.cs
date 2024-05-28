using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSpawn : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int rows = 10;
    public int columns = 10;
    public PlayerSettings playerSettings;
    public UIController ui;
    public UIYesButton yesButton;
    public bool hasSpawned;

    void Start()
    {
        ui.numberOfSpawners += 1;
        yesButton.gridSpawnList.Add(this.gameObject);
    }

    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        Vector3 size = GetComponent<Renderer>().bounds.size;
        Vector3 startPosition = transform.position - size / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 spawnPosition = new Vector3(startPosition.x + (size.x / rows) * i, startPosition.y, startPosition.z + (size.z / columns) * j);

                float coinFlip = Random.Range(0, 1);

                GameObject enemy = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                ui.enemyCount++;
            }
        }

        ui.ReportSpawnComplete();
    }
}
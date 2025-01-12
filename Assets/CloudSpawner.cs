using System.Collections.Generic;

using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public List<GameObject> cloudPrefabs; // List of cloud prefabs
    public RectTransform canvasTransform; // Reference to the Canvas for spawning
    public float spawnInterval = 2f; // Time between spawns
    public float spawnYRange = 230f; // Range for random Y positions (adjust as needed)
    private bool isStartSpwaning = false;
    private List<GameObject> clouds = new List<GameObject>();
    private void Start()
    {
        //Spwan();
    }

    public void Spwan()
    {
        isStartSpwaning = true;
        // Start spawning clouds at regular intervals
        InvokeRepeating(nameof(SpawnCloud), 1f, spawnInterval);
    }

    private void SpawnCloud()
    {
        if (cloudPrefabs.Count == 0)
        {
            Debug.LogWarning("No cloud prefabs assigned to the spawner!");
            return;
        }

        if (!isStartSpwaning)
            return;

        // Randomly select a cloud prefab from the list
        int randomIndex = Random.Range(0, cloudPrefabs.Count);
        GameObject selectedCloud = cloudPrefabs[randomIndex];

        // Instantiate the cloud at a random Y position
        float randomY = Random.Range(0, spawnYRange);
        GameObject newCloud = Instantiate(selectedCloud, canvasTransform);
        clouds.Add(newCloud);

        // Set the position of the cloud (off-screen to the right)
        RectTransform cloudRect = newCloud.GetComponent<RectTransform>();
        cloudRect.anchoredPosition = new Vector2(800f, randomY); // Adjust X position as needed
    }

    public void ClearClouds()
    {
        isStartSpwaning = false;
        for (int i = 0; i < clouds.Count; ++i)
        {
            Destroy(clouds[i]);
        }
        clouds.Clear();
    }
}

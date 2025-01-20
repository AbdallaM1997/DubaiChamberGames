using System.Collections.Generic;

using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public List<GameObject> cloudPrefabs; // List of cloud prefabs
    public RectTransform canvasTransform; // Reference to the Canvas for spawning
    public float spawnInterval = 2f; // Time between spawns
    private bool isStartSpawning = false;
    private List<GameObject> clouds = new List<GameObject>();

    [System.Serializable]
    public class CloudData
    {
        public int prefabIndex; // Index of the prefab in the cloudPrefabs list
        public float yPosition; // Y position of the cloud
    }

    public List<CloudData> predefinedClouds; // Predefined list of clouds with prefab indices and positions
    private int currentCloudIndex = 0; // Index of the current cloud to spawn

    private void Start()
    {
        // Call Spwan() when you want to start spawning
    }

    public void Spwan()
    {
        isStartSpawning = true;
        currentCloudIndex = 0; // Reset the index to start from the beginning
        InvokeRepeating(nameof(SpawnCloud), 1f, spawnInterval);
    }

    private void SpawnCloud()
    {
        if (cloudPrefabs.Count == 0 || predefinedClouds.Count == 0)
        {
            Debug.LogWarning("No cloud prefabs or predefined clouds assigned to the spawner!");
            CancelInvoke(nameof(SpawnCloud));
            return;
        }

        if (!isStartSpawning || currentCloudIndex >= predefinedClouds.Count)
        {
            CancelInvoke(nameof(SpawnCloud)); // Stop spawning when all clouds are spawned
            return;
        }

        // Get the predefined cloud data
        CloudData cloudData = predefinedClouds[currentCloudIndex];

        // Validate prefab index
        if (cloudData.prefabIndex < 0 || cloudData.prefabIndex >= cloudPrefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index {cloudData.prefabIndex} in predefined clouds!");
            currentCloudIndex++;
            return;
        }

        // Instantiate the cloud
        GameObject selectedCloud = cloudPrefabs[cloudData.prefabIndex];
        GameObject newCloud = Instantiate(selectedCloud, canvasTransform);
        clouds.Add(newCloud);

        // Set the position of the cloud
        RectTransform cloudRect = newCloud.GetComponent<RectTransform>();
        cloudRect.anchoredPosition = new Vector2(800f, cloudData.yPosition); // Adjust X position as needed

        currentCloudIndex++; // Move to the next cloud
    }

    public void ClearClouds()
    {
        isStartSpawning = false;
        for (int i = 0; i < clouds.Count; ++i)
        {
            Destroy(clouds[i]);
        }
        clouds.Clear();
    }
}

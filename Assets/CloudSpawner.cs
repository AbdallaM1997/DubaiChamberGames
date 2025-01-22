using System.Collections.Generic;

using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public List<GameObject> cloudPrefabs;
    public RectTransform canvasTransform;
    public float spawnInterval = 2f;
    private bool isStartSpawning = false;
    private List<GameObject> clouds = new List<GameObject>();

    [System.Serializable]
    public class CloudData
    {
        public int prefabIndex;
        public float yPosition;
    }

    public List<CloudData> predefinedClouds;
    private int currentCloudIndex = 0;

    private void Start()
    {
    }

    public void StartSpawning()
    {
        isStartSpawning = true;
        currentCloudIndex = 0;

        if (!IsInvoking(nameof(SpawnCloud)))
        {
            SpawnCloud();
            InvokeRepeating(nameof(SpawnCloud), spawnInterval, spawnInterval);
        }
    }

    public void StopSpawning()
    {
        isStartSpawning = false;
        CancelInvoke(nameof(SpawnCloud));
    }

    private void SpawnCloud()
    {
        if (cloudPrefabs.Count == 0 || predefinedClouds.Count == 0)
        {
            Debug.LogWarning("No cloud prefabs or predefined clouds assigned to the spawner!");
            StopSpawning();
            return;
        }

        if (!isStartSpawning) return;

        if (currentCloudIndex < predefinedClouds.Count)
        {
            SpawnCloudOnce();
        }
        else if (AllCloudsOffScreen())
        {
            currentCloudIndex = 0;
            clouds.Clear();
        }
    }

    private bool AllCloudsOffScreen()
    {
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            if (clouds[i] != null) return false;
        }
        return true;
    }

    private void SpawnCloudOnce()
    {
        CloudData cloudData = predefinedClouds[currentCloudIndex];

        if (cloudData.prefabIndex < 0 || cloudData.prefabIndex >= cloudPrefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index {cloudData.prefabIndex} in predefined clouds!");
            currentCloudIndex++;
            return;
        }

        GameObject selectedCloud = cloudPrefabs[cloudData.prefabIndex];
        GameObject newCloud = Instantiate(selectedCloud, canvasTransform);
        clouds.Add(newCloud);

        RectTransform cloudRect = newCloud.GetComponent<RectTransform>();
        cloudRect.anchoredPosition = new Vector2(250f, cloudData.yPosition);

        currentCloudIndex++;
    }

    public void ClearClouds()
    {
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            if (clouds[i] != null)
            {
                Destroy(clouds[i]);
            }
        }
        clouds.Clear();
    }
}

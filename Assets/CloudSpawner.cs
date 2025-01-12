using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab; // Cloud prefab
    public RectTransform canvasTransform; // Reference to the canvas
    public float spawnRate = 2f; // Time between spawns

    void Start()
    {
        InvokeRepeating(nameof(SpawnCloud), 1f, spawnRate);
    }

    void SpawnCloud()
    {
        // Spawn cloud at a random Y position
        float yPosition = Random.Range(0, 300f); // Adjust bounds as needed
        GameObject cloud = Instantiate(cloudPrefab, canvasTransform);
        cloud.GetComponent<RectTransform>().anchoredPosition = new Vector2(800f, yPosition); // Spawn off-screen to the right
    }
}

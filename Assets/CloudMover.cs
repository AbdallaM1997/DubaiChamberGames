using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public RectTransform cloud; // Cloud UI element
    public float speed = 150f; // Cloud movement speed

    void Update()
    {
        // Move the cloud left
        cloud.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Destroy the cloud if it moves out of bounds
        if (cloud.anchoredPosition.x < -852f) // Adjust based on canvas size
        {
            Destroy(gameObject);
        }
    }   
}

using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public RectTransform plane; // The UI Image representing the plane
    public float speed = 200f; // Movement speed
    public float upperBound = 400f; // Upper Y boundary in UI space
    public float lowerBound = -400f; // Lower Y boundary in UI space

    private bool moveUp = false; // Flag to move up
    private bool moveDown = false; // Flag to move down

    void Update()
    {
        if (moveUp)
        {
            MovePlane(Vector2.up);
        }
        if (moveDown)
        {
            MovePlane(Vector2.down);
        }
    }

    public void StartMovingUp()
    {
        moveUp = true;
    }

    public void StopMovingUp()
    {
        moveUp = false;
    }

    public void StartMovingDown()
    {
        moveDown = true;
    }

    public void StopMovingDown()
    {
        moveDown = false;
    }

    private void MovePlane(Vector2 direction)
    {
        // Move the plane in the specified direction
        plane.anchoredPosition += direction * speed * Time.deltaTime;

        // Clamp the plane's position within boundaries
        Vector2 pos = plane.anchoredPosition;
        pos.y = Mathf.Clamp(pos.y, lowerBound, upperBound);
        plane.anchoredPosition = pos;
    }
}

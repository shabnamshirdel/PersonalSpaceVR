using UnityEngine;

// Moves the virtual person in a straight line at a constant speed.
// Attach this to your walker object (a capsule or a humanoid model).
public class Walker : MonoBehaviour
{
    public bool isWalking = false;
    Vector3 direction;
    float speed;

    // Put the walker at a start position, facing its walking direction.
    public void Begin(Vector3 startPosition, Vector3 walkDirection, float walkSpeed)
    {
        transform.position = startPosition;
        direction = walkDirection.normalized;
        speed = walkSpeed;
        transform.rotation = Quaternion.LookRotation(direction);
        gameObject.SetActive(true);
        isWalking = true;
    }

    public void Stop()
    {
        isWalking = false;
    }

    public void Hide()
    {
        isWalking = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isWalking)
            transform.position += direction * speed * Time.deltaTime;
    }
}

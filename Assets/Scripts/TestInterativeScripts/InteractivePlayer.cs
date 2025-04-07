using UnityEngine;

public class InteractivePlayer : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 m_position;

    // Update is called once per frame
    void Update()
    {
        m_position.x = Input.GetAxisRaw("Horizontal");
        m_position.y = Input.GetAxisRaw("Vertical");
        
        m_position.Normalize();

        rb.linearVelocity = m_position * moveSpeed;
    }
}

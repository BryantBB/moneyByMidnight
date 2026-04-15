using UnityEngine;

public class HorizontalScroller : MonoBehaviour
{
    public float speed = 5f;
    public bool moveRight = true;

    private float spriteWidth;

    void Start()
    {
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x; 
    }

    void Update()
    {
        float direction = moveRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
        if (moveRight && transform.position.x >= spriteWidth)
            transform.position -= new Vector3(spriteWidth * 2f, 0, 0);
        else if (!moveRight && transform.position.x <= -spriteWidth)
            transform.position += new Vector3(spriteWidth * 2f, 0, 0);
    }
}
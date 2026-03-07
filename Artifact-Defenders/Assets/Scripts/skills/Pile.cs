using UnityEngine;

public class Pile : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Collider2D col;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void SetPreview(bool valid)
    {
        if (sprite != null)
        {
            sprite.color = valid ?
                new Color(0, 1, 0, 0.7f) :
                new Color(1, 0, 0, 0.7f);
        }

        if (col != null)
            col.enabled = false;
    }

    public void Place()
    {
        if (sprite != null)
            sprite.color = Color.white;

        if (col != null)
            col.enabled = true;

        int layer = LayerMask.NameToLayer("Pile");

        if (layer >= 0)
            gameObject.layer = layer;
    }
}
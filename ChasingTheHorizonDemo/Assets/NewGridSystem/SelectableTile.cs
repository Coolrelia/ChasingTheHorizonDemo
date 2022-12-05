using UnityEngine;

public class SelectableTile : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMap map;
    public new BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
}

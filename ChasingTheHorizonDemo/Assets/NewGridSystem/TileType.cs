using UnityEngine;

[System.Serializable]
public class TileType
{
    public string name;
    public GameObject tileVisualPrefab;
    public bool isChest;
    public Item storedItem;

    public float movementCost = 1;
    public bool isWalkable = true;
}

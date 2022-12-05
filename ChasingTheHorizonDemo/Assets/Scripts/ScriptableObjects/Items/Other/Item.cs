using UnityEngine;

//The base item class, defines the properties of a basic item 
public enum ItemType
{
    Consumable,
    Weapon,
    Key,
}

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Header("Item Attributes")]
    public string itemName = null;
    public Sprite itemIcon = null;
    public string itemDescription = null;
    public ItemType type = ItemType.Consumable;
}

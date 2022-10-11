using UnityEngine;

//A Weapon scriptable object, some weapons like magic may have a secondary animation which can be optionally attached
[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    public enum WeaponType { Sword, Spear, Bow, Axe}
    [Header("Weapon Attributes")]
    public int might = 0;
    public int hit = 0;
    public int crit = 0;
    public int range = 0;
    public int weight = 0;
    public int durability = 0;
    public int rank = 0;
    public WeaponType weaponType;

    public GameObject animation = null;
    public AnimationClip animationLength = null;
}

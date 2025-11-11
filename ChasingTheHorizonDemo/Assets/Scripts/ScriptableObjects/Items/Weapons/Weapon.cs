using UnityEngine;

//A Weapon scriptable object, some weapons like magic may have a secondary animation which can be optionally attached
[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Attributes")]
    public int might = 0;
    public int hit = 0;
    public int crit = 0;
    public int range = 0;
    public int weight = 0;
    public bool magical = false;

    public GameObject animation = null;
    public AnimationClip animationLength = null;
}

using UnityEngine;
using UnityEngine.UI;

//A data container for the Units Menu
//Used in conjuction with the Units Menu
public class UnitSlot : MonoBehaviour
{
    public UnitLoader slotUnit = null;

    [SerializeField] private Image sprite = null;
    [SerializeField] private Text unitName = null;
    [SerializeField] private Text unitWeapon = null;
    [SerializeField] private Text unitLvl = null;
    [SerializeField] private Text unitEXP = null;
    [SerializeField] private Text unitHealth = null;

    public void FillSlot(UnitLoader unit)
    {
        slotUnit = unit;

        sprite.sprite = unit.unit.sprite;
        unitName.text = unit.unit.unitName;
        unitWeapon.text = unit.equippedWeapon.name;
        unitLvl.text = unit.level.ToString();
        unitEXP.text = unit.exp.ToString();
        unitHealth.text = unit.currentHealth.ToString() + " / " + unit.unitHP.ToString();
    }

    private void OnDisable()
    {
        slotUnit = null;

        sprite.sprite = null;
        unitName.text = null;
        unitWeapon.text = null;
        unitLvl.text = null;
        unitEXP.text = null;
        unitHealth.text = null;

        Destroy(gameObject);
    }
}

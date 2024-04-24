using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellMenuSlot : MonoBehaviour
{
    public Spell spell = null;
    private Button slotButton = null;
    private Image slotIcon = null;

    private void Awake()
    {
        if (GetComponent<Button>())
        {
            slotButton = GetComponent<Button>();
        }
        slotIcon = gameObject.transform.GetChild(0).GetComponent<Image>();
    }

    public void FillSlot()
    {
        slotIcon.sprite = spell.spellIcon;
        slotIcon.color = new Color32(255, 255, 255, 255);
    }
    public void ClearSlot()
    {
        if (spell)
        {
            spell = null;
        }
        if (slotIcon)
        {
            slotIcon.sprite = null;
            slotIcon.color = new Color32(255, 255, 255, 0);
        }
    }
}

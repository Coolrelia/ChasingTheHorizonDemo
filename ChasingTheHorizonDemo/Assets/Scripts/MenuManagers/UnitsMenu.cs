using UnityEngine;

//UnitsMenu displays the information of all AllyUnits in the scene
//Works in conjuction with the UnitSlot script
//When enabled, loops through all AllyUnits and fills each UnitSlot with the appropriate data
//The UI currently only has room for 4 ally units to be displayed at once, functionality for multiple pages will need to be added in the future
public class UnitsMenu : MonoBehaviour
{
    //VARIABLES
    private int currentSlot = 1;
    private int slot1 = 125;
    private int slot2 = 25;
    private int slot3 = -75;
    private int slot4 = -175;

    //REFERENCES
    [SerializeField] private UnitSlot unitSlot = null;
    private UnitSlot selectedSlot = null;
    public UnitRuntimeList allyUnits;
    public UnitRuntimeList enemyUnits;


    private void OnEnable()
    {
        SoundManager.instance.PlayFX(11);
        Invoke("SetSlots", 0.17f);
    }
    private void OnDisable()
    {
        currentSlot = 1;
    }


    private void SetSlots()
    {
        for (int i = 0; i < allyUnits.objects.Count; i++)
        {
            selectedSlot = Instantiate(unitSlot, transform);
            SlotPosition();
            selectedSlot.FillSlot(allyUnits.objects[i]);
        }
    }
    private void SlotPosition()
    {
        switch (currentSlot)
        {
            case 1:
                selectedSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, slot1);
                currentSlot++;
                break;
            case 2:
                selectedSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, slot2);
                currentSlot++;
                break;
            case 3:
                selectedSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, slot3);
                currentSlot++;
                break;
            case 4:
                selectedSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, slot4);
                currentSlot++;
                break;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//Handles the Main Menu of Map Scenes
//Additional functionality can be added
//Options Menu coming soon

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button primaryButton = null;
    [SerializeField] private GameObject unitsMenu = null;
    [SerializeField] private GameObject optionsMenu = null;
    [SerializeField] private CursorController cursor = null;

    private void OnEnable()
    {
        StartCoroutine(HighlightButton());
    }

    private void OnDisable()
    {
        unitsMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void UnitMenu()
    {
        if(!unitsMenu.activeSelf){
            unitsMenu.SetActive(true);
        }
        else{
            unitsMenu.SetActive(false);
        }
    }
    public void Options()
    {
        if(!optionsMenu.activeSelf){
            optionsMenu.SetActive(true);
        }  
        else{
            optionsMenu.SetActive(false);
        }
    }

    public void EndTurn()
    {
        cursor.cursorControls.currentActionMap.Disable();
        foreach (Unit unit in FindObjectsOfType<Unit>()) {
            if(unit.unit.allyUnit == true) {
                unit.Rest();
            }
            gameObject.SetActive(false);
        }
    }

    public void Highlight()
    {
        StartCoroutine(HighlightButton());
    }
    private IEnumerator HighlightButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(primaryButton.gameObject);
    }
}

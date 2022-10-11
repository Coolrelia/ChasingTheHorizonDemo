using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentConvoy : MonoBehaviour
{
    public static DeploymentConvoy instance;

    public GameObject itemPrefab;
    private SaveTest saveData;
    [HideInInspector] public Item selectedItem;
    [SerializeField] private Transform menuContent;
    [SerializeField] private GameObject interactMenu;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        saveData = FindObjectOfType<SaveTest>();
        CreateConvoy();
    }

    private void CreateConvoy()
    {
        foreach(Transform child in menuContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < saveData.saveObject.smallConvoy.Count; i++)
        {
            GameObject go = Instantiate(itemPrefab, menuContent);
            go.GetComponent<SmallConvoyItem>().LoadItem(saveData.saveObject.smallConvoy[i]);
        }
    }

    public void InteractMenu()
    {
        interactMenu.SetActive(true);
    }

    public void EquipItem()
    {
        if(DeploymentArmy.instance.selectedUnit.inventory.Count < 5)
        {
            DeploymentArmy.instance.selectedUnit.inventory.Add(selectedItem);
            saveData.saveObject.smallConvoy.Remove(selectedItem);
            selectedItem = null;
            CreateConvoy();
            DeploymentInventory.instance.CreateInventory();
            interactMenu.SetActive(false);
        }
    }

    public void AddItem(Item item)
    {
        saveData.saveObject.smallConvoy.Add(item);
        CreateConvoy();
    }

    public void DiscardItem()
    {
        saveData.saveObject.smallConvoy.Remove(selectedItem);
        selectedItem = null;
        CreateConvoy();
        interactMenu.SetActive(false);
    }

    public void DiscardItem(Item item)
    {
        saveData.saveObject.smallConvoy.Remove(item);
        CreateConvoy();
    }
}

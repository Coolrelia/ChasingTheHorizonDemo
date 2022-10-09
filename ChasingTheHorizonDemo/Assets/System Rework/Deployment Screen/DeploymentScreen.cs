using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentScreen : MonoBehaviour
{
    public static DeploymentScreen instance;

    public List<UnitLoader> deployedUnits = new List<UnitLoader>();

    [SerializeField] private GameObject convoyMenu = null;
    [SerializeField] private GameObject deploymentMenu = null;
    [SerializeField] private GameObject mapPositionMenu = null;
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private TileMap map;
    [SerializeField] private SaveTest saveManager;

    public string activeMenu = "";
    public bool unitsSpawned = false;

    private void Awake()
    {
        instance = this;
    }

    public void OpenConvoy()
    {
        if(convoyMenu.activeSelf){
            convoyMenu.SetActive(false);
            activeMenu = "";
        }
        else{
            convoyMenu.SetActive(true);
            activeMenu = "Convoy";
        }
    }
    public void OpenDeployment()
    {
        if (deploymentMenu.activeSelf){
            deploymentMenu.SetActive(false);
            activeMenu = "";
        }
        else{
            deploymentMenu.SetActive(true);
            activeMenu = "Deployment";
        }
    }
    public void OpenMapPosition()
    {
        if (mapPositionMenu.activeSelf)
        {
            mapPositionMenu.SetActive(false);
        }
        else
        {
            mapPositionMenu.SetActive(true);
        }
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }
    public void Save()
    {
        saveManager.SaveData();
    }
    public void Load()
    {
        saveManager.LoadData();
    }
    public void Engage()
    {
        if (saveManager.saveObject.deployedArmy.Count < MapConditions.instance.minimumDeployableUnits)
        {
            print("you need to deploy units first");
            return;
        }
        if (!unitsSpawned)
        {
            for (int i = 0; i < saveManager.saveObject.deployedArmy.Count; i++)
            {
                GameObject go = Instantiate(saveManager.saveObject.deployedArmy[i].gameObject, map.transform);
                go.transform.position = map.allySpawnPoints[i].position;
            }
        }
        print("starting map");
    }
}

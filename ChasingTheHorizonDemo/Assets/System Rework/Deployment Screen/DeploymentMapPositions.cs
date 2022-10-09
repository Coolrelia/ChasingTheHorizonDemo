using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentMapPositions : MonoBehaviour
{
    public Transform[] allySpawnPoints;
    public Transform grid;
    public CursorController cursor;
    private SaveTest saveData;

    private void Awake()
    {
        saveData = FindObjectOfType<SaveTest>();
    }

    private void OnEnable()
    {
        cursor.SetState(new MapPositionState(cursor));

        foreach(UnitLoader unit in FindObjectsOfType<UnitLoader>())
        {
            if (unit.unit.allyUnit)
            {
                Destroy(unit.gameObject);
            }
        }

        if (saveData.saveObject.deployedArmy.Count > 0)
        {
            for (int i = 0; i < saveData.saveObject.deployedArmy.Count; i++)
            {
                UnitLoader unit = Instantiate(saveData.saveObject.deployedArmy[i], grid);                
                if(unit.startPosition == Vector2.zero)
                {
                    unit.gameObject.transform.position = allySpawnPoints[i].position;
                    unit.startPosition = allySpawnPoints[i].position;
                }
                else
                {
                    unit.transform.position = unit.startPosition;
                }                
            }
            DeploymentScreen.instance.unitsSpawned = true;
        }
    }
}

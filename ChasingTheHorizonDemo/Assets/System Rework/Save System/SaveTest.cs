using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTest : MonoBehaviour
{
    public SaveObject saveObject;
    public SaveManager saveManager;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
    }

    public void SaveData()
    {
        saveManager.Save(saveObject);
    }

    public void LoadData()
    {
        saveObject = saveManager.Load();
    }
}

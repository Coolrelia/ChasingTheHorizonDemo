using System.Collections.Generic;

[System.Serializable]
public class SaveObject
{
    public string sceneName;
    public List<UnitLoader> army = new List<UnitLoader>();
    public List<UnitLoader> deployedArmy = new List<UnitLoader>();
    public List<Item> largeConvoy = new List<Item>();
    public List<Item> smallConvoy = new List<Item>();
}

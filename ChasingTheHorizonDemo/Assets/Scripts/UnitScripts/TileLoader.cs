using UnityEngine;

//TileLoader script loads all the data and functions every tile needs
//Every tile in a scene requires this script to function properly
public class TileLoader : MonoBehaviour
{
    //VARIABLES
    public string tileName = null;
    public int tileCost = 0;
    public bool walkable = true;
    public bool occupied = false;
    
    //REFERENCES
    public Tile tile = null;
    public SpriteRenderer spriteRenderer = null;    
    private GameObject walkableHighlight = null;
    private GameObject attackableHighlight = null;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateOccupationStatus();

        tileName = tile.tileName;
        tileCost = tile.tileCost;

        walkableHighlight = transform.GetChild(0).gameObject;
        attackableHighlight = transform.GetChild(1).gameObject;

        spriteRenderer.sortingOrder = 2;
        
        walkableHighlight.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
        walkableHighlight.GetComponent<SpriteRenderer>().sortingOrder = 1;

        attackableHighlight.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
        attackableHighlight.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void HighlightTile(UnitData unit)
    {
        if(unit.allyUnit)
        {
            walkable = true;
            walkableHighlight.SetActive(true);
        }
        else
        {
            attackableHighlight.SetActive(true);
        }
    }
    public void AttackableTile()
    {
        attackableHighlight.SetActive(true);
    }
    public void ResetTile()
    {
        walkable = false;
        walkableHighlight.SetActive(false);
        attackableHighlight.SetActive(false);
    }
    public void UpdateOccupationStatus()
    {
        occupied = false;
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            if(transform.position == unit.transform.position)
            {
                occupied = true;
            }
        }
    }
}

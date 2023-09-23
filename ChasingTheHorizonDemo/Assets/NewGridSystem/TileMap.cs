using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{
    public TextAsset mapData;
    private string mapValues;

    public int mapSizeX = 0;
    public int mapSizeY = 0;

    public UnitLoader selectedUnit; 

    public TileType[] tileTypes; 
    public int[,] tiles;
    public Node[,] graph;

    public GameObject redHighlight;
    public GameObject blueHighlight;

    public List<Node> walkableTiles;
    public List<Node> attackableTiles;

    public List<UnitLoader> allyUnits = new List<UnitLoader>();
    public List<UnitLoader> enemyUnits = new List<UnitLoader>();

    private void Start()
    {
        mapValues = string.Concat(mapData.text.Where(c => !char.IsWhiteSpace(c)));

        InitializeMapData();
        GeneratePathfindingGraph();
        GenerateMapFromFile();
        GenerateMapVisuals();

        foreach(UnitLoader unit in FindObjectsOfType<UnitLoader>())
        {
            if(unit.unit.allyUnit)
            {
                allyUnits.Add(unit);
            }
            else
            {
                enemyUnits.Add(unit);
            }
        }
    }
    
    private void InitializeMapData()
    {       
        // Allocate our map tiles
        tiles = new int[mapSizeX, mapSizeY];

        // Initialize our map tiles to default values
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                tiles[x, y] = 0;
            }
        }
    }
    private void GeneratePathfindingGraph()
    {
        // Initialize the array
        graph = new Node[mapSizeX, mapSizeY];

        // Initialize a Node for each spot in the array
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // Now that all the nodes exist, calculate their neighbors
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                //We have a 4-way map baby none of that 6-way hex bullshit                
                if(x > 0)
                    graph[x, y].neighbors.Add(graph[x - 1, y]); //Adds neighbor to the left
                if (x < mapSizeX - 1)
                    graph[x, y].neighbors.Add(graph[x + 1, y]); //Adds neighbor to the right
                if(y > 0)
                    graph[x, y].neighbors.Add(graph[x, y - 1]); //Adds neighbor below
                if(y < mapSizeY - 1)
                    graph[x, y].neighbors.Add(graph[x, y + 1]); //Adds neighbor above
            }
        }

        foreach(Node n in graph)
        {
            n.SetNeighbors();
        }
    }
    private void GenerateMapVisuals()
    {
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){

                TileType tt = tileTypes[tiles[x, y]];

                GameObject go = Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                SelectableTile st = go.GetComponent<SelectableTile>();
                st.tileX = x;
                st.tileY = y;
                st.map = this;
            }
        }

        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){

                graph[x, y].redHighlight = Instantiate(redHighlight, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity, transform);
                graph[x, y].blueHighlight = Instantiate(blueHighlight, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity, transform);

                graph[x, y].redHighlight.SetActive(false);
                graph[x, y].blueHighlight.SetActive(false);
            }
        }

        foreach(SelectableTile tile in FindObjectsOfType<SelectableTile>())
        {
            tile.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        }
    }
    private void GenerateMapFromFile()
    {        
        int y = 0;
        while(y < mapSizeY)
        {
            for(int x = 0; x < mapSizeX; x++)
            {
                tiles[x, y] = (int)char.GetNumericValue(mapValues[x]);
            }
            mapValues = mapValues.Remove(0, 20);
            y++;
        }   
    }

    public void GeneratePathTo(int x, int y, UnitLoader unit)
    {
        unit.currentPath = null;

        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        // List of nodes we haven't checked yet
        List<Node> unvisitedNodes = new List<Node>();

        int sourceX = (int)(unit.transform.localPosition.x);
        int sourceY = (int)(unit.transform.localPosition.y);
        Node source = graph[sourceX, sourceY];
        Node target = graph[x, y];

        distance[source] = 0;
        previous[source] = null;

        // Initialize everything to have infinie distance
        // since we don't know any better right now.
        foreach (Node n in graph)
        {
            if(n != source)
            {
                distance[n] = Mathf.Infinity;
                previous[n] = null;
            }
            unvisitedNodes.Add(n);
        }

        // This makes sure we don't throw an error if you try to move to the tile you're current standing on
        if(source == target)
        {
            return;
        }        

        // The main loop
        while(unvisitedNodes.Count > 0)
        {
            // The unvisited node with the smallest distance.
            Node closestNode = null;

            foreach(Node possibleClosest in unvisitedNodes)
            {
                if(unit.unit.allyUnit)
                {
                    if(closestNode == null || distance[possibleClosest] < distance[closestNode] && !IsOccupiedByEnemy(possibleClosest.x, possibleClosest.y))
                        closestNode = possibleClosest;
                }
                else
                {
                    if(closestNode == null || distance[possibleClosest] < distance[closestNode])
                        closestNode = possibleClosest;
                }

            }

            if(closestNode == target)
            {
                // Target found!
                break;
            }

            // Now we have visited it, remove it
            unvisitedNodes.Remove(closestNode);

            foreach(Node n in closestNode.neighbors)
            {
                float temp = distance[closestNode] + CostToEnterTile(n.x, n.y);
                if(temp < distance[n])
                {
                    distance[n] = temp;
                    previous[n] = closestNode;
                }
            }
        }

        if(previous[target] == null)
        {
            //no route between target and source
            return;
        }        

        List<Node> currentPath = new List<Node>();
        Node current = target;

        // Step back through to find route from the target
        while(current != null)
        {
            currentPath.Add(current);
            current = previous[current];
        }

        int totalCost = 0;
        foreach (Node n in currentPath)
        {
            if(n != currentPath[0]) totalCost += (int)CostToEnterTile(n.x, n.y);
        }

        // Reverse path from target to source to make it a path from source to target
        currentPath.Reverse();

        unit.currentPath = currentPath;
    }
    public int GenerateCostBetween(int sourceX, int sourceY, int targetX, int targetY)
    {
        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();

        // List of nodes we haven't checked yet
        List<Node> unvisitedNodes = new List<Node>();

        Node source = graph[sourceX, sourceY];
        Node target = graph[targetX, targetY];

        distance[source] = 0;
        previous[source] = null;

        // Initialize everything to have infinie distance
        // since we don't know any better right now.
        foreach (Node n in graph)
        {
            if (n != source)
            {
                distance[n] = Mathf.Infinity;
                previous[n] = null;
            }
            unvisitedNodes.Add(n);
        }

        // This makes sure we don't throw an error if you try to move to the tile you're current standing on
        if(source == target)
        {
            return 0;
        }

        // The main loop
        while (unvisitedNodes.Count > 0)
        {
            // The unvisited node with the smallest distance.
            Node closestNode = null;

            foreach (Node possibleClosest in unvisitedNodes)
            {
                if (closestNode == null || distance[possibleClosest] < distance[closestNode] && !IsOccupiedByEnemy(possibleClosest.x, possibleClosest.y))
                    closestNode = possibleClosest;
            }

            if (closestNode == target)
            {
                // Target found!
                break;
            }

            // Now we have visited it, remove it
            unvisitedNodes.Remove(closestNode);

            foreach (Node n in closestNode.neighbors)
            {
                float temp = distance[closestNode] + CostToEnterTile(n.x, n.y);
                if (temp < distance[n])
                {
                    distance[n] = temp;
                    previous[n] = closestNode;
                }
            }
        }

        if (previous[target] == null)
        {
            //no route between target and source
            return 99;
        }

        List<Node> currentPath = new List<Node>();
        Node current = target;

        // Step back through to find route from the target
        while(current != null)
        {
            currentPath.Add(current);
            current = previous[current];
        }

        // Reverse path from target to source to make it a path from source to target
        currentPath.Reverse();

        int totalCost = 0;
        foreach(Node n in currentPath)
        {
            if (n != currentPath[0]) totalCost += (int)CostToEnterTile(n.x, n.y);
        }
        return totalCost;
    }

    public List<Node> GenerateRange(int x, int y, int size, UnitLoader unit, bool pathing)
    {
        // Final list that gets returned
        List<Node> finalList = new List<Node>();

        // Temporary list of nodes that still need to be searched
        List<Node> workingList = new List<Node>();
        
        List<Node> tempList = new List<Node>();

        // Add starting position to temp list
        finalList.Add(graph[x, y]);

        foreach(Node n in graph[x, y].neighbors)
        {
            if(!pathing)
            {
                if(CostToEnterTile(n.x, n.y) < unit.unit.statistics.movement && !workingList.Contains(n))
                {
                    workingList.Add(n);
                }
            }
            else
            {
                if(CostToEnterTile(n.x, n.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(n.x, n.y) && !workingList.Contains(n))
                {
                    workingList.Add(n);
                }
            }            
        }

        for(int i = 1; i <= size;)
        {
            // Clear the temp list
            tempList.Clear();

            // Copy working list over to temp list
            foreach(Node n in workingList)
            {
                if(!tempList.Contains(n))
                    tempList.Add(n);
            }

            // Copy working list over to final list
            foreach(Node n in workingList)
            {
                if(!pathing)
                {
                    if(CostToEnterTile(n.x, n.y) < unit.unit.statistics.movement && !finalList.Contains(n))
                    {
                        finalList.Add(n);
                    }
                }
                else
                {
                    if(CostToEnterTile(n.x, n.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(n.x, n.y) && !finalList.Contains(n))
                    {
                        finalList.Add(n);
                    }
                }
            }

            // Now that there are 2 copies of working list, clear it as it needs to be filled with some new nodes
            workingList.Clear();

            // For every node in the temp list, add all it's neighbors to the working list
            foreach(Node n in tempList)
            {
                foreach(Node e in n.neighbors)
                {
                    if(!pathing)
                    {
                        if(CostToEnterTile(e.x, e.y) < unit.unit.statistics.movement && !workingList.Contains(n))
                        {
                            workingList.Add(e);
                        }
                    }
                    else
                    {
                        if(CostToEnterTile(e.x, e.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(e.x, e.y) && !workingList.Contains(n))
                        {
                            workingList.Add(e);
                        }
                    }
                }
            }

            i++;

            if(i == size)
            {
                foreach(Node n in workingList)
                    if(!finalList.Contains(n))
                        finalList.Add(n);
            }
        }        
        return finalList;
    }
    public List<Node> GenerateWalkableRange(int x, int y, int size, UnitLoader unit)
    {
        // Final list that gets returned
        List<Node> finalList = new List<Node>();

        // Temporary list of nodes that still need to be searched
        List<Node> workingList = new List<Node>();

        List<Node> tempList = new List<Node>();

        // Add starting position to temp list
        finalList.Add(graph[x, y]);

        foreach(Node n in graph[x, y].neighbors)
        {
            if(CostToEnterTile(n.x, n.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(n.x, n.y) && !workingList.Contains(n))
            {
                workingList.Add(n);
            }
        }

        for (int i = 1; i <= size;)
        {
            // Clear the temp list
            tempList.Clear();

            // Copy working list over to temp list
            foreach (Node n in workingList)
            {
                if (!tempList.Contains(n))
                    tempList.Add(n);
            }

            // Copy working list over to final list
            foreach (Node n in workingList)
            {
                if(CostToEnterTile(n.x, n.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(n.x, n.y) && !finalList.Contains(n))
                {
                    finalList.Add(n);
                }
            }

            // Now that there are 2 copies of working list, clear it as it needs to be filled with some new nodes
            workingList.Clear();

            // For every node in the temp list, add all it's neighbors to the working list
            foreach (Node n in tempList)
            {
                foreach (Node e in n.neighbors)
                {
                    if(CostToEnterTile(e.x, e.y) + n.DistanceTo(ReturnNodeAt(x, y)) <= unit.unit.statistics.movement + 1 && !IsOccupiedByEnemy(e.x, e.y) && !workingList.Contains(n))
                    {
                        workingList.Add(e);
                    }
                }
            }

            i++;

            if (i == size)
            {
                foreach (Node n in workingList)
                    if (!finalList.Contains(n))
                        finalList.Add(n);
            }
        }

        Node sourceNode = ReturnNodeAt((int)unit.transform.localPosition.x, (int)unit.transform.localPosition.y);
        foreach(Node n in finalList.ToList<Node>())
        {
            if(GenerateCostBetween(sourceNode.x, sourceNode.y, n.x, n.y) > unit.unit.statistics.movement)
            {
                finalList.Remove(n);
            }
        }
        return finalList;
    } 
    public List<Node> GenerateAttackableRange(int x, int y, int size, UnitLoader unit)
    {
        // Final list that gets returned
        List<Node> finalList = new List<Node>();

        // Temporary list of nodes that still need to be searched
        List<Node> workingList = new List<Node>();

        List<Node> tempList = new List<Node>();

        // Add starting position to temp list
        finalList.Add(graph[x, y]);

        foreach(Node n in graph[x, y].neighbors)
        {
            if (CostToEnterTile(n.x, n.y) < unit.unit.statistics.movement && !workingList.Contains(n))
            {
                workingList.Add(n);
            }
            else if(CostToEnterTile(n.x, n.y) > unit.unit.statistics.movement && ReturnTileAt(n.x, n.y).isWalkable == false && !workingList.Contains(n))
            {
                workingList.Add(n);
            }
        }

        for (int i = 1; i <= size;)
        {
            // Clear the temp list
            tempList.Clear();

            // Copy working list over to temp list
            foreach (Node n in workingList)
            {
                if (!tempList.Contains(n))
                    tempList.Add(n);
            }

            // Copy working list over to final list
            foreach (Node n in workingList)
            {
                if(CostToEnterTile(n.x, n.y) < unit.unit.statistics.movement && !finalList.Contains(n))
                {
                    finalList.Add(n);
                }
                else if(CostToEnterTile(n.x, n.y) > unit.unit.statistics.movement && ReturnTileAt(n.x, n.y).isWalkable == false && !finalList.Contains(n))
                {
                    finalList.Add(n);
                }
            }

            // Now that there are 2 copies of working list, clear it as it needs to be filled with some new nodes
            workingList.Clear();

            // For every node in the temp list, add all it's neighbors to the working list
            foreach (Node n in tempList)
            {
                foreach (Node e in n.neighbors)
                {
                    if(CostToEnterTile(e.x, e.y) < unit.unit.statistics.movement && !workingList.Contains(e))
                    {
                        workingList.Add(e);
                    }
                    else if(CostToEnterTile(e.x, e.y) > unit.unit.statistics.movement && ReturnTileAt(e.x, e.y).isWalkable == false && !workingList.Contains(e))
                    {
                        workingList.Add(e);
                    }
                }
            }

            i++;

            if (i == size)
            {
                foreach (Node n in workingList)
                    if (!finalList.Contains(n))
                        finalList.Add(n);
            }
        }
        
        foreach(Node n in finalList.ToList<Node>())
        {
            Node node = ClosestWalkableNode(n.x, n.y);            
            if(n.DistanceTo(ReturnNodeAt(node.x, node.y)) > selectedUnit.equippedWeapon.range)
            {
                finalList.Remove(n);
            }
        }

        return finalList;
    }
    
    public void CleanAttackableTiles()
    {
        foreach(Node n in attackableTiles.ToList<Node>())
        {
            if(walkableTiles.Contains(n) && !IsOccupiedByEnemy(n.x, n.y))
            {
                attackableTiles.Remove(n);
            }
        }
    }    

    private float CostToEnterTile(int x, int y)
    {
        TileType tt = tileTypes[tiles[x,y]];

        return tt.movementCost;
    }
    public bool CanTraverse(int x, int y)
    {
        if(tileTypes[tiles[x, y]].isWalkable == false)
        {
            return false;
        }
        foreach(UnitLoader u in TurnManager.instance.allyUnits)
        {
            if(u != selectedUnit && u.transform.localPosition == new Vector3(x, y))
            {
                return false;
            }
        }
        foreach(UnitLoader u in TurnManager.instance.enemyUnits)
        {
            if(u.transform.localPosition == new Vector3(x, y))
            {
                return false;
            }
        }
        return true;
    }
    public bool IsOccupied(int x, int y)
    {
        foreach(UnitLoader unit in FindObjectsOfType<UnitLoader>())
        {
            if(unit.transform.localPosition == new Vector3(x, y))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOccupiedByEnemy(int x, int y)
    {
        foreach (UnitLoader unit in enemyUnits)
        {
            if(unit.transform.localPosition == new Vector3(x, y))
            {
                return true;
            }
        }
        return false;
    }
    public TileType ReturnTileAt(int x, int y)
    {
        return tileTypes[tiles[x, y]];
    }
    public Node ReturnNodeAt(int x, int y)
    {
        return graph[x, y];
    }
    public Node ClosestWalkableNode(int x, int y)
    {
        Node closestNode = null;
        int closestSoFar = 100;
        foreach (Node n in walkableTiles)
        {
            if(n.DistanceTo(ReturnNodeAt(x, y)) < closestSoFar)
            {
                closestSoFar = n.DistanceTo(ReturnNodeAt(x, y));
                closestNode = n;
            }
        }
        return closestNode;
    }

    public void HighlightTiles()
    {
        if(attackableTiles != null)
        {
            foreach(Node n in attackableTiles)
            {
                n.redHighlight.SetActive(true);
            }
        }
        if(walkableTiles != null)
        {
            foreach(Node n in walkableTiles)
            {
                if(!IsOccupiedByEnemy(n.x, n.y))
                {
                    n.blueHighlight.SetActive(true);
                }                
            }
        }
    }
    public void DehighlightTiles()
    {
        foreach(Node n in graph)
        {
            n.blueHighlight.SetActive(false);
            n.redHighlight.SetActive(false);
        }
        walkableTiles = null;
        attackableTiles = null;
    }
}
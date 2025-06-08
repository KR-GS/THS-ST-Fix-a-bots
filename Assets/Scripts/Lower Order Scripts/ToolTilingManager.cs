using Unity.VisualScripting;
using UnityEngine;

public class ToolTilingManager : MonoBehaviour
{
    private PartTile tileObj;

    private Vector2 tilePosition;

    private Vector2 tileSize;

    private GameObject[] tiles;
    
    void Awake()
    {
        tileObj = FindFirstObjectByType<PartTile>();

        Debug.Log(tileObj.gameObject);

        tilePosition = tileObj.transform.position;

        tileSize = tileObj.GetSpriteSize();
    }

    public void SpawnPartTiled(int numberOfTiles)
    {
        tiles = new GameObject[numberOfTiles];

        Debug.Log("Hello World");

        for (int i = 0; i < numberOfTiles; i++)
        {
            tiles[i] = Instantiate(tileObj.gameObject);
            if(i>0)
                tiles[i].transform.position = new Vector2(tiles[i-1].transform.position.x + tiles[i].GetComponentInChildren<SpriteRenderer>().bounds.size.x, tilePosition.y);
        }
    }

    public float TileMidPoint()
    {
        float midPoint;

        midPoint = (tiles[0].transform.position.x + tiles[tiles.Length - 1].transform.position.x) / 2;

        return midPoint;
    }

    public GameObject[] GetTileList()
    {
        return tiles;
    }
}

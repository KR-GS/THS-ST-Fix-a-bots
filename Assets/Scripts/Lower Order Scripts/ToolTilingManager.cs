using Unity.VisualScripting;
using UnityEngine;

public class ToolTilingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject partPrefab;

    private PartTile tileObj;

    private Vector2 tilePosition;

    private Vector2 tileSize;

    private GameObject[] tiles;
    
    void Awake()
    {
        tileObj = partPrefab.GetComponent<PartTile>();

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
                tiles[i].transform.position = new Vector2(tiles[i-1].transform.position.x + tileSize.x, tilePosition.y);
        }
    }

    public Vector3 GetTileDistance()
    {
        Vector3 distance;

        distance = tiles[1].transform.position;

        return distance;
    }

    public float TileMidPoint()
    {
        float midPoint;

        midPoint = (tiles[0].transform.position.x + tiles[tiles.Length - 1].transform.position.x) / 2;

        return midPoint;
    }

    public Vector2 TileLength()
    {
        Vector2 tile_Length = new Vector2(tileSize.x * tiles.Length, tileSize.y);

        return tile_Length;
    }

    public GameObject[] GetTileList()
    {
        return tiles;
    }

    public float GetYPosOfTile()
    {
        return tiles[0].GetComponent<PartTile>().GetSpritePosition().y;
    }
}

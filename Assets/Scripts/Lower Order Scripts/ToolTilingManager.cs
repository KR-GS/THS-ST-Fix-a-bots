using UnityEngine;

public class ToolTilingManager : MonoBehaviour
{
    private PartTile tileObj;

    private Vector2 tilePosition;

    private Vector2 tileSize;

    private GameObject[] tiles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileObj = FindFirstObjectByType<PartTile>();

        tilePosition = tileObj.transform.position;

        tileSize = tileObj.GetSpriteSize();
    }

    public void SpawnPartTiled(int numberOfTiles)
    {
        tiles = new GameObject[numberOfTiles];

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

        midPoint = tiles[0].transform.position.x + tiles[tiles.Length - 1].transform.position.x;

        return midPoint;
    }

    public GameObject[] GetTileList()
    {
        return tiles;
    }
}

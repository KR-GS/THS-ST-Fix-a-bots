using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ToolTilingManager : MonoBehaviour
{
    [SerializeField]
    private GameObject partPrefab;

    [SerializeField]
    private GameObject[] ruler_lines;

    [SerializeField]
    private GameObject ruler_base;

    private PartTile tileObj;

    private Vector2 tilePosition;

    private Vector2 tileSize;

    private GameObject[] tiles;

    private GameObject tile_complete;

    //private List<Vector3> tilesPoints = new List<Vector3>();

    private List<GameObject> fastener = new List<GameObject>();

    void Awake()
    {
        tile_complete = new GameObject("Robot Part");

        tileObj = partPrefab.GetComponent<PartTile>();

        Debug.Log(tileObj.gameObject);

        tilePosition = tileObj.transform.position;

        tileSize = tileObj.GetSpriteSize();
    }

    public void SpawnPartTiled(int numberOfTiles)
    {
        tiles = new GameObject[numberOfTiles];

        List<Vector3> tilesPoints = new List<Vector3>();

        Debug.Log("Hello World");

        for (int i = 0; i < numberOfTiles; i++)
        {
            tiles[i] = Instantiate(tileObj.gameObject, tile_complete.transform);
            if(i>0)
                tiles[i].transform.position = new Vector2(tiles[i-1].transform.position.x + tiles[i].GetComponentInChildren<SpriteRenderer>().bounds.size.x, tilePosition.y);
        }

        for(int i = 0; i < numberOfTiles; i++)
        {
            tilesPoints.Add(new Vector3(tiles[i].transform.position.x, tiles[i].GetComponent<PartTile>().GetFastenerPosition().position.y, tiles[i].transform.position.z));

            if(i < numberOfTiles - 1)
            {
                tilesPoints.Add(new Vector3(tiles[i].transform.position.x + (tileSize.x / 2), tiles[i].GetComponent<PartTile>().GetFastenerPosition().position.y, tiles[i].transform.position.z));
            }
        }

        Debug.Log("Total points for fasteners: " + tilesPoints.Count);

        for (int i = 0; i<tilesPoints.Count; i++)
        {
            fastener.Add(new GameObject());

            fastener[i].transform.position = tilesPoints[i];

            fastener[i].transform.SetParent(tile_complete.transform);
        }

        ruler_base.transform.localScale = new Vector2((tileSize.x * tiles.Length)*2, ruler_base.transform.localScale.y);

        ruler_lines[0].transform.SetParent(ruler_base.transform);

        ruler_lines[1].transform.SetParent(ruler_base.transform);

        ruler_base.transform.position = new Vector3(TileMidPoint(), tiles[0].transform.position.y - 1, ruler_base.transform.position.z);

        //ruler_lines[1].transform.position = new Vector3(tilesPoints[0].x, ruler_lines[1].transform.position.y, ruler_lines[1].transform.position.z);

        //ruler_lines[1].GetComponentInChildren<TextMeshProUGUI>().text = (1).ToString();

        for (int i = 0; i < tilesPoints.Count; i++)
        {
            GameObject newObj = Instantiate(ruler_lines[1], ruler_base.transform);
            newObj.transform.position = new Vector3(tilesPoints[i].x, newObj.transform.position.y, newObj.transform.position.z);
            newObj.GetComponentInChildren<TextMeshProUGUI>().text = (i+1).ToString();
            if (i>=1)
            {
                GameObject newObj2 = Instantiate(ruler_lines[0], ruler_base.transform);
                newObj2.transform.position = new Vector3((tilesPoints[i-1].x + tilesPoints[i].x) / 2, newObj2.transform.position.y, newObj2.transform.position.z);
            }
        }

        Destroy(ruler_lines[0]);
        Destroy(ruler_lines[1]);
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

    public Transform SetFastener(int position)
    {
        return fastener[position-1].transform;
    }

    public void IsDragged()
    {
        ruler_base.transform.SetParent(tile_complete.transform);
    }

    public void IsNotDragged()
    {
        ruler_base.transform.SetParent(null);
    }

    public void SetCenterFocus(int index)
    {
        IsDragged();
        tile_complete.transform.position = new Vector3(tile_complete.transform.position.x - fastener[index].transform.position.x, tile_complete.transform.position.y, tile_complete.transform.position.z);
        IsNotDragged();
    }

    public Vector3 CheckPointPosition(int index)
    {
        Debug.Log("Fastener " + index + " position: " + fastener[index].transform.position);

        return fastener[index].transform.position;
    }


}

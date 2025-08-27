using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FastenerBtn : MonoBehaviour
{
    private enum FastenerType
    {
        nail,
        philips,
        flat,
        bolt
    }

    [SerializeField]
    private GameObject fastenerPrefab;

    [SerializeField]
    private GameObject fastenerHitPrefab;

    [SerializeField]
    private FastenerType fastenerType;

    [SerializeField]
    private int fastenerValue;

    [SerializeField]
    private GameObject tool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject GetFastenerSprite()
    {
        return fastenerPrefab;
    }

    public GameObject GetHitIcon()
    {
        return fastenerHitPrefab;
    }

    public int GetFastenerType()
    {
        return fastenerValue;
    }

    public GameObject GetToolToUse()
    {
        return tool;
    }
}

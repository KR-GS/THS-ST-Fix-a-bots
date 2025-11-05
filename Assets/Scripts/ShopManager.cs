using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    public class ShopItem
    {
        public string itemName;
        public Sprite rating;
        public Sprite icon;
        public int price;
        public int hammerID;
    }

    public TextMeshProUGUI moneyText;
    public GameObject shopObject;
    [SerializeField] private GameLoopManager glm;
    [SerializeField] private OrderManager om;
    [SerializeField] private RaycastInteractor ri;
    [SerializeField] private TimerScript ts;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform content; // Reference to Content object
    public List<ShopItem> allItems;
    private List<ShopItemUI> spawnedItemUIs = new List<ShopItemUI>();
    public Sprite oneStar;
    public Sprite twoStar;
    public Sprite threeStar;
    public Sprite fourStar;
    public Sprite fiveStar;
    public Sprite blueHammer;
    public Sprite greenHammer;
    public Sprite redHammer;

    void Start()
    {
        moneyText.text = glm.money.ToString();
        // Now all Inspector references are guaranteed to be assigned
        allItems = new List<ShopItem>()
        {
            new ShopItem() { itemName = "Rusty Hammer", rating = oneStar, icon = blueHammer, price = 0, hammerID = 0 },
            new ShopItem() { itemName = "Green Hammer", rating = threeStar, icon = greenHammer, price = 150, hammerID = 1 },
            new ShopItem() { itemName = "Chill Boy's Hammer", rating = fiveStar, icon = redHammer, price = 300, hammerID = 2 }
        };

        PopulateShop();

        if (shopObject != null)
        {
            shopObject.SetActive(false);
        }
    }
    private void PopulateShop()
    {
        // Clear existing items (if any)
        Debug.Log("PopulateShop called!"); // Check if method runs
        Debug.Log($"Content reference: {content}"); // Check if content exists
        Debug.Log($"ItemPrefab reference: {itemPrefab}"); // Check if prefab exists
        Debug.Log($"AllItems count: {allItems.Count}");

        spawnedItemUIs.Clear();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Create items
        foreach (ShopItem item in allItems)
        {
            GameObject itemObj = Instantiate(itemPrefab, content);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();

            if (itemUI != null)
            {
                itemUI.Setup(item, OnItemClicked);
                spawnedItemUIs.Add(itemUI);
            }
        }
    }

    private void OnItemClicked(ShopItem item, ShopItemUI clickedUI)
    {
        bool isBought = IsItemBought(item.hammerID);

        if (!isBought)
        {
            // Try to buy
            if (item.price > glm.money)
            {
                Debug.Log("Not enough gold!");
                return;
            }
            else
            {
                Debug.Log($"SUCCESSFULLY Bought {item.itemName} for {item.price} gold!");
                SetItemBought(item.hammerID, true);
                glm.money -= item.price;
                moneyText.text = glm.money.ToString();

                // Auto-equip after purchase
                StaticData.equippedHammer = item.hammerID;
                Debug.Log($"SUCCESSFULLY Equipped {item.itemName}!");

                RefreshAllButtons();
            }
        }
        else
        {
            // Equip the item
            if (StaticData.equippedHammer != item.hammerID)
            {
                StaticData.equippedHammer = item.hammerID;
                Debug.Log($"SUCCESSFULLY Equipped {item.itemName}!");
                RefreshAllButtons();
            }
        }
    }

    private void RefreshAllButtons()
    {
        foreach (ShopItemUI itemUI in spawnedItemUIs)
        {
            itemUI.UpdateButtonState();
        }
    }

    private bool IsItemBought(int hammerID)
    {
        switch (hammerID)
        {
            case 0: return StaticData.isRustyHammerBought;
            case 1: return StaticData.isGreenHammerBought;
            case 2: return StaticData.isRedHammerBought;
            default: return false;
        }
    }

    private void SetItemBought(int hammerID, bool value)
    {
        switch (hammerID)
        {
            case 0: StaticData.isRustyHammerBought = value; break;
            case 1: StaticData.isGreenHammerBought = value; break;
            case 2: StaticData.isRedHammerBought = value; break;
        }
    }

    public void ShowShop()
    {
        if (shopObject != null)
            shopObject.SetActive(true);

        //RaycastInteractor.Instance.DisableRaycasting();

        ts.StopTimer();

        glm.HideWorkshopElements();

        
        if (ri.timeText != null)
        {
            ri.timeText.gameObject.SetActive(false); // Hide the time text
        }

        PopulateShop();

        RefreshAllButtons();

    }

    public void CloseShop()
    {
        if (shopObject != null)
        {
            shopObject.SetActive(false);
        }

        ts.StartTimer();

        DataPersistenceManager.Instance.SaveGame();
        glm.ShowWorkshopElements();

    }
}



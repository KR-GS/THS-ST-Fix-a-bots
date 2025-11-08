using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    public enum ItemCategory
    {
        Hammer,
        PhilipsScrewdriver,
        FlatScrewdriver,
        Wrench
    }
    public class ShopItem
    {
        public string itemName;
        public Sprite rating;
        public Sprite icon;
        public int price;
        public int itemID;
        public ItemCategory category;
    }

    public TextMeshProUGUI moneyText;
    public GameObject shopObject;
    [SerializeField] private GameLoopManager glm;
    [SerializeField] private OrderManager om;
    [SerializeField] private RaycastInteractor ri;
    [SerializeField] private GameObject itemPrefab;
    //[SerializeField] private Transform content; // Reference to Content object
    [SerializeField] private RectTransform content; // Change from Transform to RectTransform
    public Button pause;
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
            new ShopItem() { itemName = "MXWL Hammer", rating = oneStar, icon = blueHammer, price = 0, itemID = 0, category = ItemCategory.Hammer },
            new ShopItem() { itemName = "Red Hammer", rating = threeStar, icon = redHammer, price = 150, itemID = 1, category = ItemCategory.Hammer },
            new ShopItem() { itemName = "Chill Boy's Hammer", rating = fiveStar, icon = greenHammer, price = 300, itemID = 2, category = ItemCategory.Hammer },
            new ShopItem() { itemName = "Blue Screwdriver", rating = oneStar, icon = greenHammer, price = 0, itemID = 3, category = ItemCategory.PhilipsScrewdriver},
            new ShopItem() { itemName = "Red Screwdriver", rating = threeStar, icon = redHammer, price = 200, itemID = 4, category = ItemCategory.PhilipsScrewdriver },

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
        bool isBought = IsItemBought(item.itemID);

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
                SetItemBought(item.itemID, true);
                glm.money -= item.price;
                moneyText.text = glm.money.ToString();

                // Auto-equip after purchase
                EquipItem(item);
                Debug.Log($"SUCCESSFULLY Equipped {item.itemName}!");

                RefreshAllButtons();
            }
        }
        else
        {
            // Equip the item (check if not already equipped)
            if (!IsItemEquipped(item))
            {
                EquipItem(item);
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

    private bool IsItemBought(int itemID)
    {
        switch (itemID)
        {
            case 0: return StaticData.isRustyHammerBought;
            case 1: return StaticData.isGreenHammerBought;
            case 2: return StaticData.isRedHammerBought;

            // Phillips Screwdrivers
            case 3: return StaticData.isBlueScrewdriverBought;
            case 4: return StaticData.isRedScrewdriverBought;

            // Add more as needed
            default: return false;
        }
    }

    private void SetItemBought(int itemID, bool value)
    {
        switch (itemID)
        {
            // Hammers
            case 0: StaticData.isRustyHammerBought = value; break;
            case 1: StaticData.isGreenHammerBought = value; break;
            case 2: StaticData.isRedHammerBought = value; break;

            // Phillips Screwdrivers
            case 3: StaticData.isBlueScrewdriverBought = value; break;
            case 4: StaticData.isRedScrewdriverBought = value; break;

        }
    }

    private void EquipItem(ShopItem item)
    {
        switch (item.category)
        {
            case ItemCategory.Hammer:
                StaticData.equippedHammer = item.itemID;
                break;
            case ItemCategory.PhilipsScrewdriver:
                StaticData.equippedPhilipsScrewdriver = item.itemID;
                break;
                /*
            case ItemCategory.FlatScrewdriver:
                StaticData.equippedFlatScrewdriver = item.itemID;
                break;
            case ItemCategory.Wrench:
                StaticData.equippedWrench = item.itemID;
                break;
                */
        }
    }

    private bool IsItemEquipped(ShopItem item)
    {
        switch (item.category)
        {
            case ItemCategory.Hammer:
                return StaticData.equippedHammer == item.itemID;
            case ItemCategory.PhilipsScrewdriver:
                return StaticData.equippedPhilipsScrewdriver == item.itemID;
                /*
            case ItemCategory.FlatScrewdriver:
                return StaticData.equippedFlatScrewdriver == item.itemID;
            case ItemCategory.Wrench:
                return StaticData.equippedWrench == item.itemID;
                */
            default:
                return false;
        }
    }

    public void ShowShop()
    {
        if (shopObject != null)
            shopObject.SetActive(true);

        //RaycastInteractor.Instance.DisableRaycasting();

        om.orderCompletePanel.SetActive(false);
        pause.gameObject.SetActive(false);


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

        DataPersistenceManager.Instance.SaveGame();
        pause.gameObject.SetActive(true);
        om.orderCompletePanel.SetActive(true);
        glm.shopButton.gameObject.SetActive(true);

    }
}



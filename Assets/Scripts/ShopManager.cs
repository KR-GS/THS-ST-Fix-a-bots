using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Sprite blueHammer; //default
    public Sprite greenHammer;
    public Sprite redHammer;

    public Sprite greenPhilips; //default
    public Sprite yellowPhilips;
    public Sprite redPhilips;

    public Sprite yellowFlat; //default
    public Sprite redFlat;
    public Sprite greenFlat;

    public Sprite redWrench; //default
    public Sprite blueWrench;
    public Sprite greenWrench;


    void Start()
    {
        moneyText.text = glm.money.ToString();
        // Now all Inspector references are guaranteed to be assigned
        allItems = new List<ShopItem>()
        {
            new ShopItem() { itemName = "MXWL Hammer", rating = oneStar, icon = blueHammer, price = 0, itemID = 0, category = ItemCategory.Hammer },
            new ShopItem() { itemName = "Hot Red Hammer", rating = threeStar, icon = redHammer, price = 450, itemID = 1, category = ItemCategory.Hammer }, //450
            new ShopItem() { itemName = "Chill Boy's Hammer", rating = fourStar, icon = greenHammer, price = 900, itemID = 2, category = ItemCategory.Hammer }, //900
            new ShopItem() { itemName = "Green Philips", rating = oneStar, icon = greenPhilips, price = 0, itemID = 3, category = ItemCategory.PhilipsScrewdriver}, 
            new ShopItem() { itemName = "Dark Yellow Philips", rating = twoStar, icon = yellowPhilips, price = 400, itemID = 4, category = ItemCategory.PhilipsScrewdriver }, //400
            new ShopItem() { itemName = "Blazing Neon Philips", rating = fiveStar, icon = redPhilips, price = 800, itemID = 5, category = ItemCategory.PhilipsScrewdriver}, //800
            new ShopItem() { itemName = "Ol' Reliable Flathead", rating = twoStar, icon = yellowFlat, price = 0, itemID = 6, category = ItemCategory.FlatScrewdriver },
            new ShopItem() { itemName = "Crimson Flathead", rating = threeStar, icon = redFlat, price = 400, itemID = 7, category = ItemCategory.FlatScrewdriver }, //400
            new ShopItem() { itemName = "Sage Flathead", rating = fourStar, icon = greenFlat, price = 800, itemID = 8, category = ItemCategory.FlatScrewdriver }, //800
            new ShopItem() { itemName = "Ruby Wrench", rating = twoStar, icon = redWrench, price = 0, itemID = 9, category = ItemCategory.Wrench },
            new ShopItem() { itemName = "Sapphire Wrench", rating = threeStar, icon = blueWrench, price = 500, itemID = 10, category = ItemCategory.Wrench }, //500
            new ShopItem() { itemName = "Emerald Wrench", rating = fiveStar, icon = greenWrench, price = 1000, itemID = 11, category = ItemCategory.Wrench}, //1000
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
            case 0: return StaticData.isBlueHammerBought;
            case 1: return StaticData.isRedHammerBought;
            case 2: return StaticData.isGreenHammerBought;

            case 3: return StaticData.isGreenPhilipsBought;
            case 4: return StaticData.isYellowPhilipsBought;
            case 5: return StaticData.isRedPhilipsBought;

            case 6: return StaticData.isYellowFlatBought;
            case 7: return StaticData.isRedFlatBought;
            case 8: return StaticData.isGreenFlatBought;

            case 9: return StaticData.isRedWrenchBought;
            case 10: return StaticData.isBlueWrenchBought;
            case 11: return StaticData.isGreenWrenchBought;


            default: return false;
        }
    }

    private void SetItemBought(int itemID, bool value)
    {
        switch (itemID)
        {

            case 0: StaticData.isBlueHammerBought = value; break;
            case 1: StaticData.isRedHammerBought = value; break;
            case 2: StaticData.isGreenHammerBought = value; break;

            case 3: StaticData.isGreenPhilipsBought = value; break;
            case 4: StaticData.isYellowPhilipsBought = value; break;
            case 5: StaticData.isRedPhilipsBought = value; break;

            case 6: StaticData.isYellowFlatBought = value; break;
            case 7: StaticData.isRedFlatBought = value; break;
            case 8: StaticData.isGreenFlatBought = value; break;

            case 9: StaticData.isRedWrenchBought = value; break;
            case 10: StaticData.isBlueWrenchBought = value; break;
            case 11: StaticData.isGreenWrenchBought = value; break;

            default: break;
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
            case ItemCategory.FlatScrewdriver:
                StaticData.equippedFlatScrewdriver = item.itemID;
                break;
            case ItemCategory.Wrench:
                StaticData.equippedWrench = item.itemID;
                break;
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
            case ItemCategory.FlatScrewdriver:
                return StaticData.equippedFlatScrewdriver == item.itemID;
            case ItemCategory.Wrench:
                return StaticData.equippedWrench == item.itemID;
            default:
                return false;
        }
    }

    public void ShowShop()
    {
        if (shopObject != null)
            shopObject.SetActive(true);

        //ri.DisableRaycasting();

        om.orderCompletePanel.SetActive(false);
        pause.gameObject.SetActive(false);
        glm.prizeText.gameObject.SetActive(false); // Hide the prize text
        glm.prizeImej.gameObject.SetActive(false);

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



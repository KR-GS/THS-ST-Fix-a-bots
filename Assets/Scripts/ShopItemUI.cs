using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemRatings;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Button itemButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private ShopManager.ShopItem itemData;
    private Action<ShopManager.ShopItem, ShopItemUI> onItemClicked;

    public void Setup(ShopManager.ShopItem item, Action<ShopManager.ShopItem, ShopItemUI> clickCallback)
    {
        itemData = item;
        onItemClicked = clickCallback;

        // Populate UI elements
        itemImage.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemRatings.sprite = item.rating;
        itemPriceText.text = item.price.ToString();

        // Setup button click - pass 'this' to send the UI reference
        if (itemButton == null)
        {
            itemButton = GetComponent<Button>();
            if (itemButton == null)
            {
                Debug.LogError($"[ShopItemUI] Missing Button reference on {name}");
                return;
            }
        }
        
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() => onItemClicked?.Invoke(itemData, this));

        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        bool isBought = IsItemBought(itemData.itemID);
        bool isEquipped = IsItemEquipped(itemData); // Update this

        if (!isBought)
        {
            itemButton.interactable = true;
            if (buttonText != null)
                buttonText.text = $"Buy - {itemData.price}";
        }
        else if (isEquipped)
        {
            itemButton.interactable = false;
            if (buttonText != null)
                buttonText.text = "Equipped";
        }
        else
        {
            itemButton.interactable = true;
            if (buttonText != null)
                buttonText.text = "Equip";
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

    private bool IsItemEquipped(ShopManager.ShopItem item)
    {
        switch (item.category)
        {
            case ShopManager.ItemCategory.Hammer:
                return StaticData.equippedHammer == item.itemID;
            case ShopManager.ItemCategory.PhilipsScrewdriver:
                return StaticData.equippedPhilipsScrewdriver == item.itemID;
                /*
            case ShopManager.ItemCategory.FlatScrewdriver:
                return StaticData.equippedFlatScrewdriver == item.itemID;
            case ShopManager.ItemCategory.Wrench:
                return StaticData.equippedWrench == item.itemID;
                */
            default:
                return false;
        }
    }

}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ShopManager;

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
                buttonText.text = $"BUY";
        }
        else if (isEquipped)
        {
            itemButton.interactable = false;
            if (buttonText != null)
                buttonText.text = "EQUIPPED";
        }
        else
        {
            itemButton.interactable = true;
            if (buttonText != null)
                buttonText.text = "EQUIP";
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
                return StaticData.equippedPhilipsScrewdriver == item.itemID ;
            case ItemCategory.FlatScrewdriver:
                return StaticData.equippedFlatScrewdriver == item.itemID;
            case ItemCategory.Wrench:
                return StaticData.equippedWrench == item.itemID;
            default:
                return false;
        }
    }

}
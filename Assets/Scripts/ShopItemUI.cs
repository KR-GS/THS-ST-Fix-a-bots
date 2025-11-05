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
        if (itemButton == null)
        {
            Debug.LogWarning($"[ShopItemUI] itemButton is null for {itemData?.itemName ?? "Unknown Item"}");
            return;
        }

        bool isBought = IsItemBought(itemData.hammerID);
        bool isEquipped = StaticData.equippedHammer == itemData.hammerID;

        Debug.Log($"Item: {itemData.itemName}, HammerID: {itemData.hammerID}, IsBought: {isBought}, IsEquipped: {isEquipped}, EquippedHammer: {StaticData.equippedHammer}");

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
                buttonText.text = "Equipped";
        }
        else
        {
            itemButton.interactable = true;
            if (buttonText != null)
                buttonText.text = "Equip";
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
}
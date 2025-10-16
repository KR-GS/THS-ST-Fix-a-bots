using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using Unity.IO.LowLevel.Unsafe;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public string category; // "Potion", "Weapon", "Armor"
        public Sprite icon;
        public int price;
    }

    public static ShopManager Instance;

    public GameObject shopPanel;
    public List<ShopItem> allItems;
    public GameObject itemButtonPrefab;
    public Transform contentParent; // The Content of ScrollView
    public Sprite heartIcon;
    public Button exitShopButton;
    public Sprite TVSpriteNoOrder;
    public Sprite TVSpriteIP;
    public Sprite TVSpriteNO;
    private List<GameObject> currentButtons = new();

    void Start()
    {
        // Now all Inspector references are guaranteed to be assigned
        allItems = new List<ShopItem>()
        {
            new ShopItem() { itemName = "A", icon = heartIcon, category = "Tool", price = 50 },
            new ShopItem() { itemName = "V", icon = heartIcon, category = "Tool", price = 60 },
            new ShopItem() { itemName = "C", icon = heartIcon, category = "Sticker", price = 200 },
            new ShopItem() { itemName = "D", icon = heartIcon, category = "Wire", price = 300 },
            new ShopItem() { itemName = "E", icon = heartIcon, category = "Robot", price = 150 }
        };

        ShowCategory("All");

        CloseShop();
    }

    public void ShowCategory(string category)
    {
        Debug.Log($"itemButtonPrefab: {itemButtonPrefab}");
        Debug.Log($"contentParent: {contentParent}");
        Debug.Log($"allItems count: {allItems.Count}");
        // Clear old buttons
        foreach (var btn in currentButtons)
            Destroy(btn);
        currentButtons.Clear();

        // Filter
        var items = category == "All" ? allItems : allItems.Where(i => i.category == category);

        // Spawn buttons
        foreach (var item in items)
        {
            GameObject btnObj = Instantiate(itemButtonPrefab, contentParent);
            btnObj.transform.localScale = Vector3.one;

            var button = btnObj.GetComponent<Button>();
            btnObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{item.itemName} - {item.price}g";

            var img = btnObj.transform.Find("Item Image").GetComponent<Image>();
            img.sprite = item.icon;

            // Add click event
            button.onClick.AddListener(() => OnItemClicked(item));

            currentButtons.Add(btnObj);
        }
    }

    private void OnItemClicked(ShopItem item)
    {
        if(item.itemName == "A")
        {
            if(item.price > GameLoopManager.Instance.money)
            {
                Debug.Log("Not enough gold!");
                return;
            }
            else{
                Debug.Log($"SUCCESSFULLY Bought {item.itemName} for {item.price} gold!");
                //GameLoopManager.Instance.money += 50; //Base value 50
                //GameLoopManager.Instance.UpdateMoneyText();
            }
        }
        else
        {
            Debug.Log($"Bought {item.itemName} for {item.price} gold!");
        }
    }

    public void ShowShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

        ShowCategory("All");

        RaycastInteractor.Instance.DisableRaycasting();

        TimerScript.instance.StopTimer();

        GameLoopManager.Instance.moneyImage.gameObject.SetActive(true);
        GameLoopManager.Instance.dayNumber.gameObject.SetActive(false);
        GameLoopManager.Instance.moneyText.gameObject.SetActive(true);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(false);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(false);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(false);
        GameLoopManager.Instance.shopButton.gameObject.SetActive(false);

        if (RaycastInteractor.Instance.ToolIndicator != null) RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(false);
        if (RaycastInteractor.Instance.WireIndicator != null) RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(false);
        if (RaycastInteractor.Instance.PaintIndicator != null) RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(false);


        if (RaycastInteractor.Instance.timeText != null)
        {
            RaycastInteractor.Instance.timeText.gameObject.SetActive(false); // Hide the time text
        }


        exitShopButton.gameObject.SetActive(true);
        exitShopButton.onClick.RemoveAllListeners();
        exitShopButton.onClick.AddListener(() =>
        {
            CloseShop();
        });
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        GameLoopManager.Instance.dayNumber.gameObject.SetActive(true);
        GameLoopManager.Instance.tutorialButton.gameObject.SetActive(true);
        GameLoopManager.Instance.remainingOrders.gameObject.SetActive(true);
        GameLoopManager.Instance.ordersOnboard.gameObject.SetActive(true);
        GameLoopManager.Instance.ShowTV(true);
        GameLoopManager.Instance.shopButton.gameObject.SetActive(true);
        RaycastInteractor.Instance.TVSprite.gameObject.SetActive(true);

        if (StaticData.TVScreen == 0)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteNoOrder;
        }
        else if (StaticData.TVScreen == 1)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteNO;
        }
        else if (StaticData.TVScreen == 2)
        {
            RaycastInteractor.Instance.TVSprite.sprite = TVSpriteIP;
        }

        Order savedOrder = OrderManager.Instance.GetActiveOrder();

        if (StaticData.startOfDay == true)
        {
            Debug.Log("Aiya, debugging is sad!");
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(true);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(true);
        }
        else if (StaticData.startOfDay == false)
        {
            Debug.Log("Ayo, will this work?");
            RaycastInteractor.Instance.readyIndicator.gameObject.SetActive(false);
            RaycastInteractor.Instance.readyText.gameObject.SetActive(false);
        }


        if (RaycastInteractor.Instance.timeText != null)
        {
            RaycastInteractor.Instance.timeText.gameObject.SetActive(true); // Hide the time text
        }

        TimerScript.instance.StartTimer();

        RaycastInteractor.Instance.isOrderChecked = StaticData.isOrderChecked;
        Debug.Log("isOrderChecked status: " + RaycastInteractor.Instance.isOrderChecked);

        if (!RaycastInteractor.Instance.isOrderChecked && savedOrder != null)
        {

            Debug.Log("Setting indicators active...");

            if (savedOrder.needsTool)
            {
                RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(true);
                Debug.Log("ToolIndicator enabled");
            }
            if (savedOrder.needsWire)
            {
                RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(true);
                Debug.Log("WireIndicator enabled");
            }
            if (savedOrder.needsPaint)
            {
                RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(true);
                Debug.Log("PaintIndicator enabled");
            }

            RaycastInteractor.Instance.isOrderChecked = true;
            StaticData.isOrderChecked = true;
        }
        else if (RaycastInteractor.Instance.isOrderChecked && savedOrder != null)
        {
            Debug.Log("Order is checked, returning old indicators...");

            if (savedOrder.needsTool && !StaticData.isToolDone)
            {
                RaycastInteractor.Instance.ToolIndicator.gameObject.SetActive(true);
            }
            if (savedOrder.needsPaint && !StaticData.isPaintDone)
            {
                RaycastInteractor.Instance.PaintIndicator.gameObject.SetActive(true);
            }
            if (savedOrder.needsWire && !StaticData.isWireDone)
            {
                RaycastInteractor.Instance.WireIndicator.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"Indicators not set. isOrderChecked={RaycastInteractor.Instance.isOrderChecked}, currentOrder={savedOrder}");
        }
    }
}

   


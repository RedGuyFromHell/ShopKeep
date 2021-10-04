using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.Universal;

public class gameManager : MonoBehaviour
{
    //general variables
    public GameObject player;
    public static bool shopNeedsUpdating = true;
    public bool CanPlayerMove = false;
    public bool playerNeedsUpdate = true;
    public int day = 0;
    public float timeSpeed = 2f;
    public float craftingTime = 1f;
    public bool isCraftingItems = true;
    public bool isAddingProfits = true;
    public GameObject globalLight;

    //Coin Effects
    public Transform coinMinus;
    public Transform coinPlus;

    //Lighting Colors
    Color sunsetColor = new Color32(180, 100, 50, 100);
    Color nightColor = new Color32(30, 50, 160, 100);
    Color sunriseColor = new Color32(250, 200, 100, 100);

    //shop variables
    public static float money = 1000;
    public static float moneyIncrement = 0;
    public static float IncrementMultiplier = 1;
    public static float moneyMadeSoFar = money;
    public static string shopName = "nume generic";
    public static int beauty = 10;

    //shop upgrades
    public static int shop_tier = 0;
    public static int crafter_tier = 0;
    public static int counter_tier = 0;
    public static int beauty_tier = 0;
    public static int renown_tier = 0;

    //dungeon upgrades
    public static int dungeonDelving_tier = 0;
    public static int monsters_tier = 0;
    public static int loot_tier = 0;

    public static List<int2> wares = new List<int2>();
    public static List<int2> lastWares = new List<int2>();

    [Header("WareDictionary")]
    public List<ware> wareTypes = new List<ware>();

    public GameObject inventoryAndShop;

    public GameObject inputShopName;
    private void Start()
    {
        if (PlayerPrefs.GetInt("continueGame") == 1)
        {
            transform.GetComponent<SaveLoad>().LoadGame();
            PlayerPrefs.SetInt("continueGame", 0);

            inputShopName.SetActive(false);
        }

        StartCoroutine(CraftItems());
        StartCoroutine(AddProfits());

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    IEnumerator CraftItems()
    {
        while (isCraftingItems)
        {
            yield return new WaitForSeconds(10 - crafter_tier / 10);

            for (int i = 0; i <= counter_tier; i++)
            {
                InventorySlot tempSlot = InventoryManager.shopSlots[i].GetComponent<InventorySlot>();
                if (tempSlot.slotWare != null)
                {
                    tempSlot.craft();
                }
            }
        }
    }
    IEnumerator AddProfits ()
    {
        while (isAddingProfits)
        {
            money += moneyIncrement / 10f * IncrementMultiplier;

            UIManager.UINeedsUpdate = true;
            //Debug.Log(moneyIncrement + " | " + gameManager.money);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        

        if (shopNeedsUpdating)
        {
            UpdateGame();
            
            shopNeedsUpdating = false;
        }

        if (playerNeedsUpdate)
            if (CanPlayerMove)
                player.GetComponent<PlayerMovement>().enabled = true;
            else
                player.GetComponent<PlayerMovement>().enabled = false;

        #region InputChecks
        //Input Checks:
        if (Input.GetKeyDown("i") && !inputShopName.activeSelf)
            inventoryAndShop.SetActive(!inventoryAndShop.activeSelf);
        #endregion
    }
    void UpdateGame()

    {
        int unlockedSlots = 1 + counter_tier;

        for (int i = 0; i < unlockedSlots; i++)
        {
             InventoryManager.shopSlots[i].GetComponent<InventorySlot>().isLocked = false;
        }

        beauty = 10 + beauty_tier * 10 + shop_tier * 25;

        InventoryManager.inventoryNeedsUpdate = true;
    }

    public int startHour = 5;
    Color currentColor = new Color32(30, 50, 160, 100);
    public string GetTime()
    {
        int hours = startHour + (int)(Time.time * timeSpeed) / 60 - day * 24;
        if (hours == 24)
            day++;

        if (hours == 5 || hours == 6)
            currentColor = Color.Lerp(nightColor, sunriseColor, ((Time.time * timeSpeed) % 120) / 120);
        if (hours == 7)
            currentColor = Color.Lerp(sunriseColor, Color.white, ((Time.time * timeSpeed) % 60) / 60);
        if (hours == 18)
            currentColor = Color.Lerp(Color.white, sunsetColor, ((Time.time * timeSpeed) % 60) / 60);
        if (hours == 20 || hours == 21)
            currentColor = Color.Lerp(sunsetColor, nightColor, ((Time.time * timeSpeed) % 120) / 120);

        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = currentColor;
        return hours + ":" + (int)(Time.time * timeSpeed) % 60;
    }

    public void BuyItems(int3 items)
    {
        if (items.x != 0 && wares.Count < InventoryManager.inventorySlots.Length && wareTypes[items.x-1].price <= gameManager.money)
        {
            bool BuyNewCondition = true;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.x-1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    BuyNewCondition = false;
                    break;
                }
            }

            if (BuyNewCondition)
            {
                wares.Add(new int2(wareTypes[items.x-1].id, 1));
                Debug.Log("new item gets added " + items.x);
            }

            Instantiate(transform.GetComponent<gameManager>().coinMinus, new Vector2(2.5f, 0f), quaternion.identity);

            money -= wareTypes[items.x-1].price;
            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
        if (items.y != 0 && wares.Count < InventoryManager.inventorySlots.Length && wareTypes[items.y-1].price <= gameManager.money)
        {
            bool BuyNewCondition = true;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.y-1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    BuyNewCondition = false;
                    break;
                }
            }

            if (BuyNewCondition)
                wares.Add(new int2(wareTypes[items.y-1].id, 1));

            Instantiate(transform.GetComponent<gameManager>().coinMinus, new Vector2(2.5f, 0f), quaternion.identity);

            money -= wareTypes[items.y-1].price;
            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
        if (items.z != 0 && wares.Count < InventoryManager.inventorySlots.Length && wareTypes[items.z-1].price <= gameManager.money)
        {
            bool BuyNewCondition = true;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.z-1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    BuyNewCondition = false;
                    break;
                }
            }

            if (BuyNewCondition)
                wares.Add(new int2(wareTypes[items.z-1].id, 1));

            Instantiate(transform.GetComponent<gameManager>().coinMinus, new Vector2(2.5f, 0f), quaternion.identity);

            money -= wareTypes[items.z-1].price;
            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
    }

    public void AddItems(int3 items)
    {
        Debug.Log(items);

        if (items.x != 0 && wares.Count < InventoryManager.inventorySlots.Length)
        {
            bool condition = false;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.x - 1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    condition = true;
                    break;
                }
            }

            if (!condition)
            {
                wares.Add(new int2(wareTypes[items.x - 1].id, 1));
            }

            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
        if (items.y != 0 && wares.Count < InventoryManager.inventorySlots.Length)
        {
            bool condition = false;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.y - 1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    condition = true;
                    break;
                }
            }

            if (!condition)
                wares.Add(new int2(wareTypes[items.y - 1].id, 1));

            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
        if (items.z != 0 && wares.Count < InventoryManager.inventorySlots.Length)
        {
            bool condition = false;
            for (int i = 0; i < wares.Count; i++)
            {
                if (wares[i].x == wareTypes[items.z - 1].id)
                {
                    wares[i] = new int2(wares[i].x, wares[i].y + 1);
                    condition = true;
                    break;
                }
            }

            if (!condition)
                wares.Add(new int2(wareTypes[items.z - 1].id, 1));

            InventoryManager.inventoryNeedsUpdate = true;
            UIManager.UINeedsUpdate = true;
        }
    }
}


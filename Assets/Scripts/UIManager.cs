using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text timeUI;
    public Text moneyUI;
    public Text beautyAndIncrementUI;
    public Text moneyMadeSoFarUI;
    public GameObject name_DayPanel;

    public GameObject shopObject;
    public GameObject shopBody;
    public GameObject dungeonBody;

    public GameObject collapsableWindow;
    public GameObject helpOverlay;
    gameManager game_manager;

    public Image callAdvsBar;
    float manualCallAdvs = 0;
    public Image KillMonsterBar;
    float manualKillMonster = 0;

    //button price text
    public Text shopPrice;
    public Text crafterPrice;
    public Text counterPrice;
    public Text beautyPrice;
    public Text renownPrice;
    public Text monsterPrice;
    public Text dungeonPrice;
    public Text lootPrice;

    public static int shopUpgrades = 0;
    public static int dungeonUpgrades = 0;

    public static bool UINeedsUpdate = true;
    public static bool UpgradesNeedUpdate = true;

    private void Start()
    {
        game_manager = this.GetComponent<gameManager>();
        UpdateUpgrades(); //contains EnableUIElements

        timeUI.text = "0:00";
        name_DayPanel.transform.GetChild(0).GetComponent<Text>().text = gameManager.shopName.ToString();
        name_DayPanel.transform.GetChild(1).GetComponent<Text>().text = gameObject.GetComponent<gameManager>().day.ToString();
        moneyUI.text = gameManager.money.ToString();
        beautyAndIncrementUI.text = "Beauty: " + gameManager.beauty.ToString() + "  |  Increment: " + gameManager.moneyIncrement.ToString("F2");
        moneyMadeSoFarUI.text = "Money Made So Far: " + gameManager.moneyMadeSoFar.ToString();

        callAdvsBar.fillAmount = manualCallAdvs / 100;
        KillMonsterBar.fillAmount = manualKillMonster / 1000;

        LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
    }

    void EnableUIElements ()
    {
        //Shop Upgrades:
        for (int i = 0; i < shopBody.transform.childCount; i++)
        {
            shopBody.transform.GetChild(i).gameObject.SetActive(false);
        }
        shopBody.transform.GetChild(0).gameObject.SetActive(true);
        shopBody.transform.GetChild(1).gameObject.SetActive(true);

        if (shopUpgrades >= 0 && gameManager.moneyMadeSoFar >= 5000 && gameManager.crafter_tier >= 25)
        {
            shopBody.transform.GetChild(2).gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
        }
        if (shopUpgrades >= 1 && gameManager.moneyMadeSoFar >= 25000 && gameManager.counter_tier >= 4)
        {
            shopBody.transform.GetChild(3).gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
        }
        if (shopUpgrades >= 2 && gameManager.moneyMadeSoFar >= 100000 && gameManager.beauty_tier >= 25)
        {
            shopBody.transform.GetChild(4).gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
        }

        //Dungeon Upgrades:
        for (int i = 0; i < dungeonBody.transform.childCount; i++)
        {
            dungeonBody.transform.GetChild(i).gameObject.SetActive(false);
        }
        dungeonBody.transform.GetChild(0).gameObject.SetActive(true);

        if (dungeonUpgrades >= 0 && gameManager.moneyMadeSoFar >= 2000 && gameManager.monsters_tier >= 25)
        {
            dungeonBody.transform.GetChild(1).gameObject.SetActive(true);
            enableKillMonsterButton();
            LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
        }
        if (dungeonUpgrades >= 1 && gameManager.moneyMadeSoFar >= 10000 && gameManager.dungeonDelving_tier >= 25)
        {
            dungeonBody.transform.GetChild(2).gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());
        }

        UINeedsUpdate = true;
        UpgradesNeedUpdate = true;
    }

    private void Update()
    {
        timeUI.text = gameObject.GetComponent<gameManager>().GetTime();

        //Input Checks:
        if (Input.GetKeyDown(KeyCode.F1))
            ToggleHelpOverlay();

        if (UINeedsUpdate || gameManager.shopNeedsUpdating)
        {
            UpdateUI();
            UINeedsUpdate = false;
            gameManager.shopNeedsUpdating = false;
        }
        if (UpgradesNeedUpdate || SaveLoad.gameHasLoaded)
        {
            UpdateUpgrades(); //contains EnableUIElements
            UpgradesNeedUpdate = false;
            SaveLoad.gameHasLoaded = false;
        }
    }

    void ToggleHelpOverlay ()
    {
        helpOverlay.SetActive(!helpOverlay.activeSelf);
        Debug.Log("Help overlay toggled: " + helpOverlay.activeSelf);
    }

    void enableKillMonsterButton()
    {
        GameObject.Find("Manual Press Panel").transform.GetChild(1).gameObject.SetActive(true);
    }

    public void UpdateUI ()
    {
        name_DayPanel.transform.GetChild(0).GetComponent<Text>().text = gameManager.shopName;
        name_DayPanel.transform.GetChild(1).GetComponent<Text>().text ="Day " + gameObject.GetComponent<gameManager>().day;
        moneyUI.text = Mathf.FloorToInt(gameManager.money).ToString();
        beautyAndIncrementUI.text = "Beauty: " + gameManager.beauty.ToString() + "  |  Increment: " + gameManager.moneyIncrement.ToString("F2");
        moneyMadeSoFarUI.text = "Money Made So Far: " + gameManager.moneyMadeSoFar.ToString();
}

    public void UpdateUpgrades ()
    {
        //Debug.Log(shopBody.transform.GetChild(0).gameObject.activeSelf);
        shopBody.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Shop Tier: " + gameManager.shop_tier;
        float price1 = 1000 * Mathf.Pow(4f, gameManager.shop_tier);
        shopPrice.text = " + " + ((int)price1).ToString();
        shopBody.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "Crafter Tier: " + gameManager.crafter_tier;
        float price2 = 15f * Mathf.Pow(1.15f, gameManager.crafter_tier);
        crafterPrice.text = " + " + ((int)price2).ToString();
        shopBody.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = "Counter Tier: " + gameManager.counter_tier;
        float price3 = 100 * Mathf.Pow(1.15f, gameManager.counter_tier);
        counterPrice.text = " + " + ((int)price3).ToString();
        shopBody.transform.GetChild(3).transform.GetChild(1).GetComponent<Text>().text = "Beauty Tier: " + gameManager.beauty_tier;
        float price4 = 1100 * Mathf.Pow(1.15f, gameManager.beauty_tier);
        beautyPrice.text = " + " + ((int)price4).ToString();
        shopBody.transform.GetChild(4).transform.GetChild(1).GetComponent<Text>().text = "Renown Tier: " + gameManager.renown_tier;
        float price5 = 12000 * Mathf.Pow(1.15f, gameManager.renown_tier);
        renownPrice.text = " + " + ((int)price5).ToString();

        dungeonBody.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Monsters Tier: " + gameManager.monsters_tier;
        float price6 = 130000 * Mathf.Pow(1.15f, gameManager.monsters_tier);
        monsterPrice.text = " + " + ((int)price6).ToString();
        dungeonBody.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "Delving Tier: " + gameManager.dungeonDelving_tier;
        float price7 = 1400000 * Mathf.Pow(1.15f, gameManager.dungeonDelving_tier);
        dungeonPrice.text = " + " + ((int)price7).ToString();
        dungeonBody.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = "Loot Tier: " + gameManager.loot_tier;
        float price8 = 20000000 * Mathf.Pow(1.15f, gameManager.dungeonDelving_tier);
        lootPrice.text = " + " + ((int)price8).ToString();

        EnableUIElements();
    }

    //public InputField inputShopName;
    public void SetShopName (string name)
    {
        gameManager.shopName = name;
        UINeedsUpdate = true;
    }

    //Manual Click Buttons:
    public void CallAdventurersButton()
    {
        manualCallAdvs += 1 + gameManager.renown_tier / 10f;

        callAdvsBar.fillAmount = manualCallAdvs / 100;

        if (manualCallAdvs >= 100)
        {
            manualCallAdvs = 0;
            GetComponent<AdventurersManager>().SpawnAdventurer();
            callAdvsBar.fillAmount = 0;
        }
    }
    public void KillMonsterButton()
    {
        manualKillMonster += 20 + gameManager.dungeonDelving_tier;

        KillMonsterBar.fillAmount = manualKillMonster / 1000;

        if (manualKillMonster >= 1000)
        {
            manualKillMonster = 0;

            int newItem = UnityEngine.Random.Range(1, 2 + gameManager.loot_tier);
            int randomItem = UnityEngine.Random.Range(1, 101);
            if (randomItem >= 20 && randomItem < 70)
                game_manager.AddItems(new int3(newItem, 0, 0));
            else if (randomItem >= 70 && randomItem < 90 && gameManager.loot_tier >= 3)
                game_manager.AddItems(new int3(newItem, newItem, 0));
            else if (randomItem >= 90 && gameManager.loot_tier >= 5)
                game_manager.AddItems(new int3(newItem, newItem, newItem));
            else
                game_manager.AddItems(new int3(0, 0, 0));

            KillMonsterBar.fillAmount = 0;
        }
    }

    #region ShopUpgrades
    //Shop Upgrades: 
    public void PressShopButton(int increment)
    {
        float price = 1000 * Mathf.Pow(4f, gameManager.shop_tier);
        shopPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.shop_tier < 10)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.shop_tier += increment;
            gameManager.IncrementMultiplier += 0.1f;

            shopBody.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Shop Tier: " + gameManager.shop_tier;
            shopObject.GetComponent<shopPrefab>().UpgradeShop(gameManager.shop_tier);

            gameManager.shopNeedsUpdating = true;
            UINeedsUpdate = true;
        }
    }
    public void PressCrafterButton(int increment)
    {
        float price = 15f * Mathf.Pow(1.15f, gameManager.crafter_tier);
        crafterPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.crafter_tier < 101)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 0.1f;

            gameManager.crafter_tier += increment;
            shopBody.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "Crafter Tier: " + gameManager.crafter_tier;

            if (shopUpgrades == 0 && gameManager.moneyMadeSoFar >= 5000 && gameManager.crafter_tier == 25)
            {
                shopBody.transform.GetChild(2).gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());

                shopUpgrades++;
            }

            gameManager.shopNeedsUpdating = true;
            UINeedsUpdate = true;
        }
    }
    public void PressCounterButton(int increment)
    {
        float price = 100 * Mathf.Pow(1.15f, gameManager.counter_tier);
        counterPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.counter_tier < 5)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 1f;

            gameManager.counter_tier += increment;
            shopBody.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = "Counter Tier: " + gameManager.counter_tier;

            if (shopUpgrades == 1 && gameManager.moneyMadeSoFar >= 25000 && gameManager.counter_tier == 4)
            {
                shopBody.transform.GetChild(3).gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());

                shopUpgrades++;
            }

            gameManager.shopNeedsUpdating = true;
            UINeedsUpdate = true;
        }
    }
    public void PressBeautyButton(int increment)
    {
        float price = 1100 * Mathf.Pow(1.15f, gameManager.beauty_tier);
        beautyPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.beauty_tier < 101)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 8f;

            gameManager.beauty_tier += increment;
            shopBody.transform.GetChild(3).transform.GetChild(1).GetComponent<Text>().text = "Beauty Tier: " + gameManager.beauty_tier;

            if (shopUpgrades == 2 && gameManager.moneyMadeSoFar >= 100000 && gameManager.beauty_tier == 25)
            {
                shopBody.transform.GetChild(4).gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());

                shopUpgrades++;
            }

            UINeedsUpdate = true;
        }
    }
    public void PressRenownButton(int increment)
    {
        float price = 12000 * Mathf.Pow(1.15f, gameManager.renown_tier);
        renownPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.renown_tier < 101)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 47f;

            gameManager.renown_tier += increment;
            shopBody.transform.GetChild(4).transform.GetChild(1).GetComponent<Text>().text = "Renown Tier: " + gameManager.renown_tier;

            UINeedsUpdate = true;
        }
    }
    #endregion

    #region DungeonUpgrades
    //Dungeon Upgrades:
    public void PressMonstersButton (int increment)
    {
        float price = 130000 * Mathf.Pow(1.15f, gameManager.monsters_tier);
        monsterPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.monsters_tier < 101)
        {
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 260f;

            gameManager.monsters_tier += increment;
            dungeonBody.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Monsters Tier: " + gameManager.monsters_tier;

            if (dungeonUpgrades == 0 && gameManager.moneyMadeSoFar >= 2000 && gameManager.monsters_tier == 25)
            {
                dungeonBody.transform.GetChild(1).gameObject.SetActive(true);
                enableKillMonsterButton();

                LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());

                dungeonUpgrades++;
            }

            UINeedsUpdate = true;
        }
    }
    public void PressDungeonDelvingButton(int increment)
    {
        float price = 1400000 * Mathf.Pow(1.15f, gameManager.dungeonDelving_tier);
        dungeonPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.dungeonDelving_tier < 101)
        {
            gameManager.dungeonDelving_tier += increment;
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 1400f;

            dungeonBody.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "Delving Tier: " + gameManager.dungeonDelving_tier;
            if (dungeonUpgrades == 1 && gameManager.moneyMadeSoFar >= 10000 && gameManager.dungeonDelving_tier == 25)
            {
                dungeonBody.transform.GetChild(2).gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(collapsableWindow.GetComponent<RectTransform>());

                dungeonUpgrades++;
            }

            UINeedsUpdate = true;
        }
    }
    public void PressLootButton (int increment)
    {
        float price = 20000000 * Mathf.Pow(1.15f, gameManager.dungeonDelving_tier);
        lootPrice.text = " + " + ((int)price).ToString();
        if (gameManager.money >= price && gameManager.loot_tier < 7)
        {
            gameManager.loot_tier += increment;
            gameManager.money -= Mathf.FloorToInt(price);
            gameManager.moneyIncrement += 7800f;

            dungeonBody.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = "Loot Tier: " + gameManager.loot_tier;

            UINeedsUpdate = true;

            dungeonUpgrades++;
        }
    }
    #endregion
}

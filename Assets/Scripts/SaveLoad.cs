using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SaveLoad : MonoBehaviour
{
    gameManager game_manager;
    AdventurersManager adv_manager;
    UIManager ui_manager;
    InventoryManager inventory_manager;

    public static bool gameHasLoaded = false;

    private void Start()
    {
        game_manager = this.GetComponent<gameManager>();
        adv_manager = this.GetComponent<AdventurersManager>();
        ui_manager = this.GetComponent<UIManager>();
        //inventory_manager = inventory.GetComponent<InventoryManager>();
    }

    public void SaveGame ()
    {
        PlayerPrefs.SetInt("gameHasSaved", 1);

        #region GameManager

        //General Variables:
        PlayerPrefs.SetString("shopName", gameManager.shopName);
        PlayerPrefs.SetFloat("money", gameManager.money);
        PlayerPrefs.SetFloat("moneyMadeSoFar", gameManager.moneyMadeSoFar);
        PlayerPrefs.SetFloat("moneyIncrement", gameManager.moneyIncrement);
        PlayerPrefs.SetFloat("IncrementMultiplier", gameManager.IncrementMultiplier);
        PlayerPrefs.SetInt("beauty", gameManager.beauty);

        for (int i = 0; i < gameManager.wares.Count; i++)
        {
            PlayerPrefs.SetInt("waresX" + i, gameManager.wares[i].x);
            PlayerPrefs.SetInt("waresY" + i, gameManager.wares[i].y);
        }
        for (int i = 0; i < gameManager.lastWares.Count; i++)
        {
            PlayerPrefs.SetInt("lastWaresX" + i, gameManager.lastWares[i].x);
            PlayerPrefs.SetInt("lastWaresY" + i, gameManager.lastWares[i].y);
        }

        //Tiers:
        PlayerPrefs.SetInt("shopTier", gameManager.shop_tier);
        PlayerPrefs.SetInt("crafterTier", gameManager.crafter_tier);
        PlayerPrefs.SetInt("counterTier", gameManager.counter_tier);
        PlayerPrefs.SetInt("beautyTier", gameManager.beauty_tier);
        PlayerPrefs.SetInt("renownTier", gameManager.renown_tier);

        PlayerPrefs.SetInt("dungeonDelvingTier", gameManager.dungeonDelving_tier);
        PlayerPrefs.SetInt("monstersTier", gameManager.monsters_tier);
        PlayerPrefs.SetInt("lootTier", gameManager.loot_tier);
        #endregion

        #region UIManager

        PlayerPrefs.SetInt("shopUpgrades", UIManager.shopUpgrades);
        PlayerPrefs.SetInt("dungeonUpgrades", UIManager.dungeonUpgrades);

        #endregion

        #region Inventory

        //PlayerPrefs.SetInt("InventorySize", gameManager.wares.Count);
        //for (int i = 0; i < gameManager.wares.Count; i++)
        //{
        //    PlayerPrefs.SetInt("invX" + i, gameManager.wares[i].x);
        //    PlayerPrefs.SetInt("invY" + i, gameManager.wares[i].y);
        //}

        #endregion
    }

    //public static List<int2> loadedWares = new List<int2>();

    public void LoadGame ()
    {
        #region GameManager

        //General Variables:
        gameManager.shopName = PlayerPrefs.GetString("shopName");
        gameManager.money = PlayerPrefs.GetFloat("money");
        gameManager.moneyMadeSoFar = PlayerPrefs.GetFloat("moneyMadeSoFar");
        gameManager.moneyIncrement = PlayerPrefs.GetFloat("moneyIncrement");
        gameManager.IncrementMultiplier = PlayerPrefs.GetFloat("IncrementMultiplier");
        gameManager.beauty = PlayerPrefs.GetInt("beauty");

        for (int i = 0; i < gameManager.wares.Count; i++)
        {
            gameManager.wares[i] = new int2(PlayerPrefs.GetInt("waresX" + i), PlayerPrefs.GetInt("waresY" + i));
        }
        for (int i = 0; i < gameManager.lastWares.Count; i++)
        {
            gameManager.lastWares[i] = new int2(PlayerPrefs.GetInt("lastWaresX" + i), PlayerPrefs.GetInt("lastWaresY" + i));
        }

        //Tiers:
        gameManager.shop_tier = PlayerPrefs.GetInt("shopTier");
        gameManager.crafter_tier = PlayerPrefs.GetInt("crafterTier");
        gameManager.counter_tier = PlayerPrefs.GetInt("counterTier");
        gameManager.beauty_tier = PlayerPrefs.GetInt("beautyTier");
        gameManager.renown_tier = PlayerPrefs.GetInt("renownTier");

        gameManager.dungeonDelving_tier = PlayerPrefs.GetInt("dungeonDelvingTier");
        gameManager.monsters_tier = PlayerPrefs.GetInt("monstersTier");
        gameManager.loot_tier = PlayerPrefs.GetInt("lootTier");
        #endregion

        #region UIManager

        UIManager.shopUpgrades = PlayerPrefs.GetInt("shopUpgrades");
        UIManager.dungeonUpgrades = PlayerPrefs.GetInt("dungeonUpgrades");

        #endregion

        #region Inventory

        //loadedWares.Clear();
        //for (int i = 0; i < PlayerPrefs.GetInt("InventorySize"); i++)
        //{
        //    loadedWares.Add(new int2(PlayerPrefs.GetInt("InvX" + i), PlayerPrefs.GetInt("InvY" + i)));
        //}
        //InventoryManager.inventoryNeedsUpdate = true;
        //Debug.Log(InventoryManager.itemsToTakeFromInventory.Count + " | " + PlayerPrefs.GetInt("InventorySize"));

        #endregion

        //Enable Update Checks:
        gameHasLoaded = true;
        gameManager.shopNeedsUpdating = true;
        UIManager.UINeedsUpdate = true;
        UIManager.UpgradesNeedUpdate = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("game has been saved");
            SaveGame();
        }
        //else if (Input.GetKeyDown("l"))
        //{
        //    Debug.Log("game has been loaded");
        //    LoadGame();
        //}
    }
}

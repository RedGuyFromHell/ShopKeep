using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour
{
    public static List<GameObject> shopSlots = new List<GameObject>();
    public static List<GameObject> productSlots = new List<GameObject>();
    public static GameObject[] inventorySlots = new GameObject[12];

    public gameManager manager;

    [Header("Products List")]
    public List<ware> products = new List<ware>();

    [Header("Recipes List")]
    public List<List<ware>> recipes = new List<List<ware>>();

    GameObject inventory;
    GameObject shop;
    GameObject sellProducts;

    public static List<int2> itemsToTakeFromInventory = new List<int2>();
    public static bool inventoryNeedsUpdate = true;

    private void Start()
    {
        inventory = transform.GetChild(0).gameObject;
        shop = transform.GetChild(1).gameObject;
        sellProducts = transform.GetChild(2).gameObject;

        InitShop();
        InitInventory();

        UpdateShop();
        UpdateInventory();

        this.gameObject.SetActive(false);

        gameManager.lastWares = new List<int2>(gameManager.wares);
    }

    void InitShop()
    {
        for (int i = 0; i < shop.transform.childCount; i++)
        {
            if (shop.transform.GetChild(i).CompareTag("Shop Slot"))
                shopSlots.Add(shop.transform.GetChild(i).gameObject);
            shopSlots[i].GetComponent<InventorySlot>().isLocked = true;
        }

        for (int i = 0; i < sellProducts.transform.childCount; i++)
        {
            if (sellProducts.transform.GetChild(i).CompareTag("Sell Ware Slot"))
                productSlots.Add(sellProducts.transform.GetChild(i).gameObject);

            InventorySlot tempSlot = productSlots[i].GetComponent<InventorySlot>();

            tempSlot.slotWare = products[i];
            tempSlot.icon = tempSlot.slotWare.icon;
            tempSlot.transform.GetChild(0).GetComponent<Image>().sprite = tempSlot.icon;

            tempSlot.transform.GetChild(0).gameObject.SetActive(true);
            tempSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void SetInventory (List<int2> newInventory)
    {
        gameManager.wares.Clear();
        gameManager.wares.AddRange(newInventory);

        inventoryNeedsUpdate = true;
    }

    void InitInventory()
    {
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            if (inventory.transform.GetChild(i).CompareTag("Inventory Slot"))
                inventorySlots[i] = inventory.transform.GetChild(i).gameObject;
        }
    }

    private void LateUpdate()
    {
        if (inventoryNeedsUpdate)
        {
            UpdateInventory();
            UpdateShop();
        }
    }

    void UpdateShop()
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            InventorySlot tempSlot = shopSlots[i].GetComponent<InventorySlot>();

            if (tempSlot.slotWare != null)
                tempSlot.transform.GetChild(2).gameObject.SetActive(true);
            else
                tempSlot.transform.GetChild(2).gameObject.SetActive(false);


            if (tempSlot.isLocked == false)
            {
                shopSlots[i].transform.GetChild(3).gameObject.SetActive(false);
                shopSlots[i].transform.GetChild(4).gameObject.SetActive(true);

                if (tempSlot.slotWare != null)
                {
                    tempSlot.icon = tempSlot.slotWare.icon;
                    tempSlot.transform.GetChild(0).GetComponent<Image>().sprite = tempSlot.icon;

                    tempSlot.transform.GetChild(0).gameObject.SetActive(true);
                    tempSlot.transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            tempSlot.transform.GetChild(2).GetComponent<Text>().text = tempSlot.stackCount.ToString();
        }


        if (productSlots.Count != sellProducts.transform.childCount)
        {
            for (int i = 0; i < sellProducts.transform.childCount; i++)
            {
                if (sellProducts.transform.GetChild(i).CompareTag("Sell Ware Slot"))
                    productSlots.Add(sellProducts.transform.GetChild(i).gameObject);

                InventorySlot tempSlot = productSlots[i].GetComponent<InventorySlot>();

                tempSlot.slotWare = products[i];
                tempSlot.icon = tempSlot.slotWare.icon;
                tempSlot.transform.GetChild(0).GetComponent<Image>().sprite = tempSlot.icon;
            }
        }
    }

    void UpdateInventory()
    {
        List<int2> itemsToAddToInventory = new List<int2>();
        for (int k = 0; k < gameManager.wares.Count; k++)
        {
            bool condition = false;
            for (int j = 0; j < gameManager.lastWares.Count; j++)
            {
                if (gameManager.wares[k].x == gameManager.lastWares[j].x)
                    condition = true;
            }

            if (!condition)
                itemsToAddToInventory.Add(new int2(gameManager.wares[k].x, gameManager.wares[k].y));
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot tempSlot = inventorySlots[i].GetComponent<InventorySlot>();

            if (tempSlot.slotWare != null)
            {
                if (gameManager.wares.FirstOrDefault(item => item.x == tempSlot.slotWare.id).x != 0)
                {
                    tempSlot.stackCount = gameManager.wares.First(item => item.x == tempSlot.slotWare.id).y;
                    tempSlot.transform.GetChild(2).GetComponent<Text>().text = tempSlot.stackCount.ToString();

                    if (tempSlot.stackCount > 1)
                        tempSlot.transform.GetChild(2).gameObject.SetActive(true);
                    else if (tempSlot.stackCount == 1)
                        tempSlot.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    {
                        Debug.Log("You fucked up something");
                        tempSlot.slotWare = null;
                        gameManager.wares.Remove(gameManager.wares.FirstOrDefault(item => item.x == tempSlot.slotWare.id));

                        tempSlot.transform.GetChild(0).gameObject.SetActive(false);
                        tempSlot.transform.GetChild(1).gameObject.SetActive(false);
                        tempSlot.transform.GetChild(2).gameObject.SetActive(false);
                    }
                }
                else
                {
                    tempSlot.slotWare = null;

                    tempSlot.transform.GetChild(0).gameObject.SetActive(false);
                    tempSlot.transform.GetChild(1).gameObject.SetActive(false);
                    tempSlot.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else if (itemsToAddToInventory.Count > 0)
            {
                tempSlot.slotWare = manager.wareTypes[itemsToAddToInventory[0].x - 1];
                tempSlot.stackCount = itemsToAddToInventory[0].y;

                tempSlot.icon = manager.wareTypes[itemsToAddToInventory[0].x - 1].icon;
                tempSlot.transform.GetChild(0).GetComponent<Image>().sprite = tempSlot.icon;

                tempSlot.transform.GetChild(0).gameObject.SetActive(true);
                tempSlot.transform.GetChild(1).gameObject.SetActive(true);

                itemsToAddToInventory.RemoveAt(0);
            }
        }

        gameManager.lastWares = new List<int2>(gameManager.wares);
        inventoryNeedsUpdate = false;
    }
}


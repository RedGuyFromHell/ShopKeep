using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public ware slotWare;
    public Sprite icon;

    public static InventorySlot lastPressedSlot;

    public bool isLocked = false;

    public int stackCount = 0;

    public void OpenSaleWarePanel()
    {
        this.transform.GetChild(4).gameObject.SetActive(true);

        foreach(GameObject slot in InventoryManager.shopSlots)
        {
            slot.transform.GetChild(5).gameObject.SetActive(false);
        }

        Vector3 offset = new Vector3(this.transform.parent.parent.GetChild(2).transform.childCount * 38, 100, 0);

        this.transform.parent.parent.GetChild(2).gameObject.transform.position = this.transform.position + offset;
        this.transform.parent.parent.GetChild(2).gameObject.SetActive(true);

        lastPressedSlot = this;
    }

    public void CloseSaleWarePanel()
    {
        this.transform.GetChild(5).gameObject.SetActive(false);
        this.transform.parent.parent.GetChild(2).gameObject.SetActive(false);
    }

    public void PutWareForSale()
    {
        lastPressedSlot.slotWare = this.slotWare;
        lastPressedSlot.stackCount = 0;
        lastPressedSlot.transform.GetChild(0).gameObject.SetActive(true);

        InventoryManager.inventoryNeedsUpdate = true;
    }

    public void craft()
    {
        List<ware> itemsFound = new List<ware>();
        List<int> itemsToTake = new List<int>();

        foreach (ware ware in this.slotWare.recipe.ingredients)
        {
            for (int i = 0; i < gameManager.wares.Count; i++)
            {
                if (gameManager.wares[i].x == ware.id && gameManager.wares[i].y > 0)
                {
                    itemsFound.Add(ware);
                    itemsToTake.Add(i);
                }
            }
        }

        if (this.slotWare.recipe.ingredients.SequenceEqual(itemsFound))
        {
            foreach (int index in itemsToTake)
            {
                if (gameManager.wares[index].y == 1)
                    gameManager.wares.RemoveAt(index);
                else
                    gameManager.wares[index] = new int2(gameManager.wares[index].x, gameManager.wares[index].y - 1);
            }

            this.stackCount++;

            InventoryManager.inventoryNeedsUpdate = true;
            Debug.Log("s-a craftat");
        }
        else
            Debug.Log("Nu aveti materiale");
    }
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class recipe : ScriptableObject
{
    public ware product;
    public string name = "default recipe";
    public List<ware> ingredients = new List<ware>();

    public int id;
}

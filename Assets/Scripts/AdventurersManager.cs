using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine.UI;

public class AdventurersManager : MonoBehaviour
{
    public List<Transform> adventurerPrefab = new List<Transform>();
    List<Adventurer> advList = new List<Adventurer>();
    List<Adventurer> advToRemove = new List<Adventurer>();
    List<Adventurer> advToOffer = new List<Adventurer>();

    public Transform spawnPoint;

    public class Adventurer
    {
        public Transform transform;
        public float moveX = UnityEngine.Random.Range(-1f, -0.3f);
        public int money = 0;
        public int classType = 0;
        public int3 loot = new int3(1, 1, 1);

        public bool boughtItems = false;
    }

    private void Start()
    {
        Invoke("RandomSpawnAdventurer", 5f);
    }

    private void Update()
    {
        MovingAdventurers();
    }

    void OfferLoot(Adventurer adv)
    {
        //offers loot to player
        this.transform.GetComponent<gameManager>().BuyItems(adv.loot);
        advList[advList.IndexOf(adv)].loot = new int3(0, 0, 0);
    }

    void MovingAdventurers ()
    {
        NativeArray<float3> pawnsPos = new NativeArray<float3>(advList.Count, Allocator.TempJob);
        NativeArray<float> pawnSpeeds = new NativeArray<float>(advList.Count, Allocator.TempJob);
        NativeArray<bool> hasFlipped = new NativeArray<bool>(advList.Count, Allocator.TempJob);
        for (int i = 0; i < advList.Count; i++)
        {
            pawnsPos[i] = advList[i].transform.position;
            pawnSpeeds[i] = advList[i].moveX;
            hasFlipped[i] = false;
        }

        MoveAdventurers spawn = new MoveAdventurers
        {
            deltaTime = Time.deltaTime,
            advPos = pawnsPos,
            advSpeeds = pawnSpeeds,
            advflipped = hasFlipped,
        };

        JobHandle myHandle = spawn.Schedule(advList.Count, 5);
        myHandle.Complete();

        for (int i = advList.Count - 1; i >= 0; i--)
        {
            advList[i].transform.position = pawnsPos[i];
            advList[i].moveX = pawnSpeeds[i];

            if (hasFlipped[i] == true)
            {
                Vector3 newScale = advList[i].transform.localScale;
                newScale.x *= -math.sign(advList[i].moveX);
                advList[i].transform.localScale = newScale;

                //Debug.Log("it has flipped");

                hasFlipped[i] = false;
            }

            if (advList[i].transform.position.x >= 2 && 
                advList[i].transform.position.x <= 2.02f && 
                advList[i].moveX > 0 && advList[i].loot.x != 0) //NPCs sell items
            {
                OfferLoot(advList[i]);
            }
            else if (advList[i].transform.position.x >= 2 && 
                advList[i].transform.position.x <= 2.02f && 
                advList[i].moveX < 0 && !advList[i].boughtItems) //NPCs buy items
            {
                int itemBuy;
                int randomBuy = UnityEngine.Random.Range(1, 101);
                if (randomBuy < 50 - gameManager.beauty_tier)
                    itemBuy = 0;
                else if (randomBuy >= 50 - gameManager.beauty_tier && randomBuy < 85 - gameManager.beauty_tier)
                    itemBuy = 1;
                else if (randomBuy >= 85 - gameManager.beauty_tier && randomBuy < 110 - gameManager.beauty_tier)
                    itemBuy = 2;
                else if (randomBuy >= 110 - gameManager.beauty_tier && randomBuy < 125 - gameManager.beauty_tier)
                    itemBuy = 3;
                else
                    itemBuy = 4;

                InventorySlot tempSlot = InventoryManager.shopSlots[itemBuy].GetComponent<InventorySlot>();
                if (tempSlot.isLocked == false && tempSlot.stackCount > 0)
                {
                    tempSlot.stackCount--;
                    gameManager.money += tempSlot.slotWare.price;

                    Instantiate(transform.GetComponent<gameManager>().coinPlus, new Vector2(2.5f, 0f), quaternion.identity);
                    InventoryManager.inventoryNeedsUpdate = true;
                }

                advList[i].boughtItems = true;
                
            }
            else if (advList[i].transform.position.x <= -9.5f) //Loots is calculated
            {
                int newItem = UnityEngine.Random.Range(1, 2 + gameManager.loot_tier);
                int randomItemChance = UnityEngine.Random.Range(1, 51 + gameManager.monsters_tier);
                if (randomItemChance >= 20 && randomItemChance < 70)
                    advList[i].loot = new int3(newItem, 0, 0);
                else if (randomItemChance >= 70 && randomItemChance < 90 && gameManager.loot_tier >= 3)
                    advList[i].loot = new int3(newItem, newItem, 0);
                else if (randomItemChance >= 90 && gameManager.renown_tier >= 5)
                    advList[i].loot = new int3(newItem, newItem, newItem);
                else
                    advList[i].loot = new int3(0, 0, 0);

                Debug.Log(advList[i].loot);
            }
            else if (advList[i].transform.position.x >= spawnPoint.position.x) //NPCs are destroyed
            {
                Destroy(advList[i].transform.gameObject);
                advList.RemoveAt(i);
            }
        }

        pawnsPos.Dispose();
        pawnSpeeds.Dispose();
        hasFlipped.Dispose();
    }

    void RandomSpawnAdventurer()
    {
        float advScale = UnityEngine.Random.Range(0.0f, 1.0f) + 0.5f;
        Transform tempTransform = Instantiate(adventurerPrefab[0], 
            new Vector3(spawnPoint.position.x, spawnPoint.position.y + advScale / 1.65f, spawnPoint.position.z), 
            Quaternion.identity
            );
        Vector3 newScale = tempTransform.localScale;
        newScale.x *= -1;

        advList.Add(new Adventurer
        {
            transform = tempTransform
        });
        advList[advList.Count - 1].transform.localScale = newScale * advScale;

        float timeSpan = 120 - (gameObject.GetComponent<gameManager>().day + 1) / 10 - gameManager.beauty / 100 - gameManager.renown_tier / 20;

        Invoke("RandomSpawnAdventurer", UnityEngine.Random.Range(timeSpan, timeSpan + 30));
    }

    public void SpawnAdventurer()
    {
        float advScale = UnityEngine.Random.Range(0.0f, 1.0f) + 0.5f;
        Transform tempTransform = Instantiate(adventurerPrefab[0], new Vector3(6, -1.52f + advScale / 1.65f, -6f), Quaternion.identity);
        Vector3 newScale = tempTransform.localScale;
        newScale.x *= -1;

        advList.Add(new Adventurer
        {
            transform = tempTransform
        });
        advList[advList.Count - 1].transform.localScale = newScale * advScale;
    }
}

[BurstCompile]
public struct MoveAdventurers : IJobParallelFor
{

    public NativeArray<float3> advPos;
    public NativeArray<float> advSpeeds;
    public NativeArray<bool> advflipped;
    public float deltaTime;

    public uint baseSeed;

    public void Execute(int index)
    {
        var seed = (uint)(baseSeed + index + 1);
        var rand = new Unity.Mathematics.Random(seed);
        advPos[index] += new float3(2 * advSpeeds[index] * deltaTime, 0, 0);

        if (advPos[index].x <= -9.6f && advflipped[index] == false)
        {
            advSpeeds[index] = -advSpeeds[index];
            advflipped[index] = true;
        }
    }
}

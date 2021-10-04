using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopPrefab : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> shopPrefabs;

    private void Start()
    {
        prefab = Instantiate(shopPrefabs[0], transform.position, Quaternion.identity);
    }

    public void UpgradeShop (int index)
    {
        prefab = Instantiate(shopPrefabs[index], transform.position, Quaternion.identity);
    }
}

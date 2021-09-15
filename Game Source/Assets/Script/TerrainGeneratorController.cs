using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Templates")]
    private Dictionary<string, List<GameObject>> pool;
    public List<TerainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;
    [Header("Force Early Template")]
    public List<TerainTemplateController> earlyTerrainTemplates;

    private const float debugLineHeight = 10.0f;
    
    private List<GameObject> spawnedTerrain;

    private float lastGeneratedPositionX;
    
    private float lastRemovedPositionX;
    private float GetHorizontalPositionStart()
{
    return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
}

private float GetHorizontalPositionEnd()
{
    return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
}

    void Start()
    {
        pool = new Dictionary<string, List<GameObject>>();

        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;


        foreach (TerainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }


        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

private void GenerateTerrain(float posX, TerainTemplateController forceterrain = null)
{
    GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);

    newTerrain.transform.position = new Vector2(posX, 0f);

    spawnedTerrain.Add(newTerrain);
}
    private GameObject GenerateFromPool(GameObject item, Transform parent)
{
    if (pool.ContainsKey(item.name))
    {
        // if item available in pool
        if (pool[item.name].Count > 0)
        {
            GameObject newItemFromPool = pool[item.name][0];
            pool[item.name].Remove(newItemFromPool);
            newItemFromPool.SetActive(true);
            return newItemFromPool;
        }
    }
    else
    {
        // if item list not defined, create new one
        pool.Add(item.name, new List<GameObject>());
    }


    // create new one if no item available in pool
    GameObject newItem = Instantiate(item, parent);
    newItem.name = item.name;
    return newItem;
}
    private void ReturnToPool(GameObject item)
{
    if (!pool.ContainsKey(item.name))
    {
        Debug.LogError("INVALID POOL ITEM!!");
    }


    pool[item.name].Add(item);
    item.SetActive(false);
}
}

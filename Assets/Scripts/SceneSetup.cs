using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneSetup : MonoBehaviour
{
    public List<GameObject> inside, outside;
    [SerializeField]
    Transform inCup, outCup;
    readonly List<(float x, float y)> posOutside = new() {(-3f,0f),(-1f,0f),(1f,0),(3f,0f)};
    readonly List<(float x, float y)> posInside = new() {(-3f,-2.5f),(-1f,-2.5f),(1f,-2.5f),(3f,-2.5f)};
    [SerializeField]
    TextMeshProUGUI levelDisplay;

    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelDisplay.text = "Level: " + GameManager.Instance.level.ToString();
        Shuffle(inside);
        Shuffle(outside);

        // Set cup count for star rating calculation
        GameManager.Instance.cupCount = outside.Count;
        for (int i = 0; i < inside.Count; i++)
        {
            GameObject insideCup = inside[i];
            if (insideCup == null)
            {
                Debug.LogWarning($"Missing inside cup prefab at index {i} on {name}; skipping.");
                continue;
            }
            // write the spawned instance back so downstream code (e.g. SwappingManager)
            // operates on the live scene object, not the prefab asset
            inside[i] = Instantiate(insideCup, new Vector3(posInside[i].x, posInside[i].y), Quaternion.identity, inCup);
        }
        for (int i = 0; i < outside.Count; i++)
        {
            GameObject outsideCup = outside[i];
            if (outsideCup == null)
            {
                Debug.LogWarning($"Missing outside cup prefab at index {i} on {name}; skipping.");
                continue;
            }
            outside[i] = Instantiate(outsideCup, new Vector3(posOutside[i].x, posOutside[i].y), outsideCup.transform.rotation, outCup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

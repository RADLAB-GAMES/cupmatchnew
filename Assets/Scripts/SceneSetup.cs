using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SceneSetup : MonoBehaviour
{
    // Master pool of all available color pairs (matched by index: inside[i] and outside[i] are the same color).
    // Each level uses only a subset of these, sized by GameManager.GetCupCountForLevel.
    public List<GameObject> inside, outside;
    [SerializeField]
    Transform inCup, outCup;
    const float cupSpacing = 2f;
    const float outsideY = 0f;
    const float insideY = -2.5f;
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

    // Evenly spaces `count` cups along the x-axis, centered on x=0.
    static List<(float x, float y)> GeneratePositions(int count, float y)
    {
        var positions = new List<(float, float)>(count);
        float startX = -(count - 1) * cupSpacing / 2f;
        for (int i = 0; i < count; i++)
            positions.Add((startX + i * cupSpacing, y));
        return positions;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelDisplay.text = "Level: " + GameManager.Instance.level.ToString();

        int cupCount = Mathf.Clamp(
            GameManager.Instance.GetCupCountForLevel(GameManager.Instance.level),
            1, Mathf.Min(inside.Count, outside.Count));

        // Pick which color pairs are in play this level, keeping inside/outside aligned by index.
        var colorIndices = Enumerable.Range(0, Mathf.Min(inside.Count, outside.Count)).ToList();
        Shuffle(colorIndices);
        colorIndices = colorIndices.Take(cupCount).ToList();
        inside = colorIndices.Select(i => inside[i]).ToList();
        outside = colorIndices.Select(i => outside[i]).ToList();

        Shuffle(inside);
        Shuffle(outside);

        // Set cup count for star rating calculation
        GameManager.Instance.cupCount = cupCount;

        var posInside = GeneratePositions(cupCount, insideY);
        var posOutside = GeneratePositions(cupCount, outsideY);

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

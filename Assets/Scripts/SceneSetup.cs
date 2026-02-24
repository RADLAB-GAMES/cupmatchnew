using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public List<GameObject> inside, outside;
    [SerializeField]
    Transform inCup, outCup;
    readonly List<(float x, float y)> posOutside = new() {(-3f,0f),(-1f,0f),(1f,0),(3f,0f)};
    readonly List<(float x, float y)> posInside = new() {(-3f,-2.5f),(-1f,-2.5f),(1f,-2.5f),(3f,-2.5f)};

    int insideCups, outsideCups = 0;
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
        Shuffle(inside);
        Shuffle(outside);

        // Set cup count for star rating calculation
        GameManager.Instance.cupCount = outside.Count;
        foreach(GameObject insideCup in inside)
        {
            Instantiate(insideCup, new Vector3(posInside[insideCups].x,posInside[insideCups].y), Quaternion.identity, inCup);
            insideCups++;
        }
        foreach(GameObject outsideCup in outside)
        {
            Instantiate(outsideCup, new Vector3(posOutside[outsideCups].x,posOutside[outsideCups].y), outsideCup.transform.rotation, outCup);
            outsideCups++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

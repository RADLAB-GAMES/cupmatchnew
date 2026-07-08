using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SwappingManager : MonoBehaviour
{
    [SerializeField]
    SceneSetup ss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnGameStateChange += FireOff;
        GameManager.OnGameStateChange += Check;
    }



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= FireOff;
        GameManager.OnGameStateChange -= Check;
    }

    void FireOff(GameState state)
    {
        if(state == GameState.Swap)
        {
            Swap();
            GameManager.Instance.UpdateGameState(GameState.Trying);
        }
        
    }

    private int rotationsPending = 0;

    public void Swap()
    {
        var cupA = GameManager.Instance.clickedOn[0];
        var cupB = GameManager.Instance.clickedOn[1];
        Vector3 posA = cupA.transform.position;
        Vector3 posB = cupB.transform.position;

        // use this as midpoint to create pivot point to revolve objects
        var objDistance = (posA + posB) / 2;

        // A 180-degree revolve around the midpoint of A and B lands each cup exactly
        // on the other's starting x/z; compute that analytically instead of trusting
        // the float accumulation from many small RotateAround steps, which drifts.
        Vector3 targetA = new Vector3(posB.x, posA.y - 0.25f, posB.z);
        Vector3 targetB = new Vector3(posA.x, posB.y - 0.25f, posA.z);

        GameManager.Instance.isSwapping = true;
        rotationsPending = 2;
        StartCoroutine(RotateTimed(0.5f, cupA, objDistance, targetA));
        StartCoroutine(RotateTimed(.5f, cupB, objDistance, targetB));
        SwapListPos();
        GameManager.Instance.clickedOn.Clear();
        GameManager.Instance.coolOff = false;
        GameManager.Instance.moves++;
        //GameManager.Instance.UIManager.MoveCountUpdate();
    }

    IEnumerator RotateTimed(float duration, GameObject clicked, Vector3 objDistance, Vector3 targetPosition)
    {
        float timer = 0f;
        float startAngle = 0f;
        float currentAngle;
        while (timer < duration)
        {
            // clicked.transform.RotateAround(objDistance, Vector3.up, 1440 * Time.deltaTime);
            timer += Time.deltaTime;

            // Lerp rotation progress
            currentAngle = Mathf.Lerp(startAngle, 180f, timer / duration);

            // Rotate difference since last frame
            clicked.transform.RotateAround(objDistance, Vector3.up, currentAngle - startAngle);

            startAngle = currentAngle;

            yield return null;
        }
        // snap to the exact target slot to eliminate drift accumulated during rotation
        clicked.transform.position = targetPosition;

        rotationsPending--;
        if (rotationsPending <= 0)
        {
            GameManager.Instance.isSwapping = false;
        }
    }

    public void SwapListPos()
    {
        int indexA = ss.outside.FindIndex(c => c.GetComponent<Renderer>().sharedMaterial == GameManager.Instance.clickedOn[0].GetComponent<Renderer>().sharedMaterial);
        int indexB = ss.outside.FindIndex(c=> c.GetComponent<Renderer>().sharedMaterial == GameManager.Instance.clickedOn[1].GetComponent<Renderer>().sharedMaterial);

        if (indexA == -1 || indexB == -1)
        {
            Debug.LogError($"Object not found! indexA: {indexA}, indexB: {indexB}");
            return;
        }

        (ss.outside[indexB], ss.outside[indexA]) = (ss.outside[indexA], ss.outside[indexB]);
    }

    private void Check(GameState state)
    {
        if(state == GameState.Check)
        {
            // need to reset counter of correct matches so true number of correct is represented
            GameManager.Instance.correctMatches = 0;

            GameManager.Instance.coolOff = true;

            // Compare outside cups with inside cups by material
            bool isMatch = true;
            for (int i = 0; i < ss.outside.Count && i < ss.inside.Count; i++)
            {
                var outsideMat = ss.outside[i].GetComponent<Renderer>().sharedMaterial;
                var insideMat = ss.inside[i].GetComponent<Renderer>().sharedMaterial;

                if (outsideMat == insideMat)
                {
                    GameManager.Instance.correctMatches++;

                } else
                {
                    isMatch = false;
                }
            }

            if (isMatch)
            {
                GameManager.Instance.UpdateGameState(GameState.Win);
            }
            else
            {
                GameManager.Instance.moves++;
                GameManager.Instance.UpdateGameState(GameState.Trying);
            }
        }
    }
}

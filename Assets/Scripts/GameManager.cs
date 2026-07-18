using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State;
    public static event Action<GameState> OnGameStateChange;
    public int level;
    public int moves;
    public bool coolOff = false;
    public bool isSwapping = false;
    public List<GameObject> clickedOn = new();
    public int correctMatches = 0;
    public int cupCount = 0;

    // How many checks a player gets per level for free.
    public int freeChecksPerLevel = 2;
    // How many checks the player has used so far on the current level; resets each level.
    public int checksUsedThisLevel = 0;
    // Extra checks bought via monetization; persists across levels and is only spent once
    // the free-per-level allowance runs out.
    public int bonusChecks = 0;

    /// <summary>
    /// Calculates star rating based on moves and cup count.
    /// 3 stars: moves <= cupCount (perfect/near-perfect play)
    /// 2 stars: moves <= cupCount * 2
    /// 1 star: moves > cupCount * 2
    /// </summary>
    public int CalculateStarRating()
    {
        if (cupCount <= 0) return 1;

        if (moves <= cupCount)
            return 3;
        else if (moves <= cupCount * 2)
            return 2;
        else
            return 1;
    }

    public bool HasChecksRemaining()
    {
        return checksUsedThisLevel < freeChecksPerLevel || bonusChecks > 0;
    }

    public int RemainingChecks()
    {
        return Mathf.Max(0, freeChecksPerLevel - checksUsedThisLevel) + bonusChecks;
    }

    // Spends one check: consumes a bonus check once the free allowance is used up, and
    // detracts from the player's score the same way a wasted move does.
    public void UseCheck()
    {
        if (checksUsedThisLevel >= freeChecksPerLevel)
            bonusChecks = Mathf.Max(0, bonusChecks - 1);

        checksUsedThisLevel++;
        moves++;
    }

    public void ResetChecksForLevel()
    {
        checksUsedThisLevel = 0;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Replace this with something like playerprefs to continue where the player left off
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch(newState)
        {
            case GameState.Trying:
                break;
            case GameState.Win:
                break;
            case GameState.Swap:
                break;
            case GameState.Check:
                break;
        }
    
    OnGameStateChange?.Invoke(newState);

    }
}

public enum GameState
{
    Trying,
    Win,
    Swap,
    Check,
}

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
    public List<GameObject> clickedOn = new();
    public int correctMatches = 0;

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

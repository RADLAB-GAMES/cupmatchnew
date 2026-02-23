using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI moves;
    [SerializeField]
    TextMeshProUGUI amountCorrect;
    [SerializeField]
    TextMeshProUGUI starRating;


    void Awake()
    {
        GameManager.OnGameStateChange += ShowScore; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void ODestroy()
    {
        GameManager.OnGameStateChange -= ShowScore;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadScene()
    {
        GameManager.Instance.coolOff = false;
        GameManager.Instance.moves = 0;
        GameManager.Instance.correctMatches = 0;
        GameManager.Instance.cupCount = 0;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    void ShowScore(GameState state)
    {
        // update score after swap switches back to trying
        if (state == GameState.Trying)
        {
            moves.text = "Moves: " + GameManager.Instance.moves;
        }
        else if (state == GameState.Win)
        {
            moves.text = "You Won in " + GameManager.Instance.moves + " moves!";
            DisplayStarRating();
        }
    }

    void DisplayStarRating()
    {
        if (starRating == null) return;

        int stars = GameManager.Instance.CalculateStarRating();
        starRating.enabled = true;
        starRating.text = new string('★', stars) + new string('☆', 3 - stars);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void CheckButton()
    {
        if (GameManager.Instance.coolOff == false)
        {
            GameManager.Instance.UpdateGameState(GameState.Check);
            amountCorrect.enabled = true;
            amountCorrect.text =  "Amount Correct: " + GameManager.Instance.correctMatches.ToString();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI moves;
    [SerializeField]
    TextMeshProUGUI amountCorrect;
    [SerializeField]
    GameObject cont;
    [SerializeField]
    GameObject reset;
    [SerializeField]
    GameObject check;
    [SerializeField] Image[] starImages;
    [SerializeField] Sprite starFilled;
    [SerializeField] Sprite starEmpty;


    void Awake()
    {
        GameManager.OnGameStateChange += ShowScore; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // hide continue button until win state
        cont.SetActive(false);

        // make stars transparent until win screen
        if (starImages != null)
            foreach (Image star in starImages)
                star.color = Color.clear;
    }

    void OnDestroy()
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
        GameManager.Instance.clickedOn.Clear();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueUpdate()
    {
        // TODO: Need a way to change levels difficulty according to level number 
        // if GameManager.Instance.level < 4 3 cups...
        GameManager.Instance.level++;
        GameManager.Instance.coolOff = false;
        GameManager.Instance.moves = 0;
        GameManager.Instance.correctMatches = 0;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.UpdateGameState(GameState.Trying);
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
            // show continue button
            if (cont != null)
                cont.SetActive(true);
            moves.text = "You Won in " + GameManager.Instance.moves + " moves!";
            reset.SetActive(false);
            check.SetActive(false);
            DisplayStarRating();
        }
    }

    void DisplayStarRating()
    {
        if (starImages == null) return;

        int stars = GameManager.Instance.CalculateStarRating();
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = Color.white;
            starImages[i].sprite = i < stars ? starFilled : starEmpty;
        }
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

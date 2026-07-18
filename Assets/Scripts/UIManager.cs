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
    TextMeshProUGUI remainingChecks;
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
        if (cont != null)
            cont.SetActive(false);

        // make stars transparent until win screen
        if (starImages != null)
            foreach (Image star in starImages)
                star.color = Color.clear;

        UpdateRemainingChecksText();
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
        GameManager.Instance.ResetChecksForLevel();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueUpdate()
    {
        GameManager.Instance.level++;
        GameManager.Instance.coolOff = false;
        GameManager.Instance.moves = 0;
        GameManager.Instance.correctMatches = 0;
        GameManager.Instance.ResetChecksForLevel();

        // Wait for the new scene's objects to Awake (and subscribe to OnGameStateChange)
        // before firing Trying, instead of firing it on this scene's subscribers mid-teardown.
        SceneManager.sceneLoaded += OnNextLevelLoaded;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    void OnNextLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNextLevelLoaded;
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
            if (reset != null)
                reset.SetActive(false);
            if (check != null)
                check.SetActive(false);
            if (remainingChecks != null)
                remainingChecks.enabled = false;
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

    public void GoToMainMenu()
    {
        // GameManager persists across scenes, so clear the round state before leaving
        GameManager.Instance.coolOff = false;
        GameManager.Instance.isSwapping = false;
        GameManager.Instance.moves = 0;
        GameManager.Instance.correctMatches = 0;
        GameManager.Instance.cupCount = 0;
        GameManager.Instance.clickedOn.Clear();
        GameManager.Instance.ResetChecksForLevel();
        SceneManager.LoadSceneAsync(0);
    }

    public void CheckButton()
    {
        if (GameManager.Instance.coolOff == false)
        {
            // Whether this use still falls within the free/bonus allowance, decided
            // before it's spent so the free checks (and any purchased bonus ones)
            // are the ones that reveal the number.
            bool showAmount = GameManager.Instance.HasChecksRemaining();

            GameManager.Instance.UseCheck();
            GameManager.Instance.UpdateGameState(GameState.Check);

            // Checking can synchronously trigger a win (SwappingManager.Check), which already
            // hid the check UI via ShowScore(Win); don't let the rest of this method undo that.
            if (GameManager.Instance.State == GameState.Win)
                return;

            amountCorrect.enabled = showAmount;
            if (showAmount)
                amountCorrect.text = "Amount Correct: " + GameManager.Instance.correctMatches.ToString();

            UpdateRemainingChecksText();
        }
    }

    void UpdateRemainingChecksText()
    {
        if (remainingChecks == null) return;

        // Was duplicated from amountCorrect, which starts disabled until CheckButton
        // enables it; force it on so it doesn't need a manual toggle in the Inspector.
        remainingChecks.enabled = true;
        remainingChecks.text = "Remaining Checks: " + GameManager.Instance.RemainingChecks();
    }
}

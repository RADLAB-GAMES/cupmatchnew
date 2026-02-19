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
    GameObject cont;


    void Awake()
    {
        GameManager.OnGameStateChange += ShowScore; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // hide continue button until win state
        cont.SetActive(false);
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
            cont.SetActive(true);
            moves.text = "You Won in " + GameManager.Instance.moves + " moves!";
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

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StateMachine = new GameStateMachine();
        Debug.Log("GameManager initialisé");
    }

    private void Start()
    {
        // Une fois le GameManager en place, on charge la scène de jeu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
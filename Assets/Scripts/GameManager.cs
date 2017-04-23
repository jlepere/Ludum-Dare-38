using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject soundManager;

    public const bool IS_DEBUG = true;
    private static int Score = 0;

    private void Awake()
    {
        Instantiate(soundManager);
    }

    public static int GetScore
    {
        get { return Score; }
    }

    public static void AddScore(int score)
    {
        Score += score;
    }

    public static void Retry()
    {
        Score = 0;
        SceneManager.LoadScene("LevelScene");
    }

    public static void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}

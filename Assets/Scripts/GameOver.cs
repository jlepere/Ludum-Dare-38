using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Text gameOverScore;

    private void Start()
    {
        gameOverScore.text = string.Format("Game Over (Score {0})", GameManager.GetScore);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Jump"))
            GameManager.Retry();
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("Swap"))
            Application.Quit();
    }
}

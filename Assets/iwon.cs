
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject winMessageUI; // assign in inspector

    private bool hasWon = false;

    void Update()
    {
        // This is just an example trigger — replace with your actual win condition
        if (!hasWon && PlayerReachedGoal())
        {
            WinGame();
        }
    }

    bool PlayerReachedGoal()
    {
        // Placeholder: Replace this with your real win logic
        return Input.GetKeyDown(KeyCode.W); // e.g., testing with key press
    }

    void WinGame()
    {
        hasWon = true;
        winMessageUI.SetActive(true);
        Debug.Log("Player has won the game!");
        // Optionally pause game
        Time.timeScale = 0f;
    }
}

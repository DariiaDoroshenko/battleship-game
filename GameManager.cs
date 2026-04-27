using UnityEngine;
using TMPro;

public enum GameState
{
    PlacingShips,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameState currentState;

    public GameObject startButton;
    public TextMeshProUGUI resultText;

    public static int totalShipCells = 0;
    public static int hitCount = 0;

    
    public static int[] shipsToPlace = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
    public static int currentShipIndex = 0;

    void Start()
    {
        currentState = GameState.PlacingShips;
        resultText.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (currentShipIndex < shipsToPlace.Length)
        {
            Debug.Log("Place all ships first!");
            return;
        }

        currentState = GameState.Playing;
        startButton.SetActive(false);
        Debug.Log("Game started!");
    }

    public static void RegisterHit()
    {
        hitCount++;

        if (hitCount >= totalShipCells)
        {
            currentState = GameState.GameOver;
            Debug.Log("YOU WIN!");
        }
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState
{
    PlacingShips,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameState currentState;

    public int totalShipCells = 20;
    public int shipsToPlace = 20;
    public int[] ships =
{
    4,
    3, 3,
    2, 2, 2,
    1, 1, 1, 1
};

    public int currentShipIndex = 0;

    public bool horizontalPlacement = true;
    public int placedShips = 0;
    public bool playerTurn = true;

    public GridGenerator playerGrid;
    public GridGenerator enemyGrid;

    int playerHits = 0;
    int playerShots = 0;

    int enemyHits = 0;

    public TextMeshProUGUI hitsText;
    public TextMeshProUGUI shotsText;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI placementText;

    public GameObject startButton;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentState = GameState.PlacingShips;

        resultText.gameObject.SetActive(false);

        startButton.SetActive(false);

        UpdateUI();
    }
    public void RegisterShot(bool hit)
    {
        playerShots++;

        // якщо промах — хід переходить AI
        if (!hit)
        {
            playerTurn = false;

            UpdateUI();

            Invoke(nameof(EnemyTurn), 1f);
        }
        else
        {
            // при попаданні хід гравця залишається
            playerTurn = true;

            UpdateUI();
        }
    }

    public void RegisterHit()
    {
        playerHits++;

        UpdateUI();

        if (playerHits >= totalShipCells)
        {
            WinGame();
        }
    }


    void EnemyTurn()
    {
        int x = Random.Range(0, 10);
        int z = Random.Range(0, 10);

        while (playerGrid.hits[x, z])
        {
            x = Random.Range(0, 10);
            z = Random.Range(0, 10);
        }

        playerGrid.hits[x, z] = true;
        Renderer rend =
    playerGrid.cells[x, z]
    .GetComponent<Renderer>();



        bool hit = false;

        if (playerGrid.grid[x, z] == 1)
        {
            rend.material.color = Color.red;

            Debug.Log("Enemy HIT!");

            enemyHits++;
            if (IsShipDestroyed(playerGrid, x, z))
            {
                OpenCellsAroundShip(playerGrid, x, z);
            }

            hit = true;

            if (enemyHits >= totalShipCells)
            {
                LoseGame();
                return;
            }
        }
        else
        {
            rend.material.color = Color.blue;

            Debug.Log("Enemy MISS!");
        }

        if (hit)
        {
            Invoke(nameof(EnemyTurn), 1f);
        }
        else
        {
            playerTurn = true;
        }

        UpdateUI();

        UpdateUI();
    }


    public void UpdateUI()
    {
        hitsText.text = "Hits: " + playerHits;
        shotsText.text = "Shots: " + playerShots;

        if (currentState == GameState.Playing)
        {
            if (playerTurn)
            {
                stateText.text = "Your Turn";
            }
            else
            {
                stateText.text = "Enemy Turn";
            }
        }
        
        if (currentState == GameState.PlacingShips)
        {
            if (currentShipIndex < ships.Length)
            {
                placementText.text =
                    "Place ship: " +
                    ships[currentShipIndex] +
                    " cells";

                startButton.SetActive(false);
            }
            else
            {
                placementText.text =
                    "Press START GAME";

                startButton.SetActive(true);
            }
        }
        else
        {
            placementText.text = "";
        }
    }

    void WinGame()
    {
        currentState = GameState.GameOver;

        resultText.gameObject.SetActive(true);

        resultText.text = "YOU WIN!";
    }

    void LoseGame()
    {
        currentState = GameState.GameOver;

        resultText.gameObject.SetActive(true);

        resultText.text = "YOU LOSE!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    bool IsShipDestroyed(GridGenerator grid, int x, int z)
    {
        int[,] directions =
        {
        {1, 0},
        {-1, 0},
        {0, 1},
        {0, -1}
    };

        for (int i = 0; i < 4; i++)
        {
            int dx = directions[i, 0];
            int dz = directions[i, 1];

            int checkX = x;
            int checkZ = z;

            while (true)
            {
                checkX += dx;
                checkZ += dz;

                if (checkX < 0 || checkX >= 10 ||
                    checkZ < 0 || checkZ >= 10)
                    break;

                if (grid.grid[checkX, checkZ] == 0)
                    break;

                if (!grid.hits[checkX, checkZ])
                    return false;
            }
        }

        return true;
    }

    void OpenCellsAroundShip(GridGenerator grid, int hitX, int hitZ)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                // шукаємо клітинки цього корабля
                if (grid.grid[x, z] == 1 &&
                    grid.hits[x, z])
                {
                    // відкриваємо все навколо
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dz = -1; dz <= 1; dz++)
                        {
                            int nx = x + dx;
                            int nz = z + dz;

                            if (nx < 0 || nx >= 10 ||
                                nz < 0 || nz >= 10)
                                continue;

                            // тільки вода
                            if (grid.grid[nx, nz] == 0)
                            {
                                grid.hits[nx, nz] = true;

                                Renderer rend =
                                    grid.cells[nx, nz]
                                    .GetComponent<Renderer>();

                                rend.material.color = Color.blue;
                            }
                        }
                    }
                }
            }
        }


    }

    public void StartGame()
    {
        // не всі кораблі поставлені
        if (currentShipIndex < ships.Length)
        {
            Debug.Log("Place all ships!");
            return;
        }

        currentState = GameState.Playing;

        startButton.SetActive(false);

        UpdateUI();

        Debug.Log("GAME STARTED");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            horizontalPlacement =
                !horizontalPlacement;
        }
    }



}
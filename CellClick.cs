using UnityEngine;

public class CellClick : MonoBehaviour
{
    public int row;
    public int col;
    public GridGenerator gridGenerator;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        if (GameManager.currentState == GameState.PlacingShips)
        {
            TryPlaceShip();
        }
        else if (GameManager.currentState == GameState.Playing)
        {
            Shoot();
        }
    }

    void TryPlaceShip()
    {
        if (GameManager.currentShipIndex >= GameManager.shipsToPlace.Length)
            return;

        if (gridGenerator.grid[row, col] == 1)
            return;

        // перевірка сусідів
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                int nx = row + dx;
                int nz = col + dz;

                if (nx >= 0 && nx < gridGenerator.width &&
                    nz >= 0 && nz < gridGenerator.height)
                {
                    if (gridGenerator.grid[nx, nz] == 1)
                        return;
                }
            }
        }

        // ставимо 1 клітинку (поки без напрямку)
        gridGenerator.grid[row, col] = 1;
        rend.material.color = Color.green;

        GameManager.totalShipCells++;
        GameManager.currentShipIndex++;

        Debug.Log("Ship part placed");
    }

    void Shoot()
    {
        if (gridGenerator.hits[row, col])
            return;

        gridGenerator.hits[row, col] = true;

        if (gridGenerator.grid[row, col] == 1)
        {
            rend.material.color = Color.red;
            Debug.Log("HIT!");
            GameManager.RegisterHit();
        }
        else
        {
            rend.material.color = Color.blue;
            Debug.Log("MISS!");
        }
    }
}
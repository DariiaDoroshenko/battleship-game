using UnityEngine;

public class CellClick : MonoBehaviour
{
    public int row;
    public int col;

    public bool isEnemyCell;

    public GridGenerator gridGenerator;

    Renderer rend;

    bool clicked = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        // -------------------------
        // РОЗСТАНОВКА КОРАБЛІВ
        // -------------------------

        if (GameManager.currentState ==
            GameState.PlacingShips)
        {
            // тільки поле гравця
            if (isEnemyCell) return;

            // всі кораблі вже поставлені
            if (GameManager.instance.currentShipIndex >=
                GameManager.instance.ships.Length)
                return;

            int shipSize =
                GameManager.instance.ships[
                    GameManager.instance.currentShipIndex];

            bool horizontal =
                GameManager.instance.horizontalPlacement;

            // не можна поставити
            if (!CanPlaceShip(shipSize, horizontal))
                return;

            // ставимо корабель
            for (int i = 0; i < shipSize; i++)
            {
                int x = horizontal ? row + i : row;
                int z = horizontal ? col : col + i;

                gridGenerator.grid[x, z] = 1;

                GameObject cell =
                    gridGenerator.cells[x, z];

                cell.GetComponent<Renderer>()
                    .material.color = Color.green;
            }

            GameManager.instance.currentShipIndex++;

            GameManager.instance.UpdateUI();

            return;
        }

        // -------------------------
        // ГРА
        // -------------------------

        if (!isEnemyCell) return;

        if (!GameManager.instance.playerTurn)
            return;

        if (clicked) return;

        clicked = true;

        gridGenerator.hits[row, col] = true;

        bool hit = false;

        // HIT
        if (gridGenerator.grid[row, col] == 1)
        {
            rend.material.color = Color.red;

            GameManager.instance.RegisterHit();

            hit = true;

            if (IsShipDestroyed(row, col))
            {
                OpenCellsAroundShip(row, col);
            }
        }
        // MISS
        else
        {
            rend.material.color = Color.blue;
        }

        GameManager.instance.RegisterShot(hit);
    }

    bool CanPlaceShip(int size, bool horizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int x = horizontal ? row + i : row;
            int z = horizontal ? col : col + i;

            if (x >= 10 || z >= 10)
                return false;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx;
                    int nz = z + dz;

                    if (nx < 0 || nx >= 10 ||
                        nz < 0 || nz >= 10)
                        continue;

                    if (gridGenerator.grid[nx, nz] == 1)
                        return false;
                }
            }
        }

        return true;
    }

    bool IsShipDestroyed(int x, int z)
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

                if (gridGenerator.grid[checkX, checkZ] == 0)
                    break;

                if (!gridGenerator.hits[checkX, checkZ])
                    return false;
            }
        }

        return true;
    }

    void OpenCellsAroundShip(int x, int z)
    {
        int minX = x;
        int maxX = x;

        int minZ = z;
        int maxZ = z;

        while (minX > 0 &&
               gridGenerator.grid[minX - 1, z] == 1)
        {
            minX--;
        }

        while (maxX < 9 &&
               gridGenerator.grid[maxX + 1, z] == 1)
        {
            maxX++;
        }

        while (minZ > 0 &&
               gridGenerator.grid[x, minZ - 1] == 1)
        {
            minZ--;
        }

        while (maxZ < 9 &&
               gridGenerator.grid[x, maxZ + 1] == 1)
        {
            maxZ++;
        }

        for (int i = minX - 1; i <= maxX + 1; i++)
        {
            for (int j = minZ - 1; j <= maxZ + 1; j++)
            {
                if (i < 0 || i >= 10 ||
                    j < 0 || j >= 10)
                    continue;

                if (gridGenerator.grid[i, j] == 0)
                {
                    gridGenerator.hits[i, j] = true;

                    GameObject cell =
                        gridGenerator.cells[i, j];

                    Renderer cellRenderer =
                        cell.GetComponent<Renderer>();

                    cellRenderer.material.color =
                        Color.blue;
                }
            }
        }
    }
}
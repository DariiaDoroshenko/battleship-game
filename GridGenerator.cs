using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab;

    public bool isEnemyGrid;

    public int width = 10;
    public int height = 10;
    public float spacing = 1.2f;

    public int[,] grid;

    public bool[,] hits;
    public GameObject[,] cells;

    void Start()
    {
        grid = new int[width, height];
        hits = new bool[width, height];
        cells = new GameObject[width, height];

        if (isEnemyGrid)
        {
            GenerateShips();
        }

        GenerateGrid();

    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = transform.position + new Vector3(
                    x * spacing,
                    0,
                    z * spacing
                );

                GameObject cellObj = Instantiate(

                    cellPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );
                cells[x, z] = cellObj;

                CellClick cell = cellObj.GetComponent<CellClick>();

                cell.row = x;
                cell.col = z;

                cell.gridGenerator = this;

                cell.isEnemyCell = isEnemyGrid;

                // Показуємо кораблі тільки на полі гравця
                if (!isEnemyGrid && grid[x, z] == 1)
                {
                    cellObj.GetComponent<Renderer>().material.color = Color.green;
                }
            }
        }
    }

    void GenerateShips()
    {
        PlaceShip(4);

        PlaceShip(3);
        PlaceShip(3);

        PlaceShip(2);
        PlaceShip(2);
        PlaceShip(2);

        PlaceShip(1);
        PlaceShip(1);
        PlaceShip(1);
        PlaceShip(1);
    }

    void PlaceShip(int size)
    {
        bool placed = false;

        while (!placed)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, height);

            bool horizontal = Random.Range(0, 2) == 0;

            if (CanPlaceShip(x, z, size, horizontal))
            {
                for (int i = 0; i < size; i++)
                {
                    int px = horizontal ? x + i : x;
                    int pz = horizontal ? z : z + i;

                    grid[px, pz] = 1;
                }

                placed = true;
            }
        }
    }

    bool CanPlaceShip(int x, int z, int size, bool horizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int px = horizontal ? x + i : x;
            int pz = horizontal ? z : z + i;

            if (px >= width || pz >= height)
                return false;

            for (int checkX = px - 1; checkX <= px + 1; checkX++)
            {
                for (int checkZ = pz - 1; checkZ <= pz + 1; checkZ++)
                {
                    if (checkX >= 0 &&
                        checkX < width &&
                        checkZ >= 0 &&
                        checkZ < height)
                    {
                        if (grid[checkX, checkZ] == 1)
                            return false;
                    }
                }
            }
        }

        return true;
    }
}
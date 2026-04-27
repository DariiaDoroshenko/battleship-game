using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public int width = 10;
    public int height = 10;
    public float spacing = 1.2f;

    public int[,] grid;
    public bool[,] hits;

    void Start()
    {
        grid = new int[width, height];
        hits = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * spacing, 0.1f, z * spacing);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                CellClick cell = cellObj.GetComponent<CellClick>();
                cell.row = x;
                cell.col = z;
                cell.gridGenerator = this;
            }
        }
    }
}
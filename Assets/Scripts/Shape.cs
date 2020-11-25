using UnityEngine;

public class Shape : MonoBehaviour
{
    float lastFall = 0;

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = PlayField.RoundVec2(child.position);

            // Not inside Border?
            if (!PlayField.InsideBorder(v))
            {
                return false;
            }

            // Т.к. фигуры создаются за пределами поля, выбрасывается исключение IndexOutOfRangeException
            // просто игнорируем его
            try
            {
                // Block in grid cell (and not part of same group)?
                if (PlayField.grid[(int)v.x, (int)v.y] != null
                    && PlayField.grid[(int)v.x, (int)v.y].parent != transform)
                {
                    return false;
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                return true;
            }
        }

        return true;
    }

    void UpdateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < PlayField.h; ++y)
        {
            for (int x = 0; x < PlayField.w; ++x)
            {
                if (PlayField.grid[x, y] != null)
                {
                    if (PlayField.grid[x, y].parent == transform)
                    {
                        PlayField.grid[x, y] = null;
                    }
                }
            }
        }

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = PlayField.RoundVec2(child.position);

            try
            {
                PlayField.grid[(int)v.x, (int)v.y] = child;
            }
            catch (System.IndexOutOfRangeException)
            {
            }
        }
    }

    void Start()
    {
        if (!IsValidGridPos())
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (PlayField.isGameOver)
        {
            GameObject go = GameObject.Instantiate(Resources.Load("GameOver", typeof(GameObject))) as GameObject;
            Instantiate(go, new Vector3(4.5f, 9.5f, 0), Quaternion.identity);

            // Disable script
            enabled = false;

            return;
        }

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if it's valid
            if (IsValidGridPos())
            {
                // It's valid. Update grid.
                UpdateGrid();
            }
            else
            {
                // Its not valid. revert.
                transform.position += new Vector3(1, 0, 0);
            }
        }
        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (IsValidGridPos())
            {
                // It's valid. Update grid.
                UpdateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            // See if valid
            if (IsValidGridPos())
            {
                // It's valid. Update grid.
                UpdateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.Rotate(0, 0, 90);
            }
        }
        // Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (IsValidGridPos())
            {
                // It's valid. Update grid.
                UpdateGrid();
            }
            else
            {
                PlayField.CheckGameOver();

                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                PlayField.DeleteFullRows();

                // Spawn next Group
                FindObjectOfType<Spawner>().SpawnNext();

                // Disable script
                enabled = false;
            }

            lastFall = Time.time;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 14, 100, 100), "Score: " + PlayField.score);
    }
}

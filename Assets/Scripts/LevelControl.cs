using UnityEngine;
using Random = UnityEngine.Random;

public class LevelControl : MonoBehaviour
{
    public Level currentlevelData;
    
    [SerializeField] private Cell cellPrefab;
    private Cell[,] cells;
    private Color[,] correctColors;
    private bool hasGameStarted;
    private bool canMove;
    private bool canStartClicking;
    private Cell selectedCell;
    private Cell movedCell;

    public void GenerateLvl(Cell cell, Level levelData)
    {
        currentlevelData = levelData;
        cellPrefab = cell;
        currentlevelData.hasPassed = GameManager.instance.CheckPassedLevel(currentlevelData.name);
        SpawnCells();
    }

    private void Update()
    {
        if (GameManager.instance.HasGameFinished || !hasGameStarted) return;
        if (!hasGameStarted) return;
        if (!canStartClicking)
        {
            for (int i = 0; i < currentlevelData.Row; i++)
            {
                for (int j = 0; j < currentlevelData.Col; j++)
                {
                    if (cells[i, j].IsStartMovePlaying)
                        return;
                }
            }
            canStartClicking = true;
            canMove = true;
        }

        if (!canMove)
        {
            if (!selectedCell.HasSelectedMoveFinished && (movedCell == null || !movedCell.HasMoveFinished)) return;
            selectedCell = null;
            movedCell = null;
            canMove = true;
            if (CheckWin())
            {
                hasGameStarted = false;
                GameManager.instance.Win();
            }
            return;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedCell == null) return;
         
            canMove = false;
            Vector3 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var selectedCellCollider = selectedCell.GetComponent<Collider2D>();
            selectedCellCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell newCell) 
                    && !currentlevelData.LockedCells.Contains(
                        new Vector2Int(newCell.Position.y, newCell.Position.x)))
            {
                movedCell = newCell;
                         
                (selectedCell.Position, movedCell.Position) = (movedCell.Position, selectedCell.Position);
                         
                cells[selectedCell.Position.y, selectedCell.Position.x] = selectedCell;
                cells[movedCell.Position.y, movedCell.Position.x] = movedCell;
                         
                selectedCell.SelectedMoveEnd();
                movedCell.MoveEnd();
            }
            else
            {
                selectedCell.SelectedMoveEnd();
            }
            selectedCellCollider.enabled = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell newSelectedCell) && selectedCell != newSelectedCell)
            {
                if (currentlevelData.LockedCells.Contains(
                    new Vector2Int(newSelectedCell.Position.y, newSelectedCell.Position.x)
                    ))
                {
                    selectedCell = null;
                    return;
                }

                selectedCell = newSelectedCell;
                selectedCell.SelectedMoveStart();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (selectedCell == null) return;
            Vector3 mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            selectedCell.SelectedMove(mousePos);
        }
    }

    
    public void Init()
    {
        if(hasGameStarted) return;
        hasGameStarted = true;
        for (int i = 0; i < currentlevelData.Row; i++)
        {
            for (int j = 0; j < currentlevelData.Col; j++)
            {
                if (currentlevelData.LockedCells.Contains(new Vector2Int(i, j)))
                {
                    continue;
                }

                int swapX, swapY;
                do
                {
                    swapX = Random.Range(0, currentlevelData.Row);
                    swapY = Random.Range(0, currentlevelData.Col);
                } while (currentlevelData.LockedCells.Contains(new Vector2Int(swapX, swapY)));
                Cell temp = cells[i, j];
                cells[i, j] = cells[swapX, swapY];
                Vector2Int swappedPostion = cells[swapX, swapY].Position;
                cells[i, j].Position = temp.Position;
                cells[swapX, swapY] = temp;
                temp.Position = swappedPostion;
            }
        }

        for (int i = 0; i < currentlevelData.Row; i++)
        {
            for (int j = 0; j < currentlevelData.Col; j++)
            {
                cells[i, j].AnimateStartPosition();
            }
        }
    }

    private bool CheckWin()
    {
        for (int i = 0; i < currentlevelData.Row; i++)
        {
            for (int j = 0; j < currentlevelData.Col; j++)
            {
                if (cells[i, j].Color != correctColors[i, j])
                    return false;
            }
        }
        return true;
    }
    
    [ContextMenu("GenerateLvl")]
    public void SpawnCells()
    {
        cells = new Cell[currentlevelData.Row, currentlevelData.Col];
        correctColors = new Color[currentlevelData.Row, currentlevelData.Col];

        Camera.main!.backgroundColor = currentlevelData.BackGroundColor;

        for (int x = 0; x < currentlevelData.Row; x++)
        {
            for (int y = 0; y < currentlevelData.Col; y++)
            {
                float xLerp = (float)y / (currentlevelData.Col - 1);
                float yLerp = (float)x / (currentlevelData.Row - 1);
                Color leftColor = Color.Lerp(
                    currentlevelData.BottomLeftColor,
                    currentlevelData.TopLeftColor,
                    yLerp
                ); 
                Color rightColor = Color.Lerp(
                    currentlevelData.BottomRightColor,
                    currentlevelData.TopRightColor,
                    yLerp
                );
                Color currentColor = Color.Lerp(
                    leftColor,
                    rightColor,
                    xLerp
                );
                correctColors[x, y] = currentColor;
                cells[x, y] = Instantiate(cellPrefab, transform);
                cells[x, y].Init(currentColor, this, y, x);
                if (currentlevelData.LockedCells.Contains(new Vector2Int(x, y)))
                {
                    cells[x, y].SetLockedCell();
                }
            }
        }
    }
    
    [ContextMenu("InitCells")]
    public void InitCells()
    {
        for (int x = 0; x < currentlevelData.Row; x++)
        {
            for (int y = 0; y < currentlevelData.Col; y++)
            {
                cells[x, y].AnimateStartScale();
            }
        }
    }

    public void ResetLevel()
    {
        for (int i = 0; i < currentlevelData.Row; i++)
        {
            for (int j = 0; j < currentlevelData.Col; j++)
            {
                cells[i, j].GameFinished();
            }
        }
    }

    public bool CheckCellsAnimationEnd()
    {
        for (int i = 0; i < currentlevelData.Row; i++)
        {
            for (int j = 0; j < currentlevelData.Col; j++)
            {
                if (cells[i, j].IsStartTweenPlaying)
                    return false;
            }
        }

        return true;
    }
}
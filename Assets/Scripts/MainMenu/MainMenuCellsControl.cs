using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuCellsControl : MonoBehaviour
{
    public List<Color> cellsColors;
    public List<Vector2> cellsCords;
    
    [SerializeField] private Cell[] cells;
    [SerializeField] private Cell firstCell;
    [SerializeField] private Cell secondCell;

    private void Start()
    {
        InitCells();
    }

    public void InitCells()
    {
        (firstCell.Position, secondCell.Position) = (secondCell.Position, firstCell.Position);

        firstCell.MoveEnd(secondCell.transform.localPosition).SetDelay(0.5f + (5 + 5) * 0.04f);
        secondCell.MoveEnd(firstCell.transform.localPosition).SetDelay(0.5f + (5 + 5) * 0.04f);
        foreach (var cell in cells)
        {
            var cellCord = cellsCords[cellsColors.IndexOf(cell.spriteRenderer.color)];
            cell.InitScaleAnimation((int)cellCord.x, (int)cellCord.y);
            cell.AnimateStartScale();
        }
    }
}
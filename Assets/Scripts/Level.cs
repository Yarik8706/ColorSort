using System;
using System.Collections.Generic;
using UnityEngine;

public enum LockedCellsType
{
    None,
    OpenOnlyOneLine,
    BlockedDiagonally,
    OpenOnlyTwoLine,
    CornersBlocked
}

public enum LockedCellsLineType
{
    None,
    CornersBlocked,
    All, 
    CornersNotBlocked,
    One,
    Two
}

[CreateAssetMenu(fileName = "Level", menuName = "ColorSort/Level")]
public class Level : ScriptableObject
{
    public Color BackGroundColor;
    public bool hasPassed;

    public Color TopLeftColor;
    public Color TopRightColor;
    public Color BottomLeftColor;
    public Color BottomRightColor;

    public int Row;
    public int Col;

    public List<Vector2Int> LockedCells;
    public LockedCellsType lockedCellsType;
    
    public LockedCellsLineType[] lockedCellsVerticalLineTypes;
    public LockedCellsLineType[] lockedCellsHorizontalLineTypes;

    public void SetLockedCells()
    {
        if(lockedCellsType == LockedCellsType.None) return;
        
        LockedCells.Clear();

        switch (lockedCellsType)
        {
            case LockedCellsType.OpenOnlyOneLine:
                SetOpenOnlyOneLine();
                break;
            case LockedCellsType.BlockedDiagonally:
                SetBlockedDiagonally();
                break;
            case LockedCellsType.CornersBlocked:
                SetCornersBlocked();
                break;
            case LockedCellsType.OpenOnlyTwoLine:
                SetOpenOnlyTwoLine();
                break;
        }
    }

    [ContextMenu("SetLockedCellsInEditor")]
    public void SetLockedCellsInEditor()
    {
        LockedCells.Clear();
        for (int i = 0; i < lockedCellsVerticalLineTypes.Length; i ++)
        {
            switch (lockedCellsVerticalLineTypes[i])
            {
                case LockedCellsLineType.None:
                    break;
                case LockedCellsLineType.CornersBlocked:
                    LockedCells.Add(new Vector2Int(0, i));
                    LockedCells.Add(new Vector2Int(Row-1, i));
                    break;
                case LockedCellsLineType.All:
                    LockedLine(i);
                    break;
                case LockedCellsLineType.CornersNotBlocked:
                    for (int j = 1; j < Row-1; j++)
                    {
                        LockedCells.Add(new Vector2Int(j, i));
                    }
                    break;
                case LockedCellsLineType.One:
                    for (int j = 0; j < Row; j+=2)
                    {
                        LockedCells.Add(new Vector2Int(j, i));
                    }
                    break;
                case LockedCellsLineType.Two:
                    for (int j = 1; j < Row; j+=2)
                    {
                        LockedCells.Add(new Vector2Int(j, i));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if(lockedCellsHorizontalLineTypes.Length == 0) return;
        for (int i = 0; i < lockedCellsHorizontalLineTypes.Length; i ++)
        {
            switch (lockedCellsHorizontalLineTypes[i])
            {
                case LockedCellsLineType.None:
                    break;
                case LockedCellsLineType.CornersBlocked:
                    LockedCells.Add(new Vector2Int(i, 0));
                    LockedCells.Add(new Vector2Int(i, Col-1));
                    break;
                case LockedCellsLineType.All:
                    LockedLine(i, false);
                    break;
                case LockedCellsLineType.CornersNotBlocked:
                    for (int j = 1; j < Row-1; j++)
                    {
                        LockedCells.Add(new Vector2Int(i, j));
                    }
                    break;
                case LockedCellsLineType.One:
                    for (int j = 0; j < Row; j+=2)
                    {
                        LockedCells.Add(new Vector2Int(i, j));
                    }
                    break;
                case LockedCellsLineType.Two:
                    for (int j = 1; j < Row; j+=2)
                    {
                        LockedCells.Add(new Vector2Int(i, j));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void SetOpenOnlyOneLine()
    {
        var centerLine = (Row - 1) / 2;
        
        for (int i = 0; i < Row; i++)
        {
            if(i != centerLine) LockedLine(i);
        }
    }

    private void SetBlockedDiagonally()
    {
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                if ((i % 2 != 0 || j % 2 != 0) && !(i % 2 != 0 && j % 2 != 0)) continue;
                LockedCells.Add(new Vector2Int(j, i));
            }
        }
    }

    private void SetOpenOnlyTwoLine()
    {
        int oneLine = 0;
        int twoLine = 0;
        if (Row % 2 == 0)
        {
            oneLine = Row / 2;
            twoLine = Row / 2 - 1;
        }
        else
        {
            oneLine = (Row - 3) / 2;
            twoLine = (Row + 1) / 2;
        }
        for (int i = 0; i < Row; i++)
        {
            if(i != oneLine && i != twoLine) LockedLine(i);
        }
    }

    private void SetCornersBlocked()
    {
        LockedCells.Add(new Vector2Int(0, 0));
        LockedCells.Add(new Vector2Int(0, Row-1));
        LockedCells.Add(new Vector2Int(Col-1, 0));
        LockedCells.Add(new Vector2Int(Col-1, Row-1));
    }

    private void LockedLine(int coordinate, bool isVertical = true)
    {
        if (isVertical)
        {
            for (int i = 0; i < Col; i++)
            {
                LockedCells.Add(new Vector2Int(i, coordinate));
            }
        }
        else
        {
            for (int i = 0; i < Row; i++)
            {
                LockedCells.Add(new Vector2Int(coordinate, i));
            }
        }
    }
}

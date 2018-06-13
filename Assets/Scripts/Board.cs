using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    public OnInteractCellEvent onFlipCell = new OnInteractCellEvent();
    public UnityEvent onFlagCell = new UnityEvent();
    public UnityEvent onUnflagCell = new UnityEvent();

    [SerializeField] private int cellSize;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float flipDelay;

    private Cell[,] cells;
    private int width = 0;
    private int height = 0;
    private BoardState state;

    #region Public methods

    public void Init(int[,] boardData)
    {
        ClearBoard();

        state = BoardState.Flip;
        width = boardData.GetLength(1);
        height = boardData.GetLength(0);

        cells = new Cell[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(
                    (x - ((float)width - 1) / 2) * cellSize,
                    (-y + ((float)height - 1) / 2) * cellSize,
                    0);
                Cell cell = ObjectPool.Spawn(cellPrefab, transform, position).GetComponent<Cell>();

                cells[y, x] = cell;
                cell.Init(new Vector2(x, y), boardData[y, x], position);
                cell.RegisterCallback(OnClickCell, OnHoldCell);
            }
        }
    }

    public void Scale(float scaleFactor)
    {
        transform.localScale = Vector3.one * scaleFactor;
    }

    public void LockBoard()
    {
        state = BoardState.Locked;
    }

    public void ActivateFlipMode()
    {
        if (state == BoardState.Locked) return;
        state = BoardState.Flip;
    }

    public void ActivateFlagMode()
    {
        if (state == BoardState.Locked) return;
        state = BoardState.Flag;
    }

    public void FlipAllMine()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = cells[y, x];
                if (cell.IsMine && cell.IsFlippable)
                {
                    cell.Flip();
                }
            }
        }
    }
    #endregion

    #region Callback

    private void OnClickCell(Cell cell)
    {
        if (state == BoardState.Flip)
        {
            if (cell.IsFlippable)
            {
                ProcessFlipCell(cell);
            }
            else if (cell.IsFlipped)
            {
                ProcessFlipAdjacent(cell);
            }
        }
        else if (state == BoardState.Flag)
        {
            ProcessFlagCell(cell);
        }
    }

    private void OnHoldCell(Cell cell)
    {
        if (state == BoardState.Locked) return;
        Handheld.Vibrate();
        ProcessFlagCell(cell);
    }
    #endregion    

    private void ClearBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x].Clear();
                ObjectPool.Recycle(cells[y, x].gameObject);
            }
        }
    }

    private void ProcessFlipCell(Cell cell)
    {
        if (cell.IsEmpty)
        {
            StartCoroutine(FlipEmptyArea((int)cell.Coordinate.x, (int)cell.Coordinate.y));
            return;
        }

        FlipCell(cell);
        if (cell.IsMine)
        {
            cell.Display(Constants.DEAD_VALUE);
        }
    }

    private void ProcessFlipAdjacent(Cell cell)
    {
        if (IsCellAutoFlippable(cell))
            FlipAdjacentCells((int)cell.Coordinate.x, (int)cell.Coordinate.y);
        else
            HightlightAdjacentCells((int)cell.Coordinate.x, (int)cell.Coordinate.y);
    }

    private void ProcessFlagCell(Cell cell)
    {
        if (cell.IsFlaggable)
        {
            cell.Flag();
            onFlagCell.Invoke();
        }
        else if (cell.IsFlagged)
        {
            cell.Unflag();
            onUnflagCell.Invoke();
        }
    }

    private bool IsCellAutoFlippable(Cell cell)
    {
        int cellValue = cell.Value;
        int x = (int)cell.Coordinate.x;
        int y = (int)cell.Coordinate.y;

        int[,] adjacentOffsets = Constants.ADJACENT_OFFSETS;
        for (int index = 0; index < Constants.ADJACENT_NUMBER; index++)
        {
            int cellY = y + adjacentOffsets[index, 1];
            int cellX = x + adjacentOffsets[index, 0];
            if (!IsCoordinateValid(cellX, cellY)) continue;

            if (cells[cellY, cellX].IsFlagged)
                cellValue--;
        }

        return cellValue == 0;
    }

    private void FlipAdjacentCells(int x, int y)
    {
        int[,] adjacentOffsets = Constants.ADJACENT_OFFSETS;
        for (int index = 0; index < Constants.ADJACENT_NUMBER; index++)
        {
            int cellY = y + adjacentOffsets[index, 1];
            int cellX = x + adjacentOffsets[index, 0];
            if (!IsCoordinateValid(cellX, cellY)) continue;

            if (cells[cellY, cellX].IsFlippable)
                ProcessFlipCell(cells[cellY, cellX]);
        }
    }

    private void HightlightAdjacentCells(int x, int y)
    {
        int[,] adjacentOffsets = Constants.ADJACENT_OFFSETS;
        for (int index = 0; index < Constants.ADJACENT_NUMBER; index++)
        {
            int cellY = y + adjacentOffsets[index, 1];
            int cellX = x + adjacentOffsets[index, 0];
            if (!IsCoordinateValid(cellX, cellY)) continue;

            if (cells[cellY, cellX].IsFlippable)
                cells[cellY, cellX].Hightlight();
        }
    }

    private IEnumerator FlipEmptyArea(int x, int y)
    {
        if (!IsCoordinateValid(x, y)) yield break;

        Cell centerCell = cells[y, x];
        if (!centerCell.IsFlippable) yield break;

        FlipCell(centerCell);

        yield return new WaitForSeconds(flipDelay);

        if (centerCell.IsEmpty)
        {
            StartCoroutine(FlipEmptyArea(x - 1, y));
            StartCoroutine(FlipEmptyArea(x + 1, y));
            StartCoroutine(FlipEmptyArea(x, y - 1));
            StartCoroutine(FlipEmptyArea(x, y + 1));
        }
    }

    private void FlipCell(Cell cell)
    {
        cell.Flip();
        onFlipCell.Invoke(cell);
    }

    private bool IsCoordinateValid(int cellX, int cellY)
    {
        if (cellX < 0 || cellY < 0 || cellX >= width || cellY >= height) return false;
        return true;
    }
}

public class OnInteractCellEvent : UnityEvent<Cell> { }

public enum BoardState
{
    Flip,
    Flag,
    Locked
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardGenerator
{
    public static int[,] CreateBoard(DifficultyType difficulty)
    {
        BoardData boardData = ConfigManager.Instance.boardConfig.GetBoard(difficulty);

        int[,] board = InitBoardWithMine(
            boardData.width, boardData.height, boardData.mine);
        board = AddClueToBoard(board);

        return board;
    }

    private static int[,] InitBoardWithMine(int width, int height, int mine)
    {
        int emptySize = width * height - mine;
        int[] emptyBoard = new int[emptySize];
        List<int> board = new List<int>(emptyBoard);

        for (int i = 0; i < mine; i++)
        {
            board.Insert(Random.Range(0, emptySize + i), Constants.MINE_VALUE);
        }

        return Convert1DArrayTo2DArray(board.ToArray(), width, height);
    }

    private static int[,] Convert1DArrayTo2DArray(int[] source, int width, int height)
    {
        int[,] board = new int[height, width];

        for (int i = 0; i < source.Length; i++)
        {
            board[i / width, i % width] = source[i];
        }

        return board;
    }

    private static int[,] AddClueToBoard(int[,] board)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] == 0)
                    board[y, x] = GetClueValue(board, x, y);
            }
        }

        return board;
    }

    private static int GetClueValue(int[,] board, int x, int y)
    {
        int value = 0;
        int[,] adjacentOffsets = Constants.ADJACENT_OFFSETS;
        for (int index = 0; index < Constants.ADJACENT_NUMBER; index++)
        {
            int cellY = y + adjacentOffsets[index, 1];
            int cellX = x + adjacentOffsets[index, 0];
            if (cellX < 0 || cellY < 0 || cellX >= board.GetLength(1) || cellY >= board.GetLength(0))
                continue;

            if (board[cellY, cellX] == -1)
                value++;
        }

        return value;
    }

    private static void Log(int[] source)
    {
        string s = "";
        for (int i = 0; i < source.GetLength(0); i++)
        {
            s += source[i] + ", ";
        }

        Debug.Log(s);
    }

    private static void Log(int[,] source)
    {
        string s = "";
        for (int i = 0; i < source.GetLength(0); i++)
        {
            for (int j = 0; j < source.GetLength(1); j++)
            {
                s += source[i, j] + ", ";
            }

            s += "\n";
        }

        Debug.Log(s);
    }
}
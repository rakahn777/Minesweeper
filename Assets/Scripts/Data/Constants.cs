using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const int MINE_VALUE = -1;
    public const int DEAD_VALUE = -10;
    public const int GROUND_VALUE = 10;
    public const int FLAG_VALUE = 100;
    public const int EMPTY_VALUE = 0;

    public const int MAX_TIME = 999;

	public const int ADJACENT_NUMBER = 8;
    public static readonly int[,] ADJACENT_OFFSETS = new int[,] {
        {-1, 1},
        {0, 1},
        {1, 1},
        {-1, 0},
        {1, 0},
        {-1, -1},
        {0, -1},
        {1, -1},
    };
}

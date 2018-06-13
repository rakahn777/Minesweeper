using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class BoardConfig : ScriptableObject
{
	[SerializeField] private BoardData[] boards;

	public BoardData GetBoard(DifficultyType difficulty)
	{
		return boards [(int)difficulty];
	}
}

[System.Serializable]
public class BoardData
{
	public int width;
	public int height;
	public int mine;
	public float scaleFactor;
}

public enum DifficultyType
{
	Easy	= 0,
	Medium	= 1,
	Hard	= 2
}
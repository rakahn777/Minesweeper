using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField] private Board board;
	[SerializeField] private GameUIController gameUI;
    [SerializeField] private Text mineText;
    [SerializeField] private Text timeText;

    private GameState currentState;
	private DifficultyType currentDifficulty;
    private int safeCellCount;
    private int mineCount;
    private float timeCount;

    void Start()
    {
        Init();
        //Reset((int)DifficultyType.Easy);
    }

    void OnDestroy()
    {
        board.onFlipCell.RemoveAllListeners();
        board.onFlagCell.RemoveAllListeners();
		board.onUnflagCell.RemoveAllListeners();
    }

    private void Init()
    {
        board.onFlipCell.AddListener(OnFlipCell);
        board.onFlagCell.AddListener(OnFlagCell);
        board.onUnflagCell.AddListener(OnUnflagCell);
    }

    private void ResetBoard(DifficultyType difficulty)
    {
        int[,] boardData = BoardGenerator.CreateBoard(difficulty);
        board.Init(boardData);
        board.Scale(ConfigManager.Instance.boardConfig.GetBoard(difficulty).scaleFactor);
    }

    private void ResetGameStats(DifficultyType difficulty)
    {
        BoardData boardData = ConfigManager.Instance.boardConfig.GetBoard(difficulty);
        timeCount = 0;
        mineCount = boardData.mine;
        safeCellCount = boardData.width * boardData.height - boardData.mine;
        currentState = GameState.Running;

        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
        mineText.text = mineCount.ToString();
        timeText.text = ((int)timeCount).ToString("d3");
    }

    private void Update()
    {
        if (currentState != GameState.Running) return;

        timeCount += Time.deltaTime;
        UpdateStatsDisplay();
        if (timeCount > Constants.MAX_TIME)
        {
            Lose();
        }
    }

    private void Win()
    {
        currentState = GameState.Finished;
        board.LockBoard();
		gameUI.ShowResult(true);
    }

    private void Lose()
    {
        currentState = GameState.Finished;
        board.LockBoard();
		gameUI.ShowResult(false);
    }

    private void OnFlipCell(Cell cell)
    {
        if (cell.IsMine)
        {
            Lose();
            board.FlipAllMine();
        }
        else
        {
            safeCellCount--;
            if (safeCellCount == 0)
            {
                Win();
            }
        }
    }

    private void OnFlagCell()
    {
		mineCount--;
    }

	private void OnUnflagCell()
	{
		mineCount++;
	}

    #region Public methods

    public void Reset(int difficulty)
    {
		currentDifficulty = (DifficultyType)difficulty;
        ResetBoard(currentDifficulty);
        ResetGameStats(currentDifficulty);
		gameUI.Init();
    }

	public void Replay()
	{
		Reset((int)currentDifficulty);
	}

	public void SetInteractMode(bool isFlip)
	{
		if (isFlip)
			board.ActivateFlipMode();
		else
			board.ActivateFlagMode();
	}

	public void PauseGame()
	{
		if (currentState == GameState.Running)
			currentState = GameState.Pause;
	}

	public void UnpauseGame()
	{
		if (currentState == GameState.Pause)		
			currentState = GameState.Running;
	}

    #endregion
}

public enum GameState
{
    Running,
	Pause,
    Finished
}

using UnityEngine;
using System.Collections;

public class GameUIController : MonoBehaviour
{
	[SerializeField] private GameObject winObj;
	[SerializeField] private GameObject loseObj;
	[SerializeField] private GameObject pausePopup;

	public void Init()
	{
		winObj.SetActive(false);
		loseObj.SetActive(false);
	}

	public void ShowResult(bool isWin)
	{
		winObj.SetActive(isWin);
		loseObj.SetActive(!isWin);
	}

	public void ShowPause()
	{
		pausePopup.SetActive(true);
	}

	public void HidePause()
	{
		pausePopup.SetActive(false);
	}
}

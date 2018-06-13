using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private float holdThreshold;

    // Flip animation specs
    [Header("Flip Animation")]
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float flipTime;
    [SerializeField] private float changeIconTime;
    [SerializeField] private float highlightDelayTime;
    private static readonly Vector3 targetScale = new Vector3(0.2f, 1, 1);
    //---

    private int cellValue;
    private Vector2 coordinate;
    private CellState state;
    private float flipDuration;
    private float holdDuration;
    private bool isHoldTriggered;

    private InteractCellEvent onClickCell = new InteractCellEvent();
    private InteractCellEvent onHoldCell = new InteractCellEvent();

    #region Properties
    public Vector2 Coordinate
    {
        get { return coordinate; }
    }

    public int Value
    {
        get { return cellValue; }
    }

    public bool IsEmpty
    {
        get { return cellValue == Constants.EMPTY_VALUE; }
    }

    public bool IsMine
    {
        get { return cellValue < 0; }
    }

    public bool IsFlippable
    {
        get { return state == CellState.Ready; }
    }

    public bool IsFlipped
    {
        get { return state == CellState.Flipped; }
    }

    public bool IsFlaggable
    {
        get { return state == CellState.Ready; }
    }

    public bool IsFlagged
    {
        get { return state == CellState.Flagged; }
    }
    #endregion

    public void Init(Vector2 coordinate, int value, Vector3 position)
    {
        state = CellState.Ready;
        this.coordinate = coordinate;
        cellValue = value;
        flipDuration = 0;
        transform.localScale = Vector3.one;
        // icon.raycastTarget = true;
        // button.interactable = true;
        Display(Constants.GROUND_VALUE, false);
    }

    public void RegisterCallback(UnityAction<Cell> clickCallback, UnityAction<Cell> holdCallback)
    {
        onClickCell.AddListener(clickCallback);
        onHoldCell.AddListener(holdCallback);
    }

    public void Clear()
    {
        onClickCell.RemoveAllListeners();
        onHoldCell.RemoveAllListeners();
    }

    public void Flip()
    {
        if (!IsFlippable) return;

        state = CellState.Flipped;
        // icon.raycastTarget = false;
        // button.interactable = false;
        Display(cellValue);
    }

    public void Flag()
    {
        if (!IsFlaggable) return;

        state = CellState.Flagged;
        Display(Constants.FLAG_VALUE);
    }

    public void Unflag()
    {
        if (!IsFlagged) return;

        state = CellState.Ready;
        Display(Constants.GROUND_VALUE);
    }

    public void Hightlight()
    {
        if (!IsFlippable) return;

        StartCoroutine(PlayHighlightAnimation());
    }

    public void Display(int value, bool displayFlip = true)
    {
        Sprite sprite = ConfigManager.Instance.textureConfig.GetTexture(value);

        if (displayFlip)
            StartCoroutine(PlayFlipAnimation(sprite));
        else
            icon.sprite = sprite;
    }

    private IEnumerator PlayFlipAnimation(Sprite sprite)
    {
        flipDuration = flipTime;

        yield return new WaitForSeconds(changeIconTime);

        icon.sprite = sprite;
    }

    private IEnumerator PlayHighlightAnimation()
    {
        Display(Constants.EMPTY_VALUE, true);
        
        yield return new WaitForSeconds(flipTime + highlightDelayTime);
        if (state != CellState.Ready) yield break;

        Display(Constants.GROUND_VALUE, true);        
    }

    private void Update()
    {
        if (flipDuration > 0)
        {
            flipDuration -= Time.deltaTime;

            float value = curve.Evaluate(1 - flipDuration / flipTime);
            icon.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, value);
        }

        if (holdDuration > 0)
        {
            holdDuration -= Time.deltaTime;

            if (holdDuration <= 0)
            {
                isHoldTriggered = true;
                onHoldCell.Invoke(this);
            }
        }
    }

    #region Events
    public void OnPress()
    {
        holdDuration = holdThreshold;
        isHoldTriggered = false;
        // Debug.Log("OnPress " + coordinate);
    }

    public void OnRelease()
    {
        holdDuration = -1;
        // Debug.Log("OnRelease " + coordinate);
    }

    public void OnClick()
    {
        if (isHoldTriggered) return;
        onClickCell.Invoke(this);
        // Debug.Log("OnClick " + coordinate);
    }
    #endregion
}

public class InteractCellEvent : UnityEvent<Cell> { }

public enum CellState
{
    Ready,
    Flipped,
    Flagged
}

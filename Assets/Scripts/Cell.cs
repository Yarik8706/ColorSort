using DG.Tweening;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Color Color { get; private set; }
    public Vector2Int Position { get; set; }

    public bool IsStartTweenPlaying => startAnimation.IsActive();
    public bool IsStartMovePlaying => startMoveAnimation.IsActive();
    public bool HasSelectedMoveFinished => !selectedMoveAnimation.IsActive();
    public bool HasMoveFinished => !moveAnimation.IsActive();
    
    [SerializeField] private float startScaleDelay = 0.04f;
    [SerializeField] private float startScaleTime = 0.2f;
    [SerializeField] private float startMoveAnimationTime = 0.32f;
    [SerializeField] private float selectedMoveAnimationTime = 0.16f;
    [SerializeField] private float moveAnimationTime = 0.32f;
    [SerializeField] private string frontLayerName;
    [SerializeField] private string backLayerName;
    [SerializeField] private GameObject lokedCellMark;
    public SpriteRenderer spriteRenderer;
    
    private Tween startAnimation;
    private Tween startMoveAnimation;
    private Tween selectedMoveAnimation;
    private Tween moveAnimation;
    private LevelControl _levelControl;
    private Vector3 baseScale = Vector3.one;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void Init(Color color, LevelControl levelControl, int x, int y)
    {
        _levelControl = levelControl;
        Color = color;
        Position = new Vector2Int(x, y);
        
        spriteRenderer.color = Color;
    }

#if UNITY_EDITOR
    public void StartAnimInEditor(int x, int y, int Col, int Row)
    {
        baseScale = new Vector3(
            8 / Col,
            8 / Row,
            1);
        transform.localPosition = new Vector3(
                x * (8
                     / Col
                    ) - 4 + baseScale.x/2, 
                y * (8 
                     / Row
                     ) - 4 + baseScale.y/2, 
                0);
        transform.DOLocalMove(
            new Vector3(
                x * (8 
                              / Col
                ) - 4 + baseScale.x/2, 
                y * (8 
                              / Row
                ) - 4 + baseScale.y/2, 
                0),
            startMoveAnimationTime
        ).SetEase(Ease.InSine)
            .Play();
    }
#endif

    public void InitScaleAnimation(int x, int y)
    {
        float delay = (x + y) * startScaleDelay;
        startAnimation = transform.DOScale(baseScale.x, startScaleTime);
        startAnimation.SetEase(Ease.OutExpo);
        startAnimation.SetDelay(0.5f + delay);
    }

    public void AnimateStartScale()
    {
        InitScaleAnimation(Position.x, Position.y);
        startAnimation.Play();
    }

    public void GameFinished()
    {
        float delay = (Position.x + Position.y) * startScaleDelay;
        startAnimation = transform.DOScale(transform.localScale.x/2, startScaleTime);
        startAnimation.SetLoops(2, LoopType.Yoyo);
        startAnimation.SetEase(Ease.InOutExpo);
        startAnimation.SetDelay(0.5f + delay);
        startAnimation.Play();
    }

    [ContextMenu("AnimateStartPosition")]
    public void AnimateStartPosition()
    {
        startMoveAnimation = StartMoveAnimationToPosition(startMoveAnimationTime);
        startMoveAnimation.SetEase(Ease.InSine);
        startMoveAnimation.Play();
    }

    private Tween StartMoveAnimationToPosition(float time)
    {
        return transform.DOLocalMove(
            new Vector3(
                Position.x * (GameManager.GamePlaceSize.x 
                              / _levelControl.currentlevelData.Col
                ) - GameManager.GamePlaceSize.x/2 + baseScale.x/2, 
                Position.y * (GameManager.GamePlaceSize.y 
                              / _levelControl.currentlevelData.Row
                ) - GameManager.GamePlaceSize.y/2 + baseScale.y/2, 
                0),
            time
        );  
    }

    public void SelectedMoveStart()
    {
        spriteRenderer.sortingLayerName = frontLayerName;
        transform.localScale *= 1.2f;
    }

    public void SelectedMove(Vector3 mousePosition)
    {
        float minX = -4;
        float maxX = 4;
        float minY = -4f;
        float maxY = 4;
        if (mousePosition.x < minX)
        {
            mousePosition.x = minX;
        }
        if (mousePosition.x > maxX)
        {
            mousePosition.x = maxX;
        }
        if (mousePosition.y < minY)
        {
            mousePosition.y = minY;
        }
        if (mousePosition.y > maxY)
        {
            mousePosition.y = maxY;
        }
        transform.position = mousePosition;
    }

    public void SelectedMoveEnd()
    {
        selectedMoveAnimation = StartMoveAnimationToPosition(selectedMoveAnimationTime);
        selectedMoveAnimation.SetEase(Ease.InSine);
        selectedMoveAnimation.onComplete = () =>
        {
            spriteRenderer.sortingLayerName = backLayerName;
            transform.localScale /= 1.2f;
        };
        selectedMoveAnimation.Play();
    }

    public void SetLockedCell()
    {
        lokedCellMark.SetActive(true);
    }

    public void MoveEnd()
    {
        spriteRenderer.sortingLayerName = frontLayerName;
        moveAnimation = StartMoveAnimationToPosition(moveAnimationTime);
        moveAnimation.onComplete = () =>
        {
            spriteRenderer.sortingLayerName = backLayerName;
        };
        moveAnimation.Play();
    }
    
    public Tween MoveEnd(Vector3 nextPosition)
    {
        spriteRenderer.sortingLayerName = frontLayerName;
        moveAnimation = transform.DOLocalMove(
            nextPosition,
            moveAnimationTime
        );
        moveAnimation.onComplete = () =>
        {
            spriteRenderer.sortingLayerName = backLayerName;
        };
        return moveAnimation.Play();
    }
}

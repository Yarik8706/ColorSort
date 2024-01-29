using DG.Tweening;
using UnityEngine;

public class ActiveElement : MonoBehaviour
{
    [SerializeField] private bool startIsHidePos;
    [SerializeField] private Transform hidePosition;
    public Vector3 ActivePosition { get; set; }

    private void Start()
    {
        if(ActivePosition == Vector3.zero) ActivePosition = transform.position;
        if (startIsHidePos) transform.position = hidePosition.position;
    }

    public Tween ChangeActive(bool isActive)
    {
        return transform.DOMove(isActive ? ActivePosition : hidePosition.position, 0.7f).SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }
    
    public Tween ChangeActiveAndSetNotActivePosition(bool isActive)
    {
        transform.position = !isActive ? ActivePosition : hidePosition.position;
        return transform.DOMove(isActive ? ActivePosition : hidePosition.position, 0.7f).SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }
}
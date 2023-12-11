using System;
using DG.Tweening;
using UnityEngine;

public class BackButtonControl : MonoBehaviour
{
    [SerializeField] private Transform positionInGame;
    private Vector3 _startPosition;

    public static BackButtonControl Instance;

    private void Start()
    {
        Instance = this;
        _startPosition = transform.position;
    }

    public void BackToMenu()
    {
        GameManager.instance.BackToMenu(false,false);
    }

    public void SetGameState(bool isMenu)
    {
        if (!isMenu)
        {
            transform.DOMove(positionInGame.transform.position, 0.7f).SetLink(gameObject)
                .SetEase(Ease.InOutExpo).Play();
            transform.DOScale(2f, 0.7f).SetEase(Ease.InSine).Play();;
        }
        else
        {
            transform.DOMove(_startPosition, 0.7f)
                .SetEase(Ease.InOutExpo).SetLink(gameObject).Play();
            transform.DOScale(1f, 0.7f).SetEase(Ease.InSine).Play();;
        }
    }
}
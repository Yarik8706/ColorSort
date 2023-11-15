using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LvlsControl : MonoBehaviour
{
    public int CurrentIndex { get; private set; }

    public List<LevelControl> LvlsControls { get; } = new();

    [SerializeField] private Level[] levelsData;
    [SerializeField] private GameObject levelControlPrefab;
    [SerializeField] private Cell cellPrefab;

    public ActiveElement activeElement { get; private set; }
    private float _betweenLvlsLenght = 10;
    private int _prefLvlIndex = 1;

    private void Start()
    {
        activeElement = GetComponent<ActiveElement>();
    }

    private void GenerateLvls()
    {
        if(LvlsControls.Count != 0) return;
        foreach (var data in levelsData)
        {
            var levelControl = Instantiate(levelControlPrefab, transform).GetComponent<LevelControl>();
            levelControl.GenerateLvl(cellPrefab, data);
            LvlsControls.Add(levelControl);
            levelControl.gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        activeElement.ChangeActive(true);
        GenerateLvls();
        LvlsControls[_prefLvlIndex].transform.position = Vector3.right * -_betweenLvlsLenght;
        UIControl.Instance.nextLvlButton.SetActive(CurrentIndex != levelsData.Length-1);
        UIControl.Instance.prefLvlButton.SetActive(CurrentIndex != 0);
        UpdateActive();
    }

    public void SwitchOff()
    {
        activeElement.ChangeActive(false);
        HideAllLvls();
    }

    private void ShowCurrentCard(bool isNext = true)
    {
        UpdateActive();
        if (isNext)
        {
            MoveLvl(_prefLvlIndex, -_betweenLvlsLenght);
            MoveLvl(CurrentIndex, 0, _betweenLvlsLenght);
        }
        else
        {
            MoveLvl(_prefLvlIndex, _betweenLvlsLenght);
            MoveLvl(CurrentIndex, 0, -_betweenLvlsLenght);
        }
    }

    public void UpdateActive()
    {
        UIControl.Instance.SetLvlCount(LvlsControls.ToArray());
        UIControl.Instance.ChangeSelectedLevel(CurrentIndex,
            LvlsControls[CurrentIndex].currentlevelData.hasPassed);
        foreach (var lvlControl in LvlsControls)
        {
            if (lvlControl == LvlsControls[_prefLvlIndex])
            { 
                lvlControl.gameObject.SetActive(true); 
                continue;
            }
            if (lvlControl == LvlsControls[CurrentIndex])
            {
                lvlControl.gameObject.SetActive(true); 
                lvlControl.InitCells();
                continue;
            }

            lvlControl.gameObject.SetActive(false);
        }
    }
    
    private void MoveLvl(int index, float end, float start)
    {
        LvlsControls[index].transform.position = Vector3.right * start;
        LvlsControls[index]
            .transform.DOMove(Vector3.right * end, 0.7f).SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }

    private void MoveLvl(int index, float end)
    {
        LvlsControls[index]
            .transform.DOMove(Vector3.right * end, 0.7f).SetLink(gameObject)
            .SetEase(Ease.InOutExpo).Play();
    }

    public void ShowNext()
    {
        _prefLvlIndex = CurrentIndex;
        CurrentIndex++;
        if (CurrentIndex == LvlsControls.Count - 1)
        {
            UIControl.Instance.nextLvlButton.SetActive(false);
        }
        UIControl.Instance.prefLvlButton.SetActive(true);
        ShowCurrentCard();
    }

    public void ShowPrevious()
    {
        _prefLvlIndex = CurrentIndex;
        CurrentIndex--;
        if (CurrentIndex == 0)
        {
            UIControl.Instance.prefLvlButton.SetActive(false);
        }
        UIControl.Instance.nextLvlButton.SetActive(true);
        ShowCurrentCard(false);
    }

    public void ChangeOtherLvlsActive(bool isActive)
    {
        foreach (var control in LvlsControls)
        {
            control.gameObject.SetActive(control == LvlsControls[CurrentIndex] || isActive);
        }
    }

    public void HideAllLvls()
    {
        foreach (var control in LvlsControls)
        {
            control.gameObject.SetActive(false);
        }
    }
}
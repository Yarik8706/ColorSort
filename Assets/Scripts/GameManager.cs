using System.Collections;
using DG.Tweening;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static readonly Vector2 GamePlaceSize = new(8, 8);

    public bool HasGameFinished { get; private set; }
    private int ActiveDifficultIndex { get; set; } = -1;
    public string[] PassedLvlsName { get; private set; }

    [SerializeField] private LvlsControl[] lvlsControls;
    [SerializeField] private PerformLvlEffect startEffect;

    private LevelControl _levelControl;

    private void Awake()
    {
#if UNITY_EDITOR
        YandexGame.ResetSaveProgress();
#endif
        instance = this;
        StartCoroutine(LoadPassedLvlsName());
        DOTween.SetTweensCapacity(500, 50);
        DOTween.defaultAutoPlay = AutoPlay.None;
    }

    private IEnumerator LoadPassedLvlsName()
    {
        yield return new WaitUntil(() => YandexGame.SDKEnabled);
        PassedLvlsName = YandexGame.savesData.levelsData.Split(";");
    }

    public bool CheckPassedLevel(string name)
    {
        foreach (var s in PassedLvlsName)
        {
            if (s == name) return true;
        }

        return false;
    }

    [ContextMenu("ResetPassedLvlsName")]
    public void ResetPassedLvlsName()
    {
        YandexGame.ResetSaveProgress();
    }

    private void Start()
    {
        // playStartTween = _playButtonTransform
        //     .DOScale(_playButtonTransform.localScale * 1.1f, 1f)
        //     .SetEase(Ease.Linear)
        //     .SetLoops(-1, LoopType.Yoyo);
        // playStartTween.Play();
        ChangeLvlsDifficult(-1);
    }

    public void ClickedPlayButton()
    {
        _levelControl = lvlsControls[ActiveDifficultIndex]
            .LvlsControls[lvlsControls[ActiveDifficultIndex]
                .CurrentIndex].GetComponent<LevelControl>();

        if(!_levelControl.CheckCellsAnimationEnd()) return;
        HasGameFinished = false;
        _levelControl.Init();
        _levelControl.transform.DOScale(1, 0.7f).SetEase(Ease.OutExpo).Play();
        lvlsControls[ActiveDifficultIndex].ChangeOtherLvlsActive(false);
        UIControl.Instance.ChangeUIElementsActive(false);
    }

    public void NextLvl()
    {
        lvlsControls[ActiveDifficultIndex].ShowNext();
    }

    public void PrefLvl()
    {
        lvlsControls[ActiveDifficultIndex].ShowPrevious();
    }

    [ContextMenu("Win")]
    public void Win()
    {
        HasGameFinished = true;
        var isFirstWin = !_levelControl.currentlevelData.hasPassed;
        if (!_levelControl.currentlevelData.hasPassed)
        {
            YandexGame.savesData.levelsData += _levelControl.currentlevelData.name + ";";
            _levelControl.currentlevelData.hasPassed = true;
        };
        BackToMenu(true, isFirstWin);
    }

    public void ChangeLvlsDifficult(int newLvls)
    {
        if(ActiveDifficultIndex != -1)
        {
            var oldActiveDifficultIndex = ActiveDifficultIndex;
            lvlsControls[ActiveDifficultIndex].activeElement
                .ChangeActive(false).OnKill(() =>
            {
                lvlsControls[oldActiveDifficultIndex].SwitchOff();
                if (newLvls == -1) return;
                lvlsControls[newLvls].Initialize();
            });
        }
        else if(newLvls != -1)
        {
            lvlsControls[newLvls].Initialize();
        }
        ActiveDifficultIndex = newLvls;
        UIControl.Instance.ChangeUIStateWhenDifficultChanged(newLvls != -1);
    }

    public void BackToMenu(bool isGame, bool isFirstWin)
    {
        if (_levelControl == null)
        {
            ChangeLvlsDifficult(-1);
            return;
        }
        _levelControl.ResetLevel();
        UIControl.Instance.ChangeSelectedLevel(-1, true);
        _levelControl.transform.DOScale(0.6f, 0.7f)
            .SetEase(Ease.OutExpo).Play().OnComplete(() =>
            {
                if(!isGame) return;
                startEffect.StartEffect();
                if(!isFirstWin) return;
                UIControl.Instance.UpdateGameCount();
            });
        _levelControl = null;
        lvlsControls[ActiveDifficultIndex].ChangeOtherLvlsActive(true);
        UIControl.Instance.ChangeUIElementsActive(true);
        lvlsControls[ActiveDifficultIndex].UpdateActive();
    }
}

public class Animal
{
    public string name;
    public int age;

    public virtual void AddOneToAge()
    {
        age++;
    }
}

public class Cat : Animal
{
    Cat(string name, int age)
    {
        this.name = name;
        this.age = age;
        AddOneToAge();
    }

    public override void AddOneToAge()
    {
        age += 2;
        base.AddOneToAge();
    }
}




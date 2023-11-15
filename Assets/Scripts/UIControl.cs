using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class UIControl : MonoBehaviour
{
    public GameObject nextLvlButton;
    public GameObject prefLvlButton;

    public int gameCount;
    
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private TMP_Text lvlsCountText;
    [SerializeField] private ActiveElement menuDifficultChange;
    [SerializeField] private ActiveElement[] activeUIElements;
    [SerializeField] private GameObject playButton, replayButton;
    [SerializeField] private TMP_Text gameCountText;

    public static UIControl Instance { get; private set; }

    private void Awake()
    {
        StartCoroutine(GetLeaderboardScore());
        Instance = this;
    }

    private IEnumerator GetLeaderboardScore()
    {
        yield return new WaitUntil(() => YandexGame.SDKEnabled);
        gameCount = YandexGame.savesData.rating;
        gameCountText.text = gameCount.ToString();
    }

    public void UpdateGameCount()
    {
        YandexGame.NewLeaderboardScores("rating", gameCount + 10);
        YandexGame.savesData.rating = gameCount + 10;
        YandexGame.SaveProgress();
        DOTween.To(value =>
            {
                gameCount = (int) value;
                gameCountText.text = gameCount.ToString();
            },
            gameCount, gameCount + 10, 1f);
    }

    public void ChangeUIElementsActive(bool isActive)
    {
        foreach (var element in activeUIElements)
        {
            element.ChangeActive(isActive);
        }
        BackButtonControl.Instance.SetGameState(isActive);
    }

    public void ChangeUIStateWhenDifficultChanged(bool state)
    {
        foreach (var element in activeUIElements)
        {
            element.ChangeActive(state);
        }
        menuDifficultChange.ChangeActive(!state);
    }

    public void ChangeSelectedLevel(int index, bool hasPassed)
    {
        playButton.SetActive(!hasPassed);
        replayButton.SetActive(hasPassed);
        if(index != -1) levelNumberText.text = (index + 1) + "";
    }

    public void SetLvlCount(LevelControl[] levelControls)
    {
        var passedCount = 0;
        foreach (var levelControl in levelControls)
        {
            if (levelControl.currentlevelData.hasPassed) passedCount++;
        }

        lvlsCountText.text = passedCount + "/" + levelControls.Length;
    }

    public void BackToMainMenuScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }
}
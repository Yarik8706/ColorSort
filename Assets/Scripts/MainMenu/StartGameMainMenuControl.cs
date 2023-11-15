using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameMainMenuControl : MonoBehaviour
{
    public void StartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }
}
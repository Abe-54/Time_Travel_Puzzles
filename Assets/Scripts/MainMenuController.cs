using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject[] tutorials;

    public Image transitionOverlay;

    public int currentTutorial = 0;

    public void PlayGame()
    {
        mainMenuScreen.SetActive(false);
        tutorials[currentTutorial].SetActive(true);
    }

    public void NextTutorial()
    {
        tutorials[currentTutorial++].SetActive(false);
        tutorials[currentTutorial].SetActive(true);
    }

    public void StartGame()
    {
        transitionOverlay.gameObject.SetActive(true);
        transitionOverlay.DOFade(1, 1f / 2).OnComplete(() =>
        {
            SceneManager.LoadScene("Demo Level");
            transitionOverlay.DOFade(0, 1f / 2).OnComplete(() =>
            {
                transitionOverlay.gameObject.SetActive(false);
            });
        });
    }
}

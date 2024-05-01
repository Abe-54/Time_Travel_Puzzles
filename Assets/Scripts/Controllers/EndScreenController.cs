using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    public Image transitionImage;

    public void MainMenu()
    {
        Sequence sequence = DOTween.Sequence();

        transitionImage.gameObject.SetActive(true);

        sequence.Append(transitionImage.DOFade(1, 1)).onComplete += () =>
        {
            SceneManager.LoadScene("Main Menu");
        };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PresentState : TimeSwapBaseState
{
    public GameObject[] presentObjects;
    public Sprite presentBackground;
    public AudioSource presentAudioSource;

    public override void EnterState()
    {
        backgroundSprite.sprite = presentBackground;

        if (Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            presentAudioSource.Play();
            presentAudioSource.DOFade(gameManager.settings.volume, 1.0f);
        }

        foreach (GameObject gameObject in presentObjects)
        {
            gameObject.SetActive(true);
        }
    }

    public override void Do()
    {
    }

    public override void ExitState()
    {
        if (Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) presentAudioSource.DOFade(0, 1.0f).onComplete += () => presentAudioSource.Pause();

        foreach (GameObject gameObject in presentObjects)
        {
            gameObject.SetActive(false);
        }
    }
}

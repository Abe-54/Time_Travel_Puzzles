using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public string nextLevel;

    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Beat Level");

            Sequence levelEndSequence = DOTween.Sequence();
            levelEndSequence.Append(Helpers.Fade(uiManager.transitionOverlay, 0f, 1f, 1f))
                .AppendInterval(1f)
                .OnComplete(() => SceneManager.LoadScene(nextLevel));
        }
    }
}

using UnityEngine;
using System;
using DG.Tweening;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public TimeSwapBaseState currentState { get; private set; }

    public TimeSwapBaseState presentState;
    public TimeSwapBaseState pastState;

    public SpriteRenderer backgroundSprite;
    public float orignalGravity;

    public SettingsSO settings;

    public event Action OnTimePeriodChanged;

    private UIManager uiManager;
    private PlayerController player;

    public bool isGameActive = false;

    void Awake()
    {
        currentState = presentState;
    }


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();

        presentState.SetupState(this, player, backgroundSprite);
        pastState.SetupState(this, player, backgroundSprite);

        orignalGravity = player.GetComponent<Rigidbody2D>().gravityScale;
    }

    public void SwapTimePeriod()
    {
        Vector2 playerVelocityBeforeSwap = player.GetComponent<Rigidbody2D>().velocity;

        player.isTimeTraveling = true;

        // Freeze Everything for a moment to swap environments
        currentState.FreezeEverything();

        uiManager.TriggerTransition(1.0f, () =>
        {
            SwapState();

            OnTimePeriodChanged?.Invoke();

            player.isTimeTraveling = false;
            player.GetComponent<Rigidbody2D>().velocity = playerVelocityBeforeSwap;

            currentState.UnfreezeEverything();
        });
    }

    public void SwapState()
    {
        currentState.ExitState();

        switch (currentState)
        {
            case PresentState:
                currentState = pastState;
                break;
            case PastState:
                currentState = presentState;
                break;
        }

        currentState.EnterState();
    }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(GameManager))]

public class GameManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager)target;

        // display the current state and split the name by uppercase letters
        GUILayout.Label("Current State: " + gameManager.currentState.GetType().Name.Split("State")[0] + " State");

        if (GUILayout.Button("Swap Time Period"))
        {
            gameManager.SwapState();
        }
    }
}

#endif
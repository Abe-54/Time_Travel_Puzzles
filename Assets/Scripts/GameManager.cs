using UnityEngine;
using System;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public TimeSwapBaseState currentState { get; private set; }

    public TimeSwapBaseState presentState;
    public TimeSwapBaseState pastState;

    public SpriteRenderer backgroundSprite;
    public float orignalGravity;

    public event Action OnTimePeriodChanged;

    private UIManager uiManager;
    private PlayerController player;

    void Awake()
    {
        currentState = presentState;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();

        currentState = presentState;

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

        if (GUILayout.Button("Swap Time Period"))
        {
            gameManager.SwapState();
        }
    }
}

#endif
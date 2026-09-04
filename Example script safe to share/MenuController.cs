/*
    MenuController
    --------------
    Central controller for the game's main menu system. Handles menu state
    transitions, UI routing, and scene navigation based on player data.

    This script manages:
    - A simple enum-driven state machine for menu navigation.
    - Delegation of UI display logic to the UiManager.
    - Safe singleton initialization and dependency acquisition.
    - Routing to Credits and Achievements via the CreditsRouter.
    - Conditional scene loading depending on player profile state.

    Designed for clarity and maintainability, this controller keeps menu
    behaviour modular and predictable while separating UI logic from scene
    flow and game data.
*/

using UnityEngine;

public class MenuController : MonoBehaviour
{
    internal static MenuController Instance { get; private set; }
    private GameManager gameManager;
    private PlayerData playerData;
    private UiManager uiManager;

    internal enum MenuState { Main, Options, Graphics, Audio, Reset }

    private MenuState currentState;
    internal MenuState CurrentState => currentState;
    private MenuState lastState;
    internal MenuState LasteState => lastState;

    internal void SwitchToMain() => ChangeState(MenuState.Main);
    internal void SwitchToOptions() => ChangeState(MenuState.Options);
    internal void SwitchToGraphics() => ChangeState(MenuState.Graphics);
    internal void SwitchToAudio() => ChangeState(MenuState.Audio);


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameManager = GameManager.Instance;
        playerData = gameManager._PlayerData;
        uiManager = FindAnyObjectByType<UiManager>();
        if (uiManager == null)
        {
            Debug.LogError("UiManager script not found in the scene!");
            return;
        }
    }

    private void Start()
    {
        ChangeState(MenuState.Main);
    }

    internal void ChangeState(MenuState newState)
    {
        lastState = currentState;
        currentState = newState;
        HandleState(newState);
    }

    private void HandleState(MenuState state)
    {
        switch (state)
        {
            case MenuState.Main:
                OnMain();
                break;

            case MenuState.Options:
                OnOptions();
                break;

            case MenuState.Graphics:
                OnGraphics();
                break;

            case MenuState.Audio:
                OnAudio();
                break;
            
            case MenuState.Reset:
                OnReset();
                break;
        }
    }
    public void OnReset()
    {
        uiManager.ShowReset();
    }

    private void OnMain()
    {
        uiManager.ShowMain();
    }

    private void OnOptions()
    {
        uiManager.ShowOptions();
    }

    private void OnGraphics()
    {
        uiManager.ShowGraphics();
    }

    private void OnAudio()
    {
        uiManager.ShowAudio();
    }
    public void OnAchievements()// lesters function
    {
        // Hijack: first-ever visit (this career save) gets routed through
        // Credits first, then auto-continues to Achievements after 10s.
        // Every visit after that goes straight to Achievements, same as
        // this method always did.
        //playerData = gameManager._PlayerData;
        CreditsRouter.GoToAchievementsButtonPressed(playerData);
    }
    public void OnCredits()// lesters function
    {
        // Options menu's Credits button - always goes to Credits directly,
        // no flag check here (that's handled inside CreditsController: it
        // shows the Back button immediately if already viewed, or runs the
        // 10s timer first if not).
        CreditsRouter.GoToCreditsFromOptions();
    }
    
    public void OnStartGame()
    {
        if (playerData.Get_PlayerSelectionCheck()) { SceneManager.Instance.LoadScene(SceneManager.Scene.StartOfDay); }
        else { SceneManager.Instance.LoadScene(SceneManager.Scene.Purpose); }
    }
}

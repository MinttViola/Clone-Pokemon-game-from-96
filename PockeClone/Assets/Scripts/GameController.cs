using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Free,
    Battle,
    Dialog,
    Cutscene,
    Paused
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera mainCamera;

    GameState state;
    GameState prevstate;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public static GameController Instance { get; private set; }
    TrainerControl? trainer;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        switch (state)
        {
            case GameState.Free:
                playerController.HundleUpdate();
                if (Input.GetKeyDown(KeyCode.F5))
                    SavingSystem.i.Save("saveSlot1");
                
                if (Input.GetKeyDown(KeyCode.F6))
                    SavingSystem.i.Load("saveSlot1");
                
                break;
            case GameState.Battle:
                battleSystem.HundleUpdate();
                break;
            case GameState.Dialog:
                DialogManagerScript.Instance.HandleUpdate();
                break;
        }
        
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        DialogManagerScript.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManagerScript.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.Free;
        };
    }

    public void PausedGame(bool pause)
    {
        if (pause)
        {
            prevstate = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevstate;
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = CurrentScene.GetComponent<MapAera>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        battleSystem.BattleStart(playerParty, wildPokemonCopy);
    }

    public void StartTrainerBattle(TrainerControl trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<PokemonParty>();

        battleSystem.TrainerBattleStart(playerParty, trainer.GetComponent<PokemonParty>());
    }

    public void OnEnterTrainersVeiw(TrainerControl trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleOver();
            trainer = null;
        }
        state = GameState.Free;
        battleSystem.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

    }
    
    public void SetCurrScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }



}

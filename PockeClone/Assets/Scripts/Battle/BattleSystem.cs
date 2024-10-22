using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;


public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    MoveScreenToForgot,
    AskSelection,
    BattleOver,
}
public enum BattleAction
{
    Move,Switch,Item,Run
}
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;

    [SerializeField] Image playerCharacter; 
    [SerializeField] Image enemyCharacter; 

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveMenu moveMenu;

    [SerializeField] GameObject pokeballSprite;
    Pokeball pokeballLogic;


    BattleState? state;
    BattleState? prevState;
    //нужно для управления
    int currentAction;
    int currentMove;
    int currentPoke;
    int escapeAttempts;
    int currentAsk;
    bool answer;

    MoveBase newMoveToForgotOld;

    [SerializeField] BattleDialog battleDialog;
    [SerializeField] LevelUpHub levelUpHub;
    [SerializeField] AskingBox askingBox;

    public event Action<bool> OnBattleOver;

    PokemonParty playerParty;
    PlayerMovement player;

    PokemonParty trainerParty;
    TrainerControl trainer;

    Pokemon wildPokemon;

    bool isTrainerBattle = false;

    private void Update()
    {
        Debug.Log($"{state} and{prevState}");
    }
    public void BattleStart(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.wildPokemon = wildPokemon;
        this.playerParty = playerParty;
        player = playerParty.GetComponent<PlayerMovement>();
        isTrainerBattle = false;

        battleDialog.MoveEnable(false);
        StartCoroutine(SetupBattle());

        pokeballLogic = pokeballSprite.GetComponent<Pokeball>();
    }

    public void TrainerBattleStart(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.trainerParty = trainerParty;
        this.playerParty = playerParty;
        battleDialog.MoveEnable(false);
        player = playerParty.GetComponent<PlayerMovement>();
        trainer = trainerParty.GetComponent<TrainerControl>();
        isTrainerBattle = true;
        StartCoroutine(SetupBattle());
    }
    //заполнение хабов и начало хода
    public IEnumerator SetupBattle()
    {
        if (!isTrainerBattle)
        {//wild pokemon
        playerUnit.Setup(playerParty.GetHelthyPokemon());
        enemyUnit.Setup(wildPokemon);
        
        battleDialog.SetMoveName(playerUnit.Pokemon.Moves);
        yield return battleDialog.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeard");
        }
        else
        {
            enemyUnit.Hub.gameObject.SetActive(false);
            playerUnit.Hub.gameObject.SetActive(false);

            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerCharacter.gameObject.SetActive(true);
            enemyCharacter.gameObject.SetActive(true);

            playerCharacter.sprite = player.Sprite;
            enemyCharacter.sprite = trainer.Sprite;
            yield return battleDialog.TypeDialog($"{trainer.Name} wants to battle");

            enemyCharacter.gameObject.SetActive(false);
            enemyUnit.Hub.gameObject.SetActive(true);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHelthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return battleDialog.TypeDialog($"{trainer.Name} send out {enemyPokemon.Base.Name}");
            battleDialog.SetMoveName(enemyUnit.Pokemon.Moves);

            playerCharacter.gameObject.SetActive(false);
            playerUnit.Hub.gameObject.SetActive(true);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon =  playerParty.GetHelthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return battleDialog.TypeDialog($"{player.Name} send out {playerPokemon.Base.Name}");
            battleDialog.SetMoveName(playerPokemon.Moves);
        }
        partyScreen.Init();
        moveMenu.Init();
        ActionSelection();
        escapeAttempts = 0;
    }
    //переход на экран пати
    public void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerParty.Pokemons);
    }
    //окончание битвы
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }
    //действие игрока
    public void ActionSelection()
    {
        state = BattleState.ActionSelection;
        battleDialog.ActionEnable(true);
        battleDialog.SetDialog("Chose an action");
    }

    //переключение выбора атаки
    public void MoveSelection()
    {
        state = BattleState.MoveSelection;
        battleDialog.MoveEnable(true);
        battleDialog.ActionEnable(false);
        battleDialog.TextBoxEnable(false);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if(playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //выбор первого ходока
            bool playerGoesFirst = true;
            
            if(enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority ==playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Base.Speed >= enemyUnit.Pokemon.Base.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;
            //первый ход
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.Hp > 0)
            {
                //второй ход
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }


        }
        else
        {//ход после выбора покемона
            switch (playerAction)
            {
                case BattleAction.Switch:
                    var selectedMember = playerParty.Pokemons[currentPoke];
                    state = BattleState.Busy;
                    playerParty.SwitchPokemonToFirstPlace(currentPoke);
                    StartCoroutine(SwitchPokemon(selectedMember));
                    break;
                case BattleAction.Item:
                    battleDialog.ActionEnable(false);
                    StartCoroutine(ThrowPokeball());
                    break;
                case BattleAction.Run:
                    yield return TryToEscape();
                    break;
            }
            //ход врага
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }
        if (state != BattleState.BattleOver)
            ActionSelection();
        
    }

    //атака одного игрока на другого
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool CanRun = sourceUnit.Pokemon.OnBeforeMove();
        if (!CanRun)
        {
            yield return ShowStatusChange(sourceUnit.Pokemon);
            yield return sourceUnit.Hub.UpdateHP();
            yield break;
        }
        yield return ShowStatusChange(sourceUnit.Pokemon);

        move.pp--;

        yield return battleDialog.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (MoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.AttackAnim();
            yield return new WaitForSeconds(1f);
            targetUnit.HitAnim();
            if (move.Base.Category == MoveCategory.Status && move.Base.Power == 0)//атаки с статусом и без урона
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon,move.Base.Target);
            
            else
            { //атаки на урон 
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hub.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.SecondaryEffects !=null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.Hp > 0)
            {
                foreach(var sec in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= sec.Chance)
                        yield return RunMoveEffect(sec, sourceUnit.Pokemon, targetUnit.Pokemon, sec.Target);
                }
            }

            if (targetUnit.Pokemon.Hp <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return battleDialog.TypeDialog($"{sourceUnit.Pokemon.Base.Name} attak missed");
        }
        
    }

    //статусы горение и отравление
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if(state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChange(sourceUnit.Pokemon);
        yield return sourceUnit.Hub.UpdateHP();
        if (sourceUnit.Pokemon.Hp <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
        }
    }
    //выполнение атаки-статуса
    IEnumerator RunMoveEffect(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget effectsTarget)
    {
        if (effects.Boosts != null)
        {
            if (effectsTarget == MoveTarget.Self)
                source.ApplyBoost(effects.Boosts);

            else
                target.ApplyBoost(effects.Boosts);

        }
        if(effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        
        if(effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChange(source);
        yield return ShowStatusChange(target);
    }
    bool MoveHits(Move move, Pokemon source, Pokemon target)
    {

        if (move.Base.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoost[Stat.Accuracy];
        int evasion = source.StatBoost[Stat.Evasion];

        var boostValue = new float[] { 1f, 4f/3f,5f/3f, 2f, 7f/3f, 8f/3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValue[accuracy];
        else
            moveAccuracy/= boostValue[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValue[evasion];
        else
            moveAccuracy *= boostValue[-evasion];

        return UnityEngine.Random.Range(1, 101)<= moveAccuracy;
    }
    //строчки о изменении в статах в битве
    IEnumerator ShowStatusChange(Pokemon pokemon)
    {
        while (pokemon.StatusChange.Count > 0)
        {
            var message = pokemon.StatusChange.Dequeue();
            yield return battleDialog.TypeDialog(message);

        }
    }

    //проверка окончания битвы при выбывании одного из покемонов
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {

            var nextPokemon = playerParty.GetHelthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else BattleOver(false);
        }
        else
        {
            if(!isTrainerBattle)
            BattleOver(true);
            else
            {
                var nextPoke = trainerParty.GetHelthyPokemon();
                if (nextPoke != null)
                {
                    StartCoroutine(SendNextPokemon(nextPoke));
                }
                else BattleOver(true);
            }
        }
    }

    //демонстрация деталей атаки
    IEnumerator ShowDamageDetails(DamageDetail damageDetail)
    {
        if(damageDetail.Critical > 1f)
            yield return battleDialog.TypeDialog("A critical hit!");
        
        if(damageDetail.Type > 1f)
            yield return battleDialog.TypeDialog("Its supper effective");
        else if(damageDetail.Type < 1f)
            yield return battleDialog.TypeDialog("Its not very effective");
    }
    //смена стадии битвы
    public void HundleUpdate()
    {
        switch (state)
        {
            case BattleState.ActionSelection:
                HandleActionSelection();
                break;
            case BattleState.MoveSelection:
                HandleMoveSelection();
                break;
            case BattleState.PartyScreen:
                HandleVerticalListSelection();
                break;
            case BattleState.MoveScreenToForgot:
                HandleVerticalListSelection();
                break;
            case BattleState.AskSelection:
                HandleAskingSelection();
                break;

        }

    }

    //смена инфы о покемоне
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        Debug.Log("RR");
        if (playerUnit.Pokemon.Hp > 0) 
        { 
            yield return battleDialog.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.FaintAnim();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.Setup(newPokemon);

        battleDialog.SetMoveName(newPokemon.Moves);

        yield return battleDialog.TypeDialog($"Go {playerUnit.Pokemon.Base.Name}");

        if (prevState == null)
            state = BattleState.RunningTurn;
        else
        {
            state = prevState;
        }
    }        

    //выборы стрелочками

        //выбор стрелочками атаки
        void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMove++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMove--;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
        battleDialog.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerUnit.Pokemon.Moves[currentMove].pp == 0)
                return;

            battleDialog.MoveEnable(false);
            battleDialog.ActionEnable(false);
            battleDialog.TextBoxEnable(true);
            StartCoroutine(RunTurns(BattleAction.Move));
            currentMove = 0;

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleDialog.MoveEnable(false);
            battleDialog.ActionEnable(false);
            battleDialog.TextBoxEnable(true);
            ActionSelection();
            currentMove = 0;
        }

    }
        //выбор стрелочками покемона в пати
        void HandleVerticalListSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            currentPoke++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            currentPoke--;


        switch (state)
        {
            case BattleState.PartyScreen:
                partyScreen.UpdatePartySelection(currentPoke);

                if (Input.GetKeyDown(KeyCode.Z))
                    PartyScreenOptionZ(playerParty.Pokemons[currentPoke]);
                else if (Input.GetKeyDown(KeyCode.X))
                {

                    partyScreen.gameObject.SetActive(false);
                    ActionSelection();
                    currentPoke = 0;
                }
                break;
            case BattleState.MoveScreenToForgot:
                currentPoke = Mathf.Clamp(currentPoke, 0, 4);
                moveMenu.UpdateMoveSelection(currentPoke);
                if (Input.GetKeyDown(KeyCode.Z))
                    
                    StartCoroutine( MoveScreenOptionZ(currentPoke));
                    break;

        }

    }

    IEnumerator MoveScreenOptionZ(int currentMove)
    {
        if (currentMove <= 3)
        {
            //forget move + link new move to list
            var oldMove = playerUnit.Pokemon.Moves[currentMove];
            playerUnit.Pokemon.Moves[currentMove] = new Move(newMoveToForgotOld);
            moveMenu.gameObject.SetActive(false);
            yield return battleDialog.TypeDialog("1, 2, and... ... ... Poof!");
            yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {oldMove.Base.Name}.");
            yield return battleDialog.TypeDialog($"And...");
            yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {playerUnit.Pokemon.Moves[currentMove].Base.Name}!");
            state = BattleState.RunningTurn;

        }
        else if(currentMove == 4)
        {
            //forget this one move
            moveMenu.gameObject.SetActive(false);
            yield return battleDialog.TypeDialog($"Stop learning {newMoveToForgotOld.Name}?");
            OpenAskingBox();
            yield return new WaitUntil(() => state != BattleState.AskSelection);
            if (answer)
            {
                yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {newMoveToForgotOld.Name}");
                state = BattleState.RunningTurn;
            } yield return TryingLearnNewMove(newMoveToForgotOld);

        }
    }
    void PartyScreenOptionZ(Pokemon selectedMember)
    {

        if (selectedMember.Hp <= 0)
        {
            partyScreen.MessageText("U cant send out fainted pokemon");
            return;
        }
        if (selectedMember == playerUnit.Pokemon)
        {
            partyScreen.MessageText("U cant switch with the same pokemon");
            return;
        }
        partyScreen.gameObject.SetActive(false);

        if (prevState == BattleState.ActionSelection)
        {
            prevState = null;
            StartCoroutine(RunTurns(BattleAction.Switch));
        }
        else if (prevState == BattleState.Busy)
        {
            state = BattleState.Busy;
            prevState = null;
            playerParty.SwitchPokemonToFirstPlace(currentPoke);
            StartCoroutine(SwitchPokemon(selectedMember));
            new WaitUntil(() => state == BattleState.RunningTurn );
            state = BattleState.ActionSelection;
        }
        else
        {
            state = BattleState.Busy;
            playerParty.SwitchPokemonToFirstPlace(currentPoke);
            StartCoroutine(SwitchPokemon(selectedMember));
            new WaitUntil(() => state == BattleState.RunningTurn);
            state = BattleState.BattleOver;
        }
    }
    
    IEnumerator PressZForNextStep()
    {
        while (!(Input.GetKeyDown(KeyCode.Z)))
        {
            yield return new WaitForSeconds(0.01f);
        }
    }
        //выбор стрелочками действия
        void HandleActionSelection()
    {
        //смена действий
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentAction;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentAction;

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        battleDialog.UpdateActionSelection(currentAction);

        //выполнение действий
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentAction)
            {
                case 0:
                    MoveSelection();
                    break;
                case 1:
                    //pokemon
                    prevState = state;
                    OpenPartyScreen();
                    break;
                case 2:
                    StartCoroutine(RunTurns(BattleAction.Item));
                    break;
                case 3:
                    StartCoroutine(RunTurns(BattleAction.Run));

                    break;
            }
            currentAction = 0;
        }
    }

    void HandleAskingSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentAsk;

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentAsk;

        currentAsk = Mathf.Clamp(currentAsk, 0, 1);

        askingBox.UpdateAskBoxArrow(currentAsk);

        //выполнение действий
        if (Input.GetKeyDown(KeyCode.Z))
        {
            answer = askingBox.Ask(currentAsk);
            
            currentAsk = 0;
            state = prevState;
            prevState = null;
            askingBox.gameObject.SetActive(false);
        }
    }
        
    IEnumerator SendNextPokemon(Pokemon nextPokemon)
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"{trainer.Name} is about to use {nextPokemon.Base.Name}. Do u wanna switch pokemon?");
        OpenAskingBox();
        yield return new WaitUntil(() => state != BattleState.AskSelection);
        if (answer)
        {
            prevState = state;
            OpenPartyScreen();
            yield return new WaitUntil(() => state == BattleState.ActionSelection);
            Debug.Log("now");
        }
        enemyUnit.Setup(nextPokemon);
        yield return battleDialog.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name}");
    }

    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;
        if (isTrainerBattle) { 
            yield return battleDialog.TypeDialog($"You cant steal trainers pokemon!");
            state = BattleState.RunningTurn;
            yield break;

        }

        yield return battleDialog.TypeDialog($"{player.Name} used pokeball!");

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        //Animation
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return pokeballLogic.AnimationOpen();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return new WaitForSeconds(1);
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.50f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y + 1f, 0.45f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.45f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y + 0.4f, 0.4f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.4f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 0.5f, 0.3f).WaitForCompletion();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.3f).WaitForCompletion();

        int shakeCount = TryToCatchPoke(enemyUnit.Pokemon);

        for (int i = 0; i<Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            yield return battleDialog.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught");
            yield return pokeball.DOColor(new Color32(90, 90, 90, 255), 0.5f).WaitForCompletion();
            yield return pokeball.DOColor(new Color32(255, 255, 255, 255), 0.5f).WaitForCompletion();
            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return battleDialog.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was added to your party!");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {

            if(shakeCount <2)
                yield return battleDialog.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free");
            else
                yield return battleDialog.TypeDialog($"Almost caught it");
            yield return new WaitForSeconds(1f);
            yield return enemyUnit.PlayBreakOutAnimation();
            yield return pokeball.DOFade(0, 0.2f);
            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }

    }

    int TryToCatchPoke(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHP - 2 * pokemon.Hp) * pokemon.Base.CatchRate * ConditionDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHP);

        if (a>= 255)
            return 4;
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            ++shakeCount;
        }
        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return battleDialog.TypeDialog("You cant run from trainer battle");
            state = BattleState.RunningTurn;
            yield break;
        }

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;
        ++ escapeAttempts;
        if (enemySpeed < playerSpeed)
        {
            yield return battleDialog.TypeDialog("Run escape safely");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return battleDialog.TypeDialog("Run escape safely");
                BattleOver(true);

            }
            else
            {
                yield return battleDialog.TypeDialog("Cant escape");
                state = BattleState.RunningTurn;
            }
        }
    }
    //смерть покемона
    IEnumerator HandlePokemonFainted(BattleUnit fainted)
    {
        yield return battleDialog.TypeDialog($"{fainted.Pokemon.Base.Name} fainted");
        yield return fainted.Hub.UpdateHP();
        fainted.FaintAnim();
        yield return new WaitForSeconds(2f);

        if (!fainted.IsPlayer)
        {
            //exp gain
            int expYield = fainted.Pokemon.Base.ExpYield;
            int enemyLevel = fainted.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hub.SetExpSmooth();

            //check level
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hub.SetLevel();
                yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to {playerUnit.Pokemon.Level} level");

                //level up hub
                battleDialog.ActionEnable(false);
                levelUpHub.gameObject.SetActive(true);
                levelUpHub.StatsUp(playerUnit.Pokemon);
                yield return PressZForNextStep();
                levelUpHub.NewStats(playerUnit.Pokemon);
                yield return new WaitForSeconds(0.5f);
                yield return PressZForNextStep();
                //try to learn a new move

                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrLevel();
                levelUpHub.gameObject.SetActive(false);
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < 4)
                    {
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}");
                        battleDialog.SetMoveName(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        newMoveToForgotOld = newMove.Base;
                        yield return TryingLearnNewMove(newMove.Base);
                    }
                    newMoveToForgotOld = null;
                }
                yield return playerUnit.Hub.SetExpSmooth(true);

            }
            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(fainted);
    }

    void OpenAskingBox()
    {
        prevState = state;
        askingBox.gameObject.SetActive(true);
        state = BattleState.AskSelection;
    }

    void OpenMoveScreen(MoveBase newMove = null)
    {
        moveMenu.gameObject.SetActive(true);
        moveMenu.SetData(playerUnit.Pokemon, newMove);
        state = BattleState.MoveScreenToForgot;

    }

    IEnumerator TryingLearnNewMove(MoveBase newMove)
    {
        yield return battleDialog.TypeDialog($"{playerUnit.Pokemon.Base.Name} is trying to learn {newMove.Name}");
        yield return battleDialog.TypeDialog($"But {playerUnit.Pokemon.Base.Name} cant learn more then four moves");
        yield return battleDialog.TypeDialog($"Delete a move to make room for {newMove.Name}?");
        OpenAskingBox();
        yield return new WaitUntil(() => state != BattleState.AskSelection);
        if (answer)
        {
            OpenMoveScreen(newMove);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }
}


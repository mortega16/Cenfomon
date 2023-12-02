using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, Bag, PartyScreen, MoveToForget, BattleOver}
public enum BattleAction { Move, SwitchCenfomon, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject cristalSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    public event Action<bool> OnBattleOver;

    BattleState state;

    int currentAction;
    int currentMove;

    CenfomonParty playerParty;
    CenfomonParty trainerParty;
    Cenfomon wildCenfomon;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    public void StartBattle(CenfomonParty playerParty, Cenfomon wildCenfomon)
    {
        this.playerParty = playerParty;
        this.wildCenfomon = wildCenfomon;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(CenfomonParty playerParty, CenfomonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            //Batalla Cenfomon salvaje
            playerUnit.SetUp(playerParty.GetHealthyCenfomon());
            enemyUnit.SetUp(wildCenfomon);

            dialogBox.SetMoveNames(playerUnit.Cenfomon.Moves);

            yield return dialogBox.TypeDialog($"¡Un {enemyUnit.Cenfomon.Base.Name} salvaje ha aparecido!");

        }
        else
        {
            //Batalla entrenador

            //Mostrar sprites de Jugador y Entrenador
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"¡{trainer.Name} te ha desafiado a una batalla!");

            //Enviar el primer Cenfomon del entrenador
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyCenfomon = trainerParty.GetHealthyCenfomon();
            enemyUnit.SetUp(enemyCenfomon);
            yield return dialogBox.TypeDialog($"{trainer.Name} ha escogido a {enemyCenfomon.Base.Name}");

            //Enviar el primer Cenfomon del jugador
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerCenfomon = playerParty.GetHealthyCenfomon();
            playerUnit.SetUp(playerCenfomon);
            yield return dialogBox.TypeDialog($"¡Ve, {playerCenfomon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Cenfomon.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("¿Qué deberíamos hacer?");
        dialogBox.EnableActionSelector(true);
    }

    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator ChooseMoveToForget(Cenfomon cenfomon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Escoge el movimiento que quieres olvidar");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(cenfomon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Cenfomon.CurrentMove = playerUnit.Cenfomon.Moves[currentMove];
            enemyUnit.Cenfomon.CurrentMove = enemyUnit.Cenfomon.GetRandomMove();

            //Verificar quién va primero
            bool playerGoesFirst = playerUnit.Cenfomon.Speed >= enemyUnit.Cenfomon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondCenfomon = secondUnit.Cenfomon;

            //Primer Turno
            yield return RunMove(firstUnit, secondUnit, firstUnit.Cenfomon.CurrentMove);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
            if (state == BattleState.BattleOver) yield break;

            if (secondCenfomon.HP > 0)
            {
                //Segundo Turno
                yield return RunMove(secondUnit, firstUnit, secondUnit.Cenfomon.CurrentMove);
                yield return new WaitUntil(() => state == BattleState.RunningTurn);
                if (state == BattleState.BattleOver) yield break;
            }

        }
        else
        {
            if (playerAction == BattleAction.SwitchCenfomon)
            {
                var selectedCenfomon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchCenfomon(selectedCenfomon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                //Esto se está gestionando desde itemScreen, entonces no se hace nada y se sigue al movimiento enemigo
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            //Turno del enemigo

            var enemyMove = enemyUnit.Cenfomon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.RunningTurn;

        var move = playerUnit.Cenfomon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        //Si el state de la batalla no fue cambiado por RunMove, ir al siguiente paso
        if (state == BattleState.RunningTurn)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.RunningTurn;

        var move = enemyUnit.Cenfomon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        //Si el state de la batalla no fue cambiado por RunMove, ir al siguiente paso
        if (state == BattleState.RunningTurn)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Cenfomon.Base.Name} usó {move.Base.Name}");


        var damageDetails = targetUnit.Cenfomon.TakeDamage(move, sourceUnit.Cenfomon);
        yield return targetUnit.Hud.WaitforHPUpdate();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return HandleCenfomonFainted(targetUnit);
        }

    }

    IEnumerator HandleCenfomonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"¡{faintedUnit.Cenfomon.Base.Name} no puede seguir en batalla!");
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            //Ganar exp
            int expYield = faintedUnit.Cenfomon.Base.ExpYield;
            int enemyLevel = faintedUnit.Cenfomon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Cenfomon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Cenfomon.Base.Name} ha ganado {expGain} puntos de experiencia");
            yield return playerUnit.Hud.SetExpSmooth();

            //Revisar subir de nivel
            while (playerUnit.Cenfomon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Cenfomon.Base.Name} subió a nivel {playerUnit.Cenfomon.Level}");

                //Tratar de aprender un nuevo movimiento

                var newMove = playerUnit.Cenfomon.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Cenfomon.Moves.Count < CenfomonBase.MaxNumOfMoves)
                    {
                        playerUnit.Cenfomon.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"¡{playerUnit.Cenfomon.Base.Name} aprendió {newMove.Base.Name}!");
                        dialogBox.SetMoveNames(playerUnit.Cenfomon.Moves);
                    }
                    else
                    {
                        //Opción de olvidar un movimiento
                        yield return dialogBox.TypeDialog($"¡{playerUnit.Cenfomon.Base.Name} puede aprender {newMove.Base.Name}!");
                        yield return dialogBox.TypeDialog($"Pero no puede aprender más de {CenfomonBase.MaxNumOfMoves} movimientos");
                        yield return ChooseMoveToForget(playerUnit.Cenfomon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }


            yield return new WaitForSeconds(1f);
        }
        CheckForBattleOver(faintedUnit);

    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextCenfomon = playerParty.GetHealthyCenfomon();
            if (nextCenfomon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextCenfomon = trainerParty.GetHealthyCenfomon();
                if (nextCenfomon != null)
                    StartCoroutine(SendNextTrainerCenfomon());
                else
                    BattleOver(true);
            }
        }

    }

    IEnumerator ShowDamageDetails(Cenfomon.DamageDetails damageDetails)
    {
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("¡Es muy efectivo!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("No es muy efectivo...");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == CenfomonBase.MaxNumOfMoves)
                {
                    //No aprender el nuevo movimiento
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Cenfomon.Base.Name} no aprendió {moveToLearn.Name}"));
                }
                else
                {
                    //Olvidar el seleccionado y aprender el nuevo movimiento
                    var selectedMove = playerUnit.Cenfomon.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Cenfomon.Base.Name} olvidó {selectedMove.Name} y aprendió {moveToLearn.Name}"));
                    playerUnit.Cenfomon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Math.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Bag
                OpenBag();
            }
            else if (currentAction == 2)
            {
                //Cenfomon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 3;

        currentMove = Math.Clamp(currentMove, 0, playerUnit.Cenfomon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Cenfomon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Cenfomon.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"No tiene las fuerzas para combatir");
                return;
            }
            if (selectedMember == playerUnit.Cenfomon)
            {
                partyScreen.SetMessageText($"{playerUnit.Cenfomon.Base.Name} ya está en batalla");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchCenfomon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchCenfomon(selectedMember));
            }

            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Cenfomon.HP <= 0)
            {
                partyScreen.SetMessageText("¡Debes escoger otro Cenfomon!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        };

        partyScreen.HandleUpdate(onSelected, onBack);
    }

    IEnumerator SwitchCenfomon(Cenfomon newCenfomon)
    {
        if (playerUnit.Cenfomon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"¡{playerUnit.Cenfomon.Base.Name}, vuelve!");
            yield return new WaitForSeconds(2f);
        }

        playerUnit.SetUp(newCenfomon);
        dialogBox.SetMoveNames(newCenfomon.Moves);
        yield return dialogBox.TypeDialog($"¡Ve, {newCenfomon.Base.Name}!");

        //StartCoroutine(EnemyMove());
        state = BattleState.RunningTurn;
    }

    IEnumerator SendNextTrainerCenfomon()
    {
        state = BattleState.Busy;

        var nextCenfomon = trainerParty.GetHealthyCenfomon();
        enemyUnit.SetUp(nextCenfomon);
        yield return dialogBox.TypeDialog($"{trainer.Name} escogió a {nextCenfomon.Base.Name}");

        state = BattleState.RunningTurn;
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is CrystalItem)
        {
            yield return ThrowCrystal((CrystalItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    IEnumerator ThrowCrystal(CrystalItem crystalItem)
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"No se pueden capturar Cenfomones ajenos");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"¡{player.Name} ha usado un {crystalItem.Name}!");

        var crystalObj = Instantiate(cristalSprite, playerUnit.transform.position - new Vector3(2,0), Quaternion.identity);
        var crystal = crystalObj.GetComponent<SpriteRenderer>();
        crystal.sprite = crystalItem.Icon;

        //Animations
        yield return crystal.transform.DOJump(enemyUnit.transform.position, 2f, 1, 1f).WaitForCompletion();
        //yield return enemyUnit.PlayCaptureAnimation();
        yield return crystal.transform.DOMoveY(enemyUnit.transform.position.y -1,0.5f).WaitForCompletion();

        int shakeCount = TryToCatchCenfomon(enemyUnit.Cenfomon, crystalItem);

        for(int i = 0; i < Mathf.Min(shakeCount,3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return crystal.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if(shakeCount == 4)
        {
            //Cenfomon atrapado
            yield return dialogBox.TypeDialog($"{enemyUnit.Cenfomon.Base.Name} ha fusionado su alma con el cristal");
            yield return crystal.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddCenfomon(enemyUnit.Cenfomon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Cenfomon.Base.Name} se ha unido a tu equipo");

            Destroy(crystal);
            BattleOver(true);
        }
        else
        {
            //Cenfomon escapa
            yield return new WaitForSeconds(1f);
            crystal.DOFade(0, 2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            yield return dialogBox.TypeDialog($"{enemyUnit.Cenfomon.Base.Name} escapó");

            Destroy(crystal);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchCenfomon(Cenfomon cenfomon, CrystalItem crystalItem)
    {
        float a = (3 * cenfomon.MaxHp - 2 * cenfomon.HP) * cenfomon.Base.CatchRate * crystalItem.CatchRateModifier / (3 * cenfomon.MaxHp);

        if (a >= 255)
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
            yield return dialogBox.TypeDialog($"No puedes huir de batallas con entrenadores");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Cenfomon.Speed;
        int enemySpeed = enemyUnit.Cenfomon.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Has huido exitosamente");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Has huido exitosamente");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"El intento de escape no tuvo éxito");
                state = BattleState.RunningTurn;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int letterBySecond;

[SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject textBox;

    [SerializeField] List<GameObject> actionArrow;
    [SerializeField] List<GameObject> moveArrow;
    [SerializeField] List<Text> moveText;


    [SerializeField] Text TypeText;
    [SerializeField] Text PPText;
    [SerializeField] TextShadow PPTextScript;
    [SerializeField] Text MaxPPText;

    //быстрый вывод диалога
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //медлееный вывод диалога
    public IEnumerator TypeDialog(string dialog)
    {
        int lbsStandart = letterBySecond;
        int once = 0;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(1f/letterBySecond);
            if (Input.GetKeyDown(KeyCode.Z)&&once ==0)
            {
                letterBySecond = letterBySecond * 2;
                once++;
            }
        }
        letterBySecond = lbsStandart;
        yield return new WaitForSeconds(1f);
    }

    //включение диалогового окна
    public void MoveEnable(bool enable)
    {
        moveSelector.SetActive(enable);
    }

    //включение окна выбора атаки
    public void ActionEnable(bool enable )
    {
        actionSelector.SetActive(enable);
    }

    //включение окошка выбора
    public void TextBoxEnable(bool enable)
    {
        textBox.SetActive(enable);
    }
    //смена выбора действия
    public void UpdateActionSelection(int selectAction) 
    {
        for(int i = 0; i <actionArrow.Count;++i)
        {
            if (i == selectAction)
            {
                actionArrow[i].SetActive(true);
            }
            else
                actionArrow[i].SetActive(false);
        }
    }

    public void UpdateMoveSelection(int selectMove, Move move)
    {
        for (int i = 0; i < moveArrow.Count; ++i)
        {
            if (i == selectMove)
            {
                moveArrow[i].SetActive(true);
            }
            else
                moveArrow[i].SetActive(false);
        }
        PPText.text = $"{move.pp}";
        MaxPPText.text = $"{move.Base.Pp}";
        TypeText.text = move.Base.Type.ToString();

        if (move.pp == 0)
            PPTextScript.ZeroPP(true);
        else PPTextScript.ZeroPP(false);
    
    }

    //вывод имени атаки
    public void SetMoveName(List<Move> moves)
    {
        for (int i = 0;i < moveText.Count;++i)
        {
            if (i < moves.Count)
            {
                moveText[i].text = moves[i].Base.Name;
            }
            else moveText[i].text = "-";
        }
    }

}

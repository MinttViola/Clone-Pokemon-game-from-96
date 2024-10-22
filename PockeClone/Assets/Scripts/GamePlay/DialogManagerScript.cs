using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManagerScript : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] float letterBySecond;
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public static DialogManagerScript Instance { get; private set; }
    Dialog dialog;
    int currentLine = 0;
    bool isTyping = false;
    Action OnFinishDialog;

    public bool IsShowing { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, Action OnFinish = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        IsShowing = true;
        this.dialog = dialog;
        OnFinishDialog = OnFinish;
        dialogBox.SetActive(true);
        yield return TypeDialog(dialog.Lines[0]);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z)&& !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            } else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                OnFinishDialog?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(1f / letterBySecond);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SetDialog(dialog);
            }
        }
        isTyping = false;
    }
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }
}

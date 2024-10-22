using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerControl : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterLose;
    [SerializeField] GameObject react;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    Character character;
    bool battleLost = false;
    public string Name { get { return name; } }

    public Sprite Sprite { get { return sprite; } }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Anim.Direction);
    }
    public IEnumerator TriggerTrainerBattle(PlayerMovement player)
    {
        //восклицательный знак
        react.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        react.SetActive(false);

        //подошел к игроку
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.Move(moveVec);

        StartCoroutine(DialogManagerScript.Instance.ShowDialog(dialog, () => { GameController.Instance.StartTrainerBattle(this); }));
    }

    public void BattleOver()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public void SetFovRotation(FaceDirection dir)
    {
        float angle = 0f;

        switch (dir)
        {
            case FaceDirection.Left:
                angle = 270f;
                break;
            case FaceDirection.Right:
                angle = 90f;
                break;
            case FaceDirection.Up:
                angle = 180f;
                break;
        }
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
        {
            fov.gameObject.SetActive(false);
        }
    }

    public void Interact(Transform interact)
    {
        character.LookTowards(interact.position);
        if (!battleLost)
        {
            StartCoroutine(DialogManagerScript.Instance.ShowDialog(dialog, () => { GameController.Instance.StartTrainerBattle(this); }));
        }
        else
        {
            StartCoroutine(DialogManagerScript.Instance.ShowDialog(dialogAfterLose));
        }
    }
}

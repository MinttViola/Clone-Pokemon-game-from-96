using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHub : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] Text namePoke;
    [SerializeField] Text level;
    [SerializeField] Text HP;
    [SerializeField] Text MaxHP;
    [SerializeField] HPBar HPBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Image status;
    [SerializeField] List<Sprite> status_effects;
    Pokemon _pokemon;

    //заполнение хаба игрока
    public void SetData(Pokemon pokemon)
    {

        _pokemon = pokemon;
        namePoke.text = pokemon.Base.Name;
        SetLevel();
        if (isPlayer)
        {
            HP.text = pokemon.Hp + "";

            MaxHP.text = "" + pokemon.MaxHP;
        }

        HPBar.SetHp((float)pokemon.Hp/pokemon.MaxHP, pokemon.MaxHP);
        HPBar.ChangeColor(pokemon.Hp);
        SetExp();
        SetStatus();
        _pokemon.OnStatusChanged += SetStatus;
    }

    public void SetStatus()
    {
        switch (_pokemon.Status?.Name)
        {
            case "Burn":
                status.enabled = true;
                status.sprite = status_effects[0];
                break;
            case "Freeze":
                status.enabled = true;
                status.sprite = status_effects[1];
                break;

            case "Paralyzed":
                status.enabled = true;
                status.sprite = status_effects[2];
                break;

            case "Poison":
                status.enabled = true;
                status.sprite = status_effects[3];
                break;

            case "Sleep":
                status.enabled = true;
                status.sprite = status_effects[4];
                break;
            default:
                status.enabled = false;
                break;
        }
    }

    public void SetLevel()
    {
        level.text = _pokemon.Level + "";
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset) expBar.transform.localScale = new Vector3(0, 1, 1);
        float normalizedExp = GetNormalizedExp();

        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level+1);

        float normalizedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public IEnumerator UpdateHP()
    {
         
        if (isPlayer)
        HP.text = _pokemon.Hp + "";

        yield return HPBar.SetHPSmooth((float)_pokemon.Hp / _pokemon.MaxHP, _pokemon.Hp);
            _pokemon.HpChange = false;
        
    }
}

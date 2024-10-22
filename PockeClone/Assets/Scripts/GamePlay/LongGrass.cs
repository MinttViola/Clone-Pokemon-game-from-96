using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerMovement player)
    { 
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                GameController.Instance.StartBattle();
            }
    }
}

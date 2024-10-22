using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    
    [SerializeField] DestinationIdentifaer destinationPortal;
    [SerializeField] Transform spawnPoint;
    PlayerMovement player;
    Fade fader;
    public void OnPlayerTriggered(PlayerMovement player)
    {
        this.player = player;
        StartCoroutine(Teleport());
    }

    public void Start()
    {
        fader = FindObjectOfType<Fade>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PausedGame(true);
        yield return fader.FadeAnim(0.2f, true);

        var desPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapTile(desPortal.SpawnPoint.position);
        yield return fader.FadeAnim(0.2f, false);
        GameController.Instance.PausedGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}


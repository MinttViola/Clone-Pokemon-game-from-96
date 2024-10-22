using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifaer destinationPortal;
    [SerializeField] Transform spawnPoint;
    PlayerMovement player;
    Fade fader;
    public void OnPlayerTriggered(PlayerMovement player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    public void Start()
    {
        fader = FindObjectOfType<Fade>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PausedGame(true);
        yield return fader.FadeAnim(0.2f, true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var desPortal = FindObjectsOfType<Portal>().First(x => x != this&& x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapTile(desPortal.SpawnPoint.position);
        yield return fader.FadeAnim(0.2f, false);
        GameController.Instance.PausedGame(false);
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifaer { A,B,C,D,E,F,G,H}

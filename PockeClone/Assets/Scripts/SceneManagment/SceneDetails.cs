using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScene;

    public bool IsLoad { get; private set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            LoadScene();
            GameController.Instance.SetCurrScene(this);
            //load scene
            foreach(var scene in connectedScene)
            {
                scene.LoadScene();
            }
            //unload scene
            if(GameController.Instance.PrevScene != null)
            {
                var prevLoadedScene = GameController.Instance.PrevScene.connectedScene;
                foreach(var scene in prevLoadedScene)
                {
                    if(!connectedScene.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }

        }
    }

    public void LoadScene()
    {
        if (!IsLoad)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoad = true;
        }
    }
    
    public void UnloadScene()
    {
        if (IsLoad)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoad = false;
        }
    }
    
}

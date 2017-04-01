using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController singleton;

    public int currentScene;

    void Awake()
    {
        if (singleton)
            Destroy(gameObject);

        currentScene = SceneManager.GetActiveScene().buildIndex;
        singleton = this;
    }

	// Use this for initialization
	void Start ()
    {

        Debug.Log(currentScene);
        HUDScript.singleton.SetDefaults();
    }

    public void LoadScene(int sceneToLoad)
    {
        if(sceneToLoad > 2)
        {
            sceneToLoad = 0;
        }

        currentScene = sceneToLoad;
        Application.LoadLevel(sceneToLoad);
    }
}

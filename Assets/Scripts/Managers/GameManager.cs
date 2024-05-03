using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    private void Awake ()
    {
        if  (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }else{
            Destroy(gameObject);
        }

        StartCoroutine(GameSetup());
    }

    public GameManager GetInstance () { return Instance; }

    private IEnumerator GameSetup()
    {
        // Debug.Log("Setting Up Player Manager...");
        PlayerManager
            .GetInstance()
            .SetUp();
        // Debug.Log("Player Manager ready.");
        yield return null;
        
        yield return null;
        // Debug.Log("Setting Up Camera Manager...");
        CameraManager
            .GetInstance()
            .SetUp();
        // Debug.Log("Camera Manager ready.");
        yield return null;
        
        // Debug.Log("Setting Up Input Manager...");
        InputManager
            .GetInstance()
            .SetUp();
        // Debug.Log("Input Manager ready.");
        yield return null;
        
        // Debug.Log("Setting Up UI...");
        UI_Manager
            .GetInstance()
            .SetUp();
        // Debug.Log("UI ready.");
        yield return null;
    }
}

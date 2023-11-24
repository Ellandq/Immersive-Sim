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
    }

    public GameManager GetInstance () { return Instance; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public BoardManager boardScript;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
        }
        else if (GameManager.Instance != this)
        {
            Destroy(gameObject);
        }

        //que se marque para que nunca se destruya
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
    }

    private void Start()
    {
        InitGame();
    }
    private void InitGame()
    {
        boardScript.SetupScene(3);
    }
}

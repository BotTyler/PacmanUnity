using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum Direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, NONE = 4, ON = 5 }
    public enum GameState { SCATTER, CHASE, FRIGHTEN, START }
    private static GameState CurrentGameState = GameManager.GameState.SCATTER;

    public static readonly int powerUpTime = 10; // seconds

    private static object scoreLock = new object();
    private static object poweredUpLock = new object();
    public static GameManager instance;

    private static int score;

    private static int poweredUp = 0;
    public static int totSteps = 4;
    public static float animationTIme = .1f;

    [SerializeField] private int temp;
    [SerializeField] private bool poweredUpNow = false;
    [SerializeField] private int p;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;

    }

    private void Update()
    {
        this.temp = GameManager.score;
        poweredUpNow = isPoweredUp();
        // put in the condiditions to handle the game states.
    }

    public static void resetScore()
    {
        lock (GameManager.scoreLock)
        {
            GameManager.score = 0;

        }
    }

    /// <summary>
    /// Method that will add to the score.
    /// </summary>
    /// <param name="score">Score you want to add.</param>
    public static void addScore(int score)
    {
        lock (GameManager.scoreLock)
        {
            GameManager.score += score;

        }
    }
    /// <summary>
    /// Method that will subtract from the score counter.
    /// </summary>
    /// <param name="subScore">Score you want to subtract</param>
    public static void subtractScore(int subScore)
    {
        lock (GameManager.scoreLock)
        {
            GameManager.score -= subScore;

        }
    }

    /// <summary>
    /// Method that adds one to the power up variable.
    /// When a power up ball is colelcted we should add one to this variable.
    /// </summary>
    public static void powerUp()
    {
        lock (GameManager.poweredUpLock)
        {
            GameManager.poweredUp++;

        }
    }

    public static void powerDown()
    {
        lock (GameManager.poweredUpLock)
        {
            GameManager.poweredUp--;
        }
    }

    /// <summary>
    /// Method to check if the game is in the powered up state.
    /// We are in a powered up state if the poweredUp variable has a value greater than 0.
    /// </summary>
    /// <returns>bool if the game is in a powered up state or not.</returns>
    public static bool isPoweredUp()
    {
        int power = 0;
        lock (GameManager.poweredUpLock)
        {
            power = GameManager.poweredUp;
            //GameManager.CurrentGameState = GameManager.GameState.FRIGHTEN;
        }
        return power > 0;
    }

    public static GameManager.GameState GetGameState()
    {
        return GameManager.CurrentGameState;
    }


}

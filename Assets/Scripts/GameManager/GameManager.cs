using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum Direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, NONE = 4, ON = 5 }
    public enum GameState { SCATTER, CHASE, FRIGHTEN, STARTFRIGHTEN, START, EATEN, LEAVEGATE }
    public enum pacManEnum { RED, BLUE, PINK, TAN }
    private static GameState[] CurrentGameState = { GameState.START, GameState.START, GameState.START, GameState.START };
    public GameState[] gs = GameManager.CurrentGameState;

    public static readonly int powerUpTime = 10; // seconds

    private static object scoreLock = new object();
    private static object poweredUpLock = new object();
    public static GameManager instance;

    private static int score;

    private static int poweredUp = 0;
    public static int totSteps = 4;
    public static float animationTIme = .2f;

    [SerializeField] private int temp;
    [SerializeField] private bool poweredUpNow = false;
    [SerializeField] private int p;

    [SerializeField] private int[] scatter;
    [SerializeField] private int[] chase;

    private static bool gameEnd = false;
    private static bool startGameStates = false;
    private static bool hasGameStarted = false;

    [SerializeField] public bool startGames = startGameStates;
    [SerializeField] public bool gamestart = hasGameStarted;


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

    private static void setEveryonesState(GameManager.GameState x)
    {
        for (int counter = 0; counter < GameManager.CurrentGameState.Length; counter++)
        {
            GameManager.CurrentGameState[counter] = x;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        GameManager.setEveryonesState(GameManager.GameState.LEAVEGATE);

    }

    private void Update()
    {
        this.temp = GameManager.score;
        poweredUpNow = GameManager.isPoweredUp();
        this.gs = GameManager.CurrentGameState;
        startGames = startGameStates;
        gamestart = hasGameStarted;

        if (GameManager.startGameStates)
        {
            Debug.Log("Should only see once");
            GameManager.startGameStates = false;
            GameManager.hasGameStarted = true;
            StartCoroutine(gameStateCoroutine(this.scatter, this.chase));
        }
    }

    IEnumerator gameStateCoroutine(int[] scatter, int[] chase)
    {
        int scatterCounter = 0, chaseCounter = 0;
        while (scatterCounter < scatter.Length || chaseCounter < chase.Length)
        {
            if (scatterCounter < scatter.Length)
            {
                int clock = scatter[scatterCounter];
                while (clock > 0)
                {
                    while (GameManager.isPoweredUp())
                    {
                        yield return new WaitForSeconds(1);
                    }
                    GameManager.setEveryonesState(GameManager.GameState.SCATTER);
                    clock--;
                    yield return new WaitForSeconds(1);
                }
                scatterCounter++;
            }

            if (chaseCounter < chase.Length)
            {
                int clock = chase[chaseCounter];
                while (clock > 0)
                {
                    while (GameManager.isPoweredUp())
                    {
                        yield return new WaitForSeconds(1);
                    }
                    GameManager.setEveryonesState(GameManager.GameState.CHASE);
                    clock--;
                    yield return new WaitForSeconds(1);
                }
                chaseCounter++;
            }

        }
        while (!GameManager.gameEnd)
        {
            while (GameManager.isPoweredUp())
            {
                yield return new WaitForSeconds(1);
            }
            GameManager.setEveryonesState(GameManager.GameState.CHASE);
            yield return new WaitForSeconds(1);
        }
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
            // make everyone frigthtened
        }

        GameManager.setEveryonesState(GameManager.GameState.STARTFRIGHTEN);
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

    public static GameManager.GameState GetGameState(pacManEnum p)
    {
        return GameManager.CurrentGameState[(int)p];
    }

    public static void ghostEaten(GameManager.pacManEnum pe)
    {
        GameManager.CurrentGameState[(int)pe] = GameManager.GameState.EATEN;
    }
    public static void ghostReturnedToBase(GameManager.pacManEnum pe)
    {
        GameManager.CurrentGameState[(int)pe] = GameManager.GameState.LEAVEGATE;
    }

    public static void ghostLeftGate(GameManager.pacManEnum pe)
    {
        GameManager.CurrentGameState[(int)pe] = GameManager.GameState.CHASE;
        if (!GameManager.startGameStates && !GameManager.hasGameStarted)
        {
            GameManager.startGameStates = true;
        }

    }
    public static void ghostHasStartedFrighten(GameManager.pacManEnum pe)
    {
        GameManager.CurrentGameState[(int)pe] = GameManager.GameState.FRIGHTEN;
    }

    public static void endGame()
    {

        lock (GameManager.poweredUpLock)
        {
            GameManager.poweredUp = 0;
            GameManager.gameEnd = true;
        }
    }


}

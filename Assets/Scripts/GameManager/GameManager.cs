using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region gameEnums
    public enum Direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, NONE = 4, ON = 5 }
    public enum GameState { SCATTER, CHASE, FRIGHTEN, STARTFRIGHTEN, START, EATEN, LEAVEGATE, RESTART, READY }
    public enum pacManEnum { RED, BLUE, PINK, TAN }
    #endregion









    #region pacman Lives
    private static object livesLock = new object();
    private static int livesLeft = 3;

    [SerializeField] private SpriteRenderer winRenderer;
    [SerializeField] private SpriteRenderer gameOverRenderer;

    public static void addLife()
    {
        lock (GameManager.livesLock)
        {
            GameManager.livesLeft += 1;
        }
    }

    public static void subtractLives()
    {
        lock (GameManager.livesLock)
        {
            GameManager.livesLeft -= 1;
        }
    }

    public static int getLives()
    {
        int num = 0;
        lock (GameManager.livesLock)
        {
            num = GameManager.livesLeft;
        }
        return num;
    }

    #endregion

    #region gameStates
    private static GameState[] CurrentGameState = { GameState.READY, GameState.READY, GameState.READY, GameState.READY };
    public GameState[] gs = GameManager.CurrentGameState;


    [SerializeField] private int[] scatter;
    [SerializeField] private int[] chase;


    private static void setEveryonesState(GameManager.GameState x)
    {
        for (int counter = 0; counter < GameManager.CurrentGameState.Length; counter++)
        {
            if (GameManager.GameState.EATEN == GameManager.CurrentGameState[counter])
            {
                continue;
            }
            GameManager.CurrentGameState[counter] = x;
        }
    }

    IEnumerator gameStateCoroutine(int[] scatter, int[] chase)
    {

        while (!GameManager.hasGameStarted)
        {
            yield return new WaitForSeconds(.1f);
        }
        GameManager.setEveryonesState(GameManager.GameState.LEAVEGATE);
        yield return new WaitForSeconds(.75f);
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



    #endregion

    #region Score
    private static object scoreLock = new object();

    private static int score = 0;


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


    #endregion

    #region Points

    public static readonly int powerUpTime = 10; // seconds

    private static object poweredUpLock = new object();
    private static object totalNumberOfPointsLock = new object();

    private static int totalNumberOfPoints = 0;

    private static int poweredUp = 0;


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


    public static void registerPoint()
    {
        lock (GameManager.totalNumberOfPointsLock)
        {
            GameManager.totalNumberOfPoints += 1;

        }
    }

    public static void subtractPoint()
    {
        lock (totalNumberOfPointsLock)
        {
            GameManager.totalNumberOfPoints -= 1;

        }
    }

    public static int getPointsLeft()
    {
        int num = 0;
        lock (GameManager.totalNumberOfPointsLock)
        {
            num = GameManager.totalNumberOfPoints;
        }
        return num;
    }



    #endregion

    #region Animation Variables

    public static int totSteps = 4;
    public static float animationTIme = .2f;
    #endregion

    #region start of game

    //private static bool startGameStates = false;
    private static bool hasGameStarted = false;
    public static void pacmanHasMoved()
    {
        if (!GameManager.hasGameStarted)
        {
            GameManager.hasGameStarted = true;
            GameManager.setEveryonesState(GameManager.GameState.START);
        }

    }

    #endregion

    #region end of game
    private static bool gameEnd = false;

    public bool isGameOver()
    {
        return GameManager.getPointsLeft() <= 0 || GameManager.getLives() <= 0;
    }
    public bool didWin()
    {
        return GameManager.getLives() > 0 && GameManager.getPointsLeft() <= 0;
    }

    private void GameOver()
    {
        // game over stuff goes in here
        lock (GameManager.poweredUpLock)
        {
            GameManager.poweredUp = 0;
            GameManager.gameEnd = true;
        }
        // pause the game
        // Determine if a win or loss
        if (this.didWin())
        {
            //win
            this.win();
        }
        else
        {
            //loss
            this.loss();
        }

        Time.timeScale = 0;
    }

    private void win()
    {
        // display the win icon
        this.gameOverRenderer.enabled = false;
        this.winRenderer.enabled = true;
    }

    private void loss()
    {
        // display game over icon
        this.winRenderer.enabled = false;
        this.gameOverRenderer.enabled = true;
    }

    #endregion

    #region Ghost Eaten

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
        // if (!GameManager.startGameStates && !GameManager.hasGameStarted)
        // {
        //     GameManager.startGameStates = true;
        // }

    }
    public static void ghostHasStartedFrighten(GameManager.pacManEnum pe)
    {
        GameManager.CurrentGameState[(int)pe] = GameManager.GameState.FRIGHTEN;
    }


    #endregion

    public static GameManager instance;


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
        GameManager.setEveryonesState(GameManager.GameState.READY);
        this.gameOverRenderer.enabled = true; // set to false
        this.winRenderer.enabled = true; // set to false
        StartCoroutine(gameStateCoroutine(this.scatter, this.chase));

    }

    private void Update()
    {
        this.gs = GameManager.CurrentGameState;
        if (this.isGameOver())
        {
            // GAME OVER (Win)
            this.GameOver();
        }



        // if (GameManager.startGameStates)
        // {
        //     Debug.Log("Should only see once");
        //     GameManager.startGameStates = false;
        //     GameManager.hasGameStarted = true;
        //     StartCoroutine(gameStateCoroutine(this.scatter, this.chase));
        // }
    }

    public static void restart()
    {
        GameManager.setEveryonesState(GameManager.GameState.RESTART);
        // stop the coroutine


    }

    public static void registerReadyRestart(pacManEnum p)
    {
        GameManager.CurrentGameState[(int)p] = GameManager.GameState.READY;
    }









}

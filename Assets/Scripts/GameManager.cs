using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] public static int score;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    public static void resetScore()
    {
        GameManager.score = 0;
    }

    public static void addScore(int score)
    {
        GameManager.score += score;
    }

    public static void subtractScore(int subScore)
    {
        GameManager.score -= subScore;
    }


}

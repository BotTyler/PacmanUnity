using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Point : MonoBehaviour
{

    private void OnTriggerEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Pacman")
        {
            //Debug.Log("gathered: " + other.gameObject.tag);
            GameManager.addScore(1);
            Destroy(this.gameObject);
        }



    }
}

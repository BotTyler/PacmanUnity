using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PowerPoint : MonoBehaviour
{

    [SerializeField] private SpriteRenderer render;

    [SerializeField] private bool isGathered;

    private void Start()
    {
        this.isGathered = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!this.isGathered && other.gameObject.tag == "Pacman")
        {
            Debug.Log("gathered: " + other.gameObject.tag);
            this.isGathered = true;
            GameManager.addScore(10);
            StartCoroutine(powerUp());

        }

    }


    IEnumerator powerUp()
    {
        //this.gameObject.SetActive(false);
        this.render.enabled = false;
        GameManager.powerUp();
        Debug.Log("POWER UP");
        yield return new WaitForSeconds(GameManager.powerUpTime);
        Debug.Log("POWER DOWN");
        GameManager.powerDown();
        Destroy(this.gameObject);
    }
}

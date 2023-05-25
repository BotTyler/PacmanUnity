using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Teleport : MonoBehaviour
{

    // Start is called before the first frame update

    [SerializeField] private GameManager.Direction exitDirection;

    [SerializeField] private Transform teleportTo;
    [SerializeField] private Tilemap teleporterTiles;

    private Vector3Int to;



    void Start()
    {
        Vector3Int toPosition = this.teleporterTiles.WorldToCell(this.teleportTo.position);

        switch (this.exitDirection)
        {
            case GameManager.Direction.ON:
                this.to = toPosition;
                break;
            case GameManager.Direction.LEFT:
                toPosition.x -= 1;
                break;
            case GameManager.Direction.RIGHT:
                toPosition.x += 1;
                break;
            case GameManager.Direction.UP:
                toPosition.y += 1;
                break;
            case GameManager.Direction.DOWN:
                toPosition.y -= 1;
                break;
        }
        this.to = toPosition;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Pacman")
        {
            other.gameObject.GetComponent<PlayerController>().teleport(this.to, this.exitDirection);
        }
        else if (other.gameObject.tag == "Ghost")
        {
            other.gameObject.GetComponent<GhostMovementInterface>().teleport(this.to, this.exitDirection);

        }
    }



}

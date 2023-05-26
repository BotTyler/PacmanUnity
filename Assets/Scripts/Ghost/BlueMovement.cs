using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlueMovement : GhostMovementInterface
{
    [SerializeField] private Transform redGhostTransform;
    internal override void Chase()
    {
        Vector3Int pacmanLocation = this.wallsMap.WorldToCell(this.pacmanGameObject.transform.position);
        Vector3Int redGhostLocation = this.wallsMap.WorldToCell(this.redGhostTransform.position);

        GameManager.Direction pacmanDirection = this.pacmanGameObject.GetComponent<PlayerController>().getCurrentDirection();
        int vertSpacing = 0;
        int horizontalSpacing = 0;

        int spacingVlaue = 2;
        switch (pacmanDirection)
        {
            case GameManager.Direction.UP:
                vertSpacing = spacingVlaue;
                horizontalSpacing = -spacingVlaue;
                break;
            case GameManager.Direction.DOWN:
                vertSpacing = -spacingVlaue;
                horizontalSpacing = 0;
                break;
            case GameManager.Direction.RIGHT:
                vertSpacing = 0;
                horizontalSpacing = spacingVlaue;
                break;
            case GameManager.Direction.LEFT:
                vertSpacing = 0;
                horizontalSpacing = -spacingVlaue;
                break;
        }
        Vector3Int nLocation = new Vector3Int(pacmanLocation.x + horizontalSpacing, pacmanLocation.y + vertSpacing, pacmanLocation.z);
        Vector3Int difference = redGhostLocation - nLocation;
        difference = nLocation - difference;

        this.targetTransform.position = new Vector3Int(difference.x, difference.y, redGhostLocation.z);
    }


}

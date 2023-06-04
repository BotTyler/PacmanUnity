using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PinkMovement : GhostMovementInterface
{
    internal override void Chase()
    {
        GameManager.Direction pacmanCurrentDirection = this.pacmanGameObject.GetComponent<PlayerController>().getCurrentDirection();
        int vertSpacing = 0;
        int horizontalSpacing = 0;

        int spacingVlaue = 2;
        switch (pacmanCurrentDirection)
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


        Vector3Int currentLocation = this.wallsMap.WorldToCell(this.pacmanGameObject.transform.position);
        this.targetTransform.position = new Vector3Int(currentLocation.x + horizontalSpacing, currentLocation.y + vertSpacing, currentLocation.z);


    }
}

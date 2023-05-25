using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TanMovement : GhostMovementInterface
{
    internal override void Chase()
    {
        Vector3Int pacmanLocation = this.wallsMap.WorldToCell(this.pacmanGameObject.transform.position);
        Vector3Int myLocation = this.wallsMap.WorldToCell(this.transform.position);

        float distance = this.distance(pacmanLocation, myLocation);

        if (distance > 5)
        {
            // follow the pacmans position
            this.targetTransform.position = this.pacmanGameObject.transform.position;
        }
        else
        {
            this.Scatter();
        }

    }

}

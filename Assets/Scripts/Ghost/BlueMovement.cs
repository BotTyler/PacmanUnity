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

        Vector3Int difference = redGhostLocation - pacmanLocation;
        difference = pacmanLocation - difference;

        this.targetTransform.position = new Vector3Int(difference.x, difference.y, redGhostLocation.z);
    }


}

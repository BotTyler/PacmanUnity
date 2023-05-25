using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedMovement : GhostMovementInterface
{
    internal override void Chase()
    {
        this.targetTransform.position = this.pacmanGameObject.transform.position;
    }

}

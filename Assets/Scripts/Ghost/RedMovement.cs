using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedMovement : GhostMovementInterface
{
    internal override void Chase()
    {
        this.targetTransform.position = this.pacmanTransform.position;
    }

    internal override void Scatter()
    {
        this.targetTransform.position = this.scatterTransform.position;
    }


}

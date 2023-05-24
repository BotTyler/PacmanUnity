using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class GhostMovementInterface : MonoBehaviour
{
    [SerializeField] internal bool canMove = false;
    [SerializeField] internal Transform lookAheadTransform;


    internal abstract void Frightened();
    internal abstract void Scatter();
    internal abstract void Chase();
    internal abstract void Eaten();
    internal abstract void StartGame();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.GetGameState())
        {

            case GameManager.GameState.START:
                this.StartGame();
                break;
            case GameManager.GameState.CHASE:
                this.Chase();
                break;
            case GameManager.GameState.FRIGHTEN:
                this.Frightened();
                break;
            case GameManager.GameState.SCATTER:
                this.Scatter();
                break;
        }

    }

    /// <summary>
    /// This method handles how the ghost are supposed to move
    /// given a specific cell and a current direction the following rules are followed.
    /// 1) the ghost cannot move backwards
    /// 2) of the 3 different cell locations, find the distance from there to the target location.
    /// 3) pick the minimum distance of those locations.
    /// 4) move that way.
    /// </summary>
    /// <param name="ghostTransform">The ghost transform, normally just the gameobject this script is attached to.</param>
    /// <param name="targetLocation">Based on current state of the game, but can be either the corner of the map or based on pacmans current position.</param>
    /// <param name="curDirection">The last known direction of the ghost.</param>
    /// <param name="map">The tilemap of the walls</param>
    private Vector3Int calcMovement(Transform ghostTransform, Transform targetLocation, GameManager.Direction curDirection, Tilemap wallMap)
    {
        // First steps is to get the values of the square I need to calculate.
        List<Vector3Int> potentialSquares = new List<Vector3Int>(3);

        Vector3Int targetSquare = wallMap.WorldToCell(targetLocation.position);

        Vector3Int currentLocation = wallMap.WorldToCell(ghostTransform.position);

        switch (curDirection)
        {
            case GameManager.Direction.UP:
                // the only options are up, left and right.
                potentialSquares[0] = upLocation(currentLocation);
                potentialSquares[1] = leftLocation(currentLocation);
                potentialSquares[2] = rightLocation(currentLocation);

                break;

            case GameManager.Direction.DOWN:
                // the only options are down, left and right.
                potentialSquares[0] = downLocation(currentLocation);
                potentialSquares[1] = leftLocation(currentLocation);
                potentialSquares[2] = rightLocation(currentLocation);
                break;

            case GameManager.Direction.LEFT:
                // the only options are up, down and right.
                potentialSquares[0] = upLocation(currentLocation);
                potentialSquares[1] = leftLocation(currentLocation);
                potentialSquares[2] = downLocation(currentLocation);

                break;

            case GameManager.Direction.RIGHT:
                // the only options are up, left and down.
                potentialSquares[0] = upLocation(currentLocation);
                potentialSquares[1] = rightLocation(currentLocation);
                potentialSquares[2] = downLocation(currentLocation);
                break;

        }

        potentialSquares = removeSomePaths(potentialSquares, wallMap);
        int targetCellIndex = findMin(potentialSquares, targetSquare);

        // now that we have our target square now to make the ghost move
        return potentialSquares[targetCellIndex];
    }

    IEnumerator movementCoroutine(int totSteps, float animationTime, Vector2 curPosition, Vector2 toPosition)
    {

        float incrementalTimeWait = animationTime / ((float)totSteps);
        Vector2 movementStep = toPosition - curPosition;
        movementStep /= (float)totSteps;


        this.lookAheadTransform.position = toPosition;
        this.canMove = false;
        for (int counter = 0; counter < totSteps; counter++)
        {
            // slowly change the movement here
            this.transform.position += new Vector3(movementStep.x, movementStep.y, this.transform.position.z);

            yield return new WaitForSeconds(incrementalTimeWait);
        }
        this.canMove = true;
    }

    private List<Vector3Int> removeSomePaths(List<Vector3Int> potentials, Tilemap map)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        for (int counter = 0; counter < potentials.Count; counter++)
        {
            if (isCurrentTileAvail(potentials[counter], map))
            {
                result.Add(potentials[counter]);
            }
        }

        return result;
    }
    private bool isCurrentTileAvail(Vector3Int tile, Tilemap map)
    {
        Tile x = map.GetTile<Tile>(tile);

        return x == null;
    }

    private Vector3Int rightLocation(Vector3Int baseLocation)
    {
        return new Vector3Int(baseLocation.x + 1, baseLocation.y, baseLocation.z);
    }
    private Vector3Int leftLocation(Vector3Int baseLocation)
    {
        return new Vector3Int(baseLocation.x - 1, baseLocation.y, baseLocation.z);
    }
    private Vector3Int upLocation(Vector3Int baseLocation)
    {
        return new Vector3Int(baseLocation.x, baseLocation.y + 1, baseLocation.z);
    }
    private Vector3Int downLocation(Vector3Int baseLocation)
    {
        return new Vector3Int(baseLocation.x, baseLocation.y - 1, baseLocation.z);
    }


    /// <summary>
    /// Calculate the distance between two points.
    /// </summary>
    /// <param name="current">current position</param>
    /// <param name="target">target position</param>
    /// <returns>float representing the distance</returns>
    private float distance(Vector3Int current, Vector3Int target)
    {
        float xDifference = target.x - current.x;
        float xSquare = xDifference * xDifference;

        float yDifference = target.y - current.y;
        float ySquare = yDifference * yDifference;

        return Mathf.Sqrt(xSquare + ySquare);
    }



    /// <summary>
    /// find the minimum value in the array and output the index which is the smallest.
    /// </summary>
    /// <param name="x">list of points</param>
    /// <returns>the index of the smallest value in the array</returns>
    private int findMin(List<Vector3Int> x, Vector3Int target)
    {
        int minIndex = 0;
        float minValue = distance(x[0], target);

        for (int counter = 1; counter < x.Count; counter++)
        {
            float val = distance(x[counter], target);
            if (val < minValue)
            {
                minIndex = counter;
                minValue = val;
            }
        }

        return minIndex;
    }


}

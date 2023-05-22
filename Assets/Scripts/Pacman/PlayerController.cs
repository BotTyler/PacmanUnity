using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public enum direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, NONE = 4 };
    [SerializeField] private Rigidbody2D rb2d = null;

    [SerializeField] private direction curDirection;
    [SerializeField] private direction wantDirection;

    [SerializeField] private Tilemap wallMap;

    [SerializeField] private Tilemap gateMap;

    [SerializeField] private Transform parentGameObjectTransform;
    //public Vector3 location;
    [SerializeField] private bool[] availDirections = { false, false, false, false, false };
    [SerializeField] private Vector2 finalVelocity = new Vector2(0, 0);

    private Vector3Int upLoc;
    private Vector3Int downLoc;
    private Vector3Int rightLoc;
    private Vector3Int leftLoc;
    [SerializeField] private bool canMove = true;


    [SerializeField] private int animationSteps = 4;

    [SerializeField] private float movementCooldown = 1f;


    // Start is called before the first frame update
    void Start()
    {

        this.curDirection = direction.NONE;

    }

    // Update is called once per frame
    void Update()
    {

        findAdjacentTiles(this.parentGameObjectTransform.position);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.wantDirection = direction.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            this.wantDirection = direction.LEFT;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            this.wantDirection = direction.UP;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.wantDirection = direction.DOWN;
        }


        if (this.availDirections[(int)this.wantDirection])
        {
            this.curDirection = this.wantDirection;
            this.wantDirection = direction.NONE;
        }




    }

    private void FixedUpdate()
    {

        float rotation = this.rb2d.rotation;
        Vector2 position = this.rb2d.position;
        if (this.canMove)
        {
            if (this.availDirections[(int)this.curDirection])
            {
                switch (this.curDirection)
                {
                    case direction.UP:
                        position.x = this.upLoc.x + .5f;
                        position.y = this.upLoc.y + .5f;
                        rotation = 90;
                        break;
                    case direction.RIGHT:
                        position.x = this.rightLoc.x + .5f;
                        position.y = this.rightLoc.y + .5f;
                        rotation = 0;
                        break;
                    case direction.LEFT:
                        position.x = this.leftLoc.x + .5f;
                        position.y = this.leftLoc.y + .5f;
                        rotation = 180;
                        break;
                    case direction.DOWN:
                        position.x = this.downLoc.x + .5f;
                        position.y = this.downLoc.y + .5f;

                        rotation = 270;
                        break;

                }
            }
            this.rb2d.rotation = rotation;
            //this.rb2d.position = position;


            StartCoroutine(movementCoroutine(this.animationSteps, this.movementCooldown, this.rb2d.position, position));
        }


    }

    IEnumerator movementCoroutine(int totSteps, float animationTime, Vector2 curPosition, Vector2 toPosition)
    {

        float incrementalTimeWait = animationTime / ((float)totSteps);
        Vector2 movementStep = toPosition - curPosition;
        movementStep /= (float)totSteps;


        this.parentGameObjectTransform.position = toPosition;
        this.canMove = false;
        for (int counter = 0; counter < totSteps; counter++)
        {
            // slowly change the movement here
            this.rb2d.position += movementStep;

            yield return new WaitForSeconds(incrementalTimeWait);
        }
        this.canMove = true;
    }

    /// <summary>
    /// Method to find all 4 directions of a tile, used to see if a adjacent tiles is possible to go to.
    /// </summary>
    /// <returns>4 value array [UP, DOWN, LEFT, RIGHT] true - possible to go to, false - not possible to go to.</returns>
    private void findAdjacentTiles(Vector2 currentLocation)
    {
        bool[] adj = { false, false, false, false, false }; //access using the direction enum
        //Vector2 currentLocation = this.rb2d.position;

        Vector3Int location = wallMap.WorldToCell(currentLocation);
        upLoc = new Vector3Int(location.x, location.y + 1, location.z);
        downLoc = new Vector3Int(location.x, location.y - 1, location.z);
        rightLoc = new Vector3Int(location.x + 1, location.y, location.z);
        leftLoc = new Vector3Int(location.x - 1, location.y, location.z);

        Tile upTile = wallMap.GetTile<Tile>(upLoc);
        Tile downTile = wallMap.GetTile<Tile>(downLoc);
        Tile rightTile = wallMap.GetTile<Tile>(rightLoc);
        Tile leftTile = wallMap.GetTile<Tile>(leftLoc);

        Tile upTileGate = gateMap.GetTile<Tile>(upLoc);
        Tile downTileGate = gateMap.GetTile<Tile>(downLoc);
        Tile rightTileGate = gateMap.GetTile<Tile>(rightLoc);
        Tile leftTileGate = gateMap.GetTile<Tile>(leftLoc);


        // populating the array
        adj[(int)direction.UP] = isCurrentTileAvall(upTile) && isCurrentTileAvall(upTileGate);
        adj[(int)direction.DOWN] = isCurrentTileAvall(downTile) && isCurrentTileAvall(downTileGate);
        adj[(int)direction.RIGHT] = isCurrentTileAvall(rightTile) && isCurrentTileAvall(rightTileGate);
        adj[(int)direction.LEFT] = isCurrentTileAvall(leftTile) && isCurrentTileAvall(leftTileGate);

        //printArray(adj);
        this.availDirections = adj;
    }

    private void printArray(bool[] x)
    {
        bool upVal = x[(int)direction.UP];
        bool downVal = x[(int)direction.DOWN];
        bool leftVal = x[(int)direction.LEFT];
        bool rightVal = x[(int)direction.RIGHT];

        Debug.Log("[" + upVal + ", " + downVal + ", " + leftVal + ", " + rightVal + "]");

    }

    private bool isCurrentTileAvall(Tile tile)
    {
        return tile == null;
    }

}

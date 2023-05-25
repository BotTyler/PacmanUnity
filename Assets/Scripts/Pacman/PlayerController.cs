using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    //public enum direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, NONE = 4 };
    [SerializeField] private Rigidbody2D rb2d = null;

    [SerializeField] private GameManager.Direction curDirection;
    [SerializeField] private GameManager.Direction wantDirection;

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



    [SerializeField] private Vector3 teleportLocation;
    private bool isTeleporting = false;


    // Start is called before the first frame update
    void Start()
    {

        this.curDirection = GameManager.Direction.NONE;

    }

    // Update is called once per frame
    void Update()
    {

        if (this.isTeleporting)
        {
            this.curDirection = this.wantDirection;
            //this.wantDirection = GameManager.Direction.NONE;
            return;
        }


        findAdjacentTiles(this.parentGameObjectTransform.position);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.wantDirection = GameManager.Direction.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            this.wantDirection = GameManager.Direction.LEFT;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            this.wantDirection = GameManager.Direction.UP;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.wantDirection = GameManager.Direction.DOWN;
        }

        if (this.availDirections[(int)this.wantDirection])
        {
            this.curDirection = this.wantDirection;
            this.wantDirection = GameManager.Direction.NONE;
        }




    }

    private void FixedUpdate()
    {

        float rotation = this.rb2d.rotation;
        Vector2 position = this.rb2d.position;
        if (this.canMove)
        {
            if (!this.isTeleporting)
            {
                if (this.availDirections[(int)this.curDirection])
                {
                    switch (this.curDirection)
                    {
                        case GameManager.Direction.UP:
                            position.x = this.upLoc.x + .5f;
                            position.y = this.upLoc.y + .5f;
                            rotation = 90;
                            break;
                        case GameManager.Direction.RIGHT:
                            position.x = this.rightLoc.x + .5f;
                            position.y = this.rightLoc.y + .5f;
                            rotation = 0;
                            break;
                        case GameManager.Direction.LEFT:
                            position.x = this.leftLoc.x + .5f;
                            position.y = this.leftLoc.y + .5f;
                            rotation = 180;
                            break;
                        case GameManager.Direction.DOWN:
                            position.x = this.downLoc.x + .5f;
                            position.y = this.downLoc.y + .5f;

                            rotation = 270;
                            break;

                    }
                }
                StartCoroutine(movementCoroutine(GameManager.totSteps, GameManager.animationTIme, this.rb2d.position, position));

            }
            else
            {
                this.teleportLocation.x += .5f;
                this.teleportLocation.y += .5f;
                position = this.teleportLocation;
                StartCoroutine(movementCoroutine(1, 0, this.rb2d.position, position));
                this.isTeleporting = false;
            }

            this.rb2d.rotation = rotation;
            //this.rb2d.position = position;


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
        adj[(int)GameManager.Direction.UP] = isCurrentTileAvall(upTile) && isCurrentTileAvall(upTileGate);
        adj[(int)GameManager.Direction.DOWN] = isCurrentTileAvall(downTile) && isCurrentTileAvall(downTileGate);
        adj[(int)GameManager.Direction.RIGHT] = isCurrentTileAvall(rightTile) && isCurrentTileAvall(rightTileGate);
        adj[(int)GameManager.Direction.LEFT] = isCurrentTileAvall(leftTile) && isCurrentTileAvall(leftTileGate);

        //printArray(adj);
        this.availDirections = adj;
    }

    private void printArray(bool[] x)
    {
        bool upVal = x[(int)GameManager.Direction.UP];
        bool downVal = x[(int)GameManager.Direction.DOWN];
        bool leftVal = x[(int)GameManager.Direction.LEFT];
        bool rightVal = x[(int)GameManager.Direction.RIGHT];

        Debug.Log("[" + upVal + ", " + downVal + ", " + leftVal + ", " + rightVal + "]");

    }

    private bool isCurrentTileAvall(Tile tile)
    {
        return tile == null;
    }
    public void teleport(Vector3 teleportLocation, GameManager.Direction dir)
    {
        this.teleportLocation = teleportLocation;
        this.isTeleporting = true;
        this.wantDirection = dir;
    }

    public GameManager.Direction getCurrentDirection()
    {
        return this.curDirection;
    }

}

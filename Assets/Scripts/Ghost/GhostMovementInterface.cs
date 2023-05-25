using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public abstract class GhostMovementInterface : MonoBehaviour
{
    #region sprites
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [SerializeField] private Sprite upSpriteEaten;
    [SerializeField] private Sprite downSpriteEaten;
    [SerializeField] private Sprite leftSpriteEaten;
    [SerializeField] private Sprite rightSpriteEaten;

    [SerializeField] private Sprite frightenSprite;

    #endregion


    [SerializeField] internal bool canMove = true;
    [SerializeField] internal Transform lookAheadTransform;
    [SerializeField] internal Transform pacmanTransform;
    [SerializeField] internal Transform targetTransform;
    [SerializeField] internal Transform scatterTransform;
    [SerializeField] internal Transform leaveGateTransform;

    [SerializeField] internal Transform deathTransform;
    [SerializeField] internal Tilemap wallsMap;

    [SerializeField] internal Tilemap gateMap;

    [SerializeField] internal GameManager.Direction curDirection = GameManager.Direction.NONE;




    [SerializeField] internal Vector3 teleportLocation;
    private bool isTeleporting = false;
    [SerializeField] internal bool hasMovedThroughGate = false;

    [SerializeField] internal GameManager.pacManEnum _pacmanEnum;
    [SerializeField] internal GameManager.GameState currentState = GameManager.GameState.START;


    internal void startFrighten()
    {
        Vector3Int currentLocation = this.wallsMap.WorldToCell(this.lookAheadTransform.position);


        // FLIP
        switch (this.curDirection)
        {
            case GameManager.Direction.UP:
                this.targetTransform.position = new Vector3Int(currentLocation.x, currentLocation.y - 2, currentLocation.z);
                this.curDirection = GameManager.Direction.DOWN;
                break;
            case GameManager.Direction.DOWN:
                this.targetTransform.position = new Vector3Int(currentLocation.x, currentLocation.y + 2, currentLocation.z);
                this.curDirection = GameManager.Direction.UP;

                break;
            case GameManager.Direction.LEFT:
                this.targetTransform.position = new Vector3Int(currentLocation.x + 2, currentLocation.y, currentLocation.z);
                this.curDirection = GameManager.Direction.RIGHT;

                break;
            case GameManager.Direction.RIGHT:
                this.targetTransform.position = new Vector3Int(currentLocation.x - 2, currentLocation.y, currentLocation.z);
                this.curDirection = GameManager.Direction.LEFT;

                break;
        }

        GameManager.ghostHasStartedFrighten(this._pacmanEnum);
    }

    /*
        All the methods below need to calculate where the new target location in
    */
    internal void Frightened()
    {
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);


        int xRandom = (rand.Next(3) - 1) * 2; // from -2 to 2
        int yRandom = (rand.Next(3) - 1) * 2; // from -2 to 2
        Vector3Int currentLocation = this.wallsMap.WorldToCell(this.lookAheadTransform.position);
        this.targetTransform.position = new Vector3Int(currentLocation.x + xRandom, currentLocation.y + yRandom, currentLocation.z);

    }
    internal abstract void Scatter();
    internal abstract void Chase();

    internal void Eaten()
    {
        this.hasMovedThroughGate = false;


        EatenLoc();
    }

    internal void EatenLoc()
    {
        this.targetTransform.position = this.deathTransform.position;
    }

    internal void StartGame()
    {
        this.targetTransform.position = this.transform.position;

    }

    internal void leaveGate()
    {
        this.targetTransform.position = this.leaveGateTransform.position;
        // tell game manager that we left the gate
        // GameManager.ghostLeftGate(this._pacmanEnum);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.canMove = true;
        this.hasMovedThroughGate = false;
    }

    // Update is called once per frame
    void Update()
    {

        this.currentState = GameManager.GetGameState(this._pacmanEnum);
        switch (this.currentState)
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
            case GameManager.GameState.EATEN:
                this.EatenLoc();
                break;
            case GameManager.GameState.LEAVEGATE:
                this.leaveGate();
                break;
            case GameManager.GameState.STARTFRIGHTEN:
                this.startFrighten();
                break;
        }


    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            if (this.isTeleporting)
            {
                this.teleportLocation.x += .5f;
                this.teleportLocation.y += .5f;
                StartCoroutine(this.movementCoroutine(1, 0, this.transform.position, teleportLocation));
                this.isTeleporting = false;
            }
            else if (this.currentState == GameManager.GameState.EATEN)
            {
                Vector3Int moveLocation = this.calcMovement(this.lookAheadTransform, this.targetTransform, this.curDirection, this.wallsMap, this.gateMap);
                Vector2 loc = new Vector2(moveLocation.x + 0.5f, moveLocation.y + 0.5f);
                StartCoroutine(this.movementCoroutine(GameManager.totSteps, 0.01f, this.transform.position, loc));

            }
            else
            {

                Vector3Int moveLocation = this.calcMovement(this.lookAheadTransform, this.targetTransform, this.curDirection, this.wallsMap, this.gateMap);
                Vector2 loc = new Vector2(moveLocation.x + 0.5f, moveLocation.y + 0.5f);
                StartCoroutine(this.movementCoroutine(GameManager.totSteps, GameManager.animationTIme, this.transform.position, loc));

            }


            switch (this.curDirection)
            {
                // may need to add the frightened state
                case GameManager.Direction.UP:
                    determineUpSprite(this.currentState);
                    break;
                case GameManager.Direction.DOWN:
                    determineDownSprite(this.currentState);
                    break;
                case GameManager.Direction.LEFT:
                    determineLeftSprite(this.currentState);
                    break;
                case GameManager.Direction.RIGHT:
                    determineRightSprite(this.currentState);
                    break;

            }

        }

    }

    public void determineUpSprite(GameManager.GameState a)
    {
        switch (a)
        {

            case GameManager.GameState.START:
                this.sr.sprite = this.upSprite;
                break;
            case GameManager.GameState.CHASE:
                this.sr.sprite = this.upSprite;
                break;
            case GameManager.GameState.FRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
            case GameManager.GameState.SCATTER:
                this.sr.sprite = this.upSprite;
                break;
            case GameManager.GameState.EATEN:
                this.sr.sprite = this.upSpriteEaten;
                break;
            case GameManager.GameState.LEAVEGATE:
                this.sr.sprite = this.upSprite;
                break;
            case GameManager.GameState.STARTFRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
        }
        // if (GameManager.GetGameState(this._pacmanEnum))
        // {
        //     this.sr.sprite = this.upSpriteEaten;
        // }
        // else if (this.localState == GameManager.GameState.FRIGHTEN)
        // {
        //     this.sr.sprite = this.frightenSprite;
        // }
        // else
        // {
        //     this.sr.sprite = this.upSprite;
        // }
    }
    public void determineDownSprite(GameManager.GameState a)
    {
        switch (a)
        {

            case GameManager.GameState.START:
                this.sr.sprite = this.downSprite;
                break;
            case GameManager.GameState.CHASE:
                this.sr.sprite = this.downSprite;
                break;
            case GameManager.GameState.FRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
            case GameManager.GameState.SCATTER:
                this.sr.sprite = this.downSprite;
                break;
            case GameManager.GameState.EATEN:
                this.sr.sprite = this.downSpriteEaten;
                break;
            case GameManager.GameState.LEAVEGATE:
                this.sr.sprite = this.downSprite;
                break;
            case GameManager.GameState.STARTFRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
        }
    }
    public void determineLeftSprite(GameManager.GameState a)
    {
        switch (a)
        {

            case GameManager.GameState.START:
                this.sr.sprite = this.leftSprite;
                break;
            case GameManager.GameState.CHASE:
                this.sr.sprite = this.leftSprite;
                break;
            case GameManager.GameState.FRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
            case GameManager.GameState.SCATTER:
                this.sr.sprite = this.leftSprite;
                break;
            case GameManager.GameState.EATEN:
                this.sr.sprite = this.leftSpriteEaten;
                break;
            case GameManager.GameState.LEAVEGATE:
                this.sr.sprite = this.leftSprite;
                break;
            case GameManager.GameState.STARTFRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
        }
    }
    public void determineRightSprite(GameManager.GameState a)
    {
        switch (a)
        {

            case GameManager.GameState.START:
                this.sr.sprite = this.rightSprite;
                break;
            case GameManager.GameState.CHASE:
                this.sr.sprite = this.rightSprite;
                break;
            case GameManager.GameState.FRIGHTEN:
                this.sr.sprite = this.frightenSprite;
                break;
            case GameManager.GameState.SCATTER:
                this.sr.sprite = this.rightSprite;
                break;
            case GameManager.GameState.EATEN:
                this.sr.sprite = this.rightSpriteEaten;
                break;
            case GameManager.GameState.LEAVEGATE:
                this.sr.sprite = this.rightSprite;
                break;
            case GameManager.GameState.STARTFRIGHTEN:
                this.sr.sprite = this.frightenSprite;
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
    /// <param name="gates">The tilemap of the gates</param>
    internal Vector3Int calcMovement(Transform ghostTransform, Transform targetLocation, GameManager.Direction curDirection, Tilemap wallMap, Tilemap gates)
    {
        // First steps is to get the values of the square I need to calculate.
        //List<Vector3Int?> potentialSquares = new List<Vector3Int?>(5);

        Vector3Int targetSquare = wallMap.WorldToCell(targetLocation.position);

        Vector3Int currentLocation = wallMap.WorldToCell(ghostTransform.position);

        // potentialSquares[(int)GameManager.Direction.UP] = upLocation(currentLocation);
        // potentialSquares[(int)GameManager.Direction.DOWN] = downLocation(currentLocation);
        // potentialSquares[(int)GameManager.Direction.LEFT] = leftLocation(currentLocation);
        // potentialSquares[(int)GameManager.Direction.RIGHT] = rightLocation(currentLocation);

        Vector3Int?[] potentialSquares = { upLocation(currentLocation), downLocation(currentLocation), leftLocation(currentLocation), rightLocation(currentLocation) };

        switch (curDirection)
        {
            case GameManager.Direction.UP:
                // the only options are up, left and right.
                potentialSquares[(int)GameManager.Direction.DOWN] = null;

                break;

            case GameManager.Direction.DOWN:
                // the only options are down, left and right.
                potentialSquares[(int)GameManager.Direction.UP] = null;

                break;

            case GameManager.Direction.LEFT:
                // the only options are up, down and right.
                potentialSquares[(int)GameManager.Direction.RIGHT] = null;


                break;

            case GameManager.Direction.RIGHT:
                // the only options are up, left and down.
                potentialSquares[(int)GameManager.Direction.LEFT] = null;

                break;

        }

        potentialSquares = removeSomePaths(potentialSquares, wallMap, gates);
        int targetCellIndex = findMin(potentialSquares, targetSquare);
        this.curDirection = (GameManager.Direction)targetCellIndex;

        // now that we have our target square now to make the ghost move
        return (Vector3Int)potentialSquares[targetCellIndex];
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

    private Vector3Int?[] removeSomePaths(Vector3Int?[] potentials, Tilemap map, Tilemap gates)
    {
        for (int counter = 0; counter < potentials.Length; counter++)
        {

            if (potentials[counter] == null)
            {
                continue;
            }

            Vector3Int val = (Vector3Int)potentials[counter];

            if (!isCurrentTileAvail(val, map, gates))
            {
                potentials[counter] = null;
            }

        }

        return potentials;
    }
    private bool isCurrentTileAvail(Vector3Int tile, Tilemap map, Tilemap gates)
    {
        Tile mapX = map.GetTile<Tile>(tile);
        Tile gateX = gates.GetTile<Tile>(tile);
        if (!this.hasMovedThroughGate)
        {
            return mapX == null;
        }
        else
        {
            return mapX == null && gateX == null;
        }
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
    private int findMin(Vector3Int?[] x, Vector3Int target)
    {
        int minIndex = -1;
        float minValue = int.MaxValue;

        for (int counter = 0; counter < x.Length; counter++)
        {
            if (x[counter] == null)
            {
                continue;
            }
            float val = distance((Vector3Int)x[counter], target);
            if (val < minValue)
            {
                minIndex = counter;
                minValue = val;
            }
        }

        return minIndex;
    }


    public void teleport(Vector3 teleportLocation, GameManager.Direction dir)
    {
        this.teleportLocation = teleportLocation;
        this.isTeleporting = true;
        this.curDirection = dir;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Gate")
        {
            Debug.Log("MOVED THROUGH GATE: " + other.gameObject.tag);

            this.hasMovedThroughGate = true;
        }

        if (other.gameObject.tag == "EatenSpawn")
        {
            this.hasMovedThroughGate = false;

        }
        if (other.gameObject.tag == "Outgate" && this.currentState == GameManager.GameState.LEAVEGATE)
        {
            GameManager.ghostLeftGate(this._pacmanEnum);

        }

        if (other.gameObject.tag == "Pacman")
        {
            if (this.currentState == GameManager.GameState.FRIGHTEN)
            {
                Eaten();
                GameManager.ghostEaten(this._pacmanEnum);
            }
            else
            {
                // lose life
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
        wait,
        move
}

public enum TileKind{
    breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType{
    public Vector2Int tilePosition;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    //change later
    public GameState currentState;
    public Vector2Int size;
    [SerializeField]private int offSet;
    [SerializeField] private GameObject titlePrefab;
    [SerializeField] private GameObject BreakableTilePrefab;
    [SerializeField] private List<GameObject> dots;
    [SerializeField] private GameObject DestroyEffect;
    [SerializeField] private List<TileType> boardLayout;
    private bool[,] blankSpaces;
    private bgTitle[,] breakableTiles;
    //change later
    public GameObject[,]allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    [SerializeField] private int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    private float refillDelay = 0.5f;
    public List<int> scoreGoals;

    private void Start() {
        breakableTiles = new bgTitle[size.x, size.y];
        //currentState = GameState.move;
        goalManager = FindObjectOfType<GoalManager>();
        findMatches=FindObjectOfType<FindMatches>();
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        blankSpaces = new bool[size.x, size.y];
        allDots = new GameObject[size.x, size.y];
        SetUP();
    }

    public void GenerateBlankSpaces(){
        for (int i = 0; i < boardLayout.Count; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Blank){
                blankSpaces[boardLayout[i].tilePosition.x, boardLayout[i].tilePosition.y] = true;
            }
        }
    }

    public void GenerateBreakableTiles(){
        //look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Count; i++)
        {
            //if a tile is a "Jelly" tile
            if(boardLayout[i].tileKind==TileKind.breakable){
                //create a "Jelly" tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].tilePosition.x,
                boardLayout[i].tilePosition.y);
                GameObject tile = Instantiate(
                    BreakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].tilePosition.x,
                boardLayout[i].tilePosition.y] = tile.GetComponent<bgTitle>();
            }
        }
    }

    private void SetUP(){
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (!blankSpaces[ix, iy])
                {
                    Vector2 tempPosition = new Vector2(ix, iy + offSet);
                    Vector2 tempPositionBG = new Vector2(ix, iy);
                    //bg
                    GameObject bgTitle = Instantiate(
                        titlePrefab, tempPositionBG, Quaternion.identity);
                    bgTitle.transform.SetParent(transform);
                    bgTitle.name = "( " + ix + ", " + iy + " )";

                    //dots
                    int dotToUse = Random.Range(0, dots.Count);
                    int maxIterations = 0;

                    while (MatchesAt(new Vector2Int(ix, iy), dots[dotToUse]) && 
                        maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Count);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(
                    dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().DotPosition(ix, iy);
                    dot.transform.SetParent(transform);
                    dot.name = "( " + ix + ", " + iy + " )";
                    allDots[ix, iy] = dot;
                }
            }
        }
    }
    private bool MatchesAt(Vector2Int positionPiece, GameObject piece){
        if(positionPiece.x > 1 && positionPiece.y > 1){
            if (allDots[positionPiece.x - 1, positionPiece.y] != null &&
            allDots[positionPiece.x - 2, positionPiece.y] != null)
            {
                if (allDots[positionPiece.x - 1, positionPiece.y].tag == piece.tag &&
                allDots[positionPiece.x - 2, positionPiece.y].tag == piece.tag)
                {
                    return true;
                }
            }
             
            if (allDots[positionPiece.x, positionPiece.y - 1] != null &&
            allDots[positionPiece.x, positionPiece.y - 2] != null)
            {
                if (allDots[positionPiece.x, positionPiece.y - 1].tag == piece.tag &&
                allDots[positionPiece.x, positionPiece.y - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }else if(positionPiece.x <= 1 || positionPiece.y <= 1){
            if (positionPiece.y > 1)
            {
                if ((allDots[positionPiece.x, positionPiece.y - 1] != null &&
                allDots[positionPiece.x, positionPiece.y - 2] != null))
                {
                    if (allDots[positionPiece.x, positionPiece.y - 1].tag == piece.tag &&
                allDots[positionPiece.x, positionPiece.y - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            if (positionPiece.x > 1)
            {
                if ((allDots[positionPiece.x-1, positionPiece.y] != null &&
                allDots[positionPiece.x-2, positionPiece.y] != null))
                {
                    if (allDots[positionPiece.x - 1, positionPiece.y].tag == piece.tag &&
                    allDots[positionPiece.x - 2, positionPiece.y].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool ColumnOrRow(){
        Vector2Int number= Vector2Int.zero;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (var item in findMatches.currentMatches)
            {
                Dot dot = item.GetComponent<Dot>();
                if(dot.dotPosition.y ==firstPiece.dotPosition.y){
                    number.x++;
                }
                if(dot.dotPosition.x ==firstPiece.dotPosition.x){
                    number.y++;
                }
            }
        }
        return (number.x ==5 || number.y==5);
    }
    private void CheckToMakeBombs(){
        if(findMatches.currentMatches.Count==4 || findMatches.currentMatches.Count==7){
            findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count==5 || findMatches.currentMatches.Count==8){
            if(ColumnOrRow()){
                //make a color bomb
                //is the current dot matched
                if(currentDot !=null){
                    if(currentDot.isMatched){
                        if(!currentDot.isColorBomb){
                            currentDot.isMatched = false;
                            currentDot.MakeColumnBomb();
                        }
                    }else{
                        if(currentDot.otherDot != null){
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isColorBomb){
                                otherDot.isMatched = false;
                                otherDot.MakeColorBomb();
                            }
                        }
                    }
                }
            }else{
                //make a adjacent bomb
                //is the current dot matched
                if(currentDot !=null){
                    if(currentDot.isMatched){
                        if(!currentDot.isAdjacentBomb ){
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }else{
                        if(currentDot.otherDot != null){
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if(otherDot.isAdjacentBomb){
                                otherDot.isMatched = false;
                                otherDot.MakeAdjacentBomb();
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void DestroyMatchesAt(Vector2Int positionPiece){
        if(allDots[positionPiece.x, positionPiece.y].GetComponent<Dot>().isMatched){
            //how many elements are in the matched pieces list from findMatches
            if(findMatches.currentMatches.Count >=4){
                CheckToMakeBombs();
            }
            // Does a tile need to break
            if(breakableTiles[positionPiece.x,positionPiece.y] != null){
                //if it does, give damage
                breakableTiles[positionPiece.x, positionPiece.y].TakeDamage(1);
                if(breakableTiles[positionPiece.x, positionPiece.y].hitPoints <=0){
                    breakableTiles[positionPiece.x, positionPiece.y] = null;
                }
            }
            
            if(goalManager !=null){
                goalManager.CompareGoal(allDots[positionPiece.x, positionPiece.y].tag.ToString());
                goalManager.UpdateGoals();
            }

            //does the sound manager exist
            if(soundManager !=null){
                soundManager.PlayRandomDestroyNoise();
            }
            GameObject particle= Instantiate(DestroyEffect, 
            allDots[positionPiece.x, positionPiece.y].transform.position,Quaternion.identity);
            Destroy(particle, 1.0f);
            Destroy(allDots[positionPiece.x, positionPiece.y]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[positionPiece.x, positionPiece.y] = null;
        }
    }
    public void DestroyMatches(){
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] !=null)
                {
                    DestroyMatchesAt(new Vector2Int(ix, iy));
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }
    private IEnumerator DecreaseRowCo2(){
   
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                //if the current spot isn`t blank and empty ...
                if (!blankSpaces[ix, iy] && allDots[ix, iy] == null)
                {
                    //loop from the space above to the top of the column
                    for (int k = iy + 1; k < size.y; k++)
                    {
                        //if a dot is found...
                        if (allDots[ix, k] != null)
                        {
                            //move that dot to this empty space
                            allDots[ix, k].GetComponent<Dot>().dotPosition.y = iy;
                            //set that spot to be null
                            allDots[ix, k] = null;
                            //break out of the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay*0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator DecreaseRowCo(){
        int nullCount = 0;
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            { 
                if (allDots[ix, iy] ==null)
                {
                    nullCount++;
                }else if(nullCount >0){
                    allDots[ix, iy].GetComponent<Dot>().dotPosition.y-= nullCount;
                    allDots[ix, iy] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay*0.5f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard(){
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] == null && !blankSpaces[ix,iy])
                { 
                    Vector2 tempPosition = new Vector2(ix, iy+offSet);
                    int dotToUse = Random.Range(0, dots.Count);
                    int maxIterations = 0;
                    while ( MatchesAt( new Vector2Int(ix,iy),dots[dotToUse]) && 
                    maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Count);
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(
                    dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.SetParent(transform);
                    dot.name = "( " + ix + ", " + iy + " )";
                    allDots[ix, iy] = dot;
                    dot.GetComponent<Dot>().DotPosition(ix,iy);
                }
            }
        }
    }
    private bool MatchesOnBoard(){
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] != null)
                { 
                    if(allDots[ix, iy].GetComponent<Dot>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCo(){
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(2*refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        if(IsDeadlocked()){
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(Vector2Int piecesPosition, Vector2Int direction){
        //Take the second piece and save it in a holder
        GameObject holder = allDots[piecesPosition.x + direction.x, 
            piecesPosition.y + direction.y];
        //switching the first dot to be the second position
        allDots[piecesPosition.x + direction.x, piecesPosition.y + direction.y]=
            allDots[piecesPosition.x, piecesPosition.y];
        //set the first dot to be the second dot
        allDots[piecesPosition.x, piecesPosition.y]=holder;
    }

    private bool CheckForMatches(){
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] != null)
                {
                    //make sure that one and two to the right are in the board
                    if (ix < size.x - 2)
                    {

                        //check if the dots to the right and two to the right exist  
                        if (allDots[ix + 1, iy] != null && allDots[ix + 2, iy] != null)
                        {
                            if (allDots[ix + 1, iy].tag == allDots[ix, iy].tag &&
                            allDots[ix + 2, iy].tag == allDots[ix, iy].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (iy < size.y - 2)
                    {
                        //check if the dots above exist
                        if (allDots[ix, iy + 1] != null && allDots[ix, iy + 2] != null)
                        {
                            if (allDots[ix, iy + 1].tag == allDots[ix, iy].tag &&
                            allDots[ix, iy + 1].tag == allDots[ix, iy].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

     public bool SwitchAndCheck(Vector2Int piecesPosition, Vector2Int direction){
        SwitchPieces(piecesPosition, direction);
        if(CheckForMatches()){
            SwitchPieces(piecesPosition, direction);
            return true;
        }
        SwitchPieces(piecesPosition, direction);
        return false;
    }

    private bool IsDeadlocked(){
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] != null)
                {
                    if (ix < size.x - 1)
                    {
                        if(SwitchAndCheck(new Vector2Int(ix, iy), Vector2Int.right)){
                            return false;
                        }
                    }
                    if (iy < size.y - 1)
                    {
                        if(SwitchAndCheck(new Vector2Int(ix, iy), Vector2Int.up)){
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard(){
        yield return new WaitForSeconds(0.5f);
        //create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                if (allDots[ix, iy] != null)
                {
                    newBoard.Add(allDots[ix, iy]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        // for every spot on the board
        for (int ix = 0; ix < size.x; ix++)
        {
            for (int iy = 0; iy < size.y; iy++)
            {
                //if this spot shouldn't be blank
                if(!blankSpaces[ix,iy]){
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    //Assign to the piece 
                    int maxIterations = 0;

                    while (MatchesAt(new Vector2Int(ix, iy), newBoard[pieceToUse]) && 
                        maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    //make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.dotPosition = new Vector2Int(ix,iy);
                    //fill in the dots array this new piece 
                    allDots[ix, iy] = newBoard[pieceToUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //check if it`s still deadlocked
        if(IsDeadlocked()){
            ShuffleBoard();
        }        
    }
}

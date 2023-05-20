using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    //board variables
    public Vector2Int dotPosition;
    public void DotPosition(int xPosition, int yPosition){
        dotPosition.x =xPosition;
        dotPosition.y =yPosition;
    }
    private Vector2Int dotPrevious;
    private Vector2Int TargetPosition;
    public bool isMatched = false;

    private HitsManager hitsManager;
    private Color matchedColor;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;

    private Vector2 fistTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 TempPosition;
    //swipe stuff
    public float swipeAngle = 0;
    private float swipeResist = 1;
    //powerUp stuff
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    [SerializeField] private GameObject AdjacentMarker;
    [SerializeField] private GameObject rowArrow;
    [SerializeField] private GameObject columnArrow;
    [SerializeField] private GameObject colorBomb;

    private float moveTime;
    private SpriteRenderer sr;
    private void Start() {
        //change 
        //need singleton
        sr = GetComponent<SpriteRenderer>();
        matchedColor = new Color(1, 1, 1, 0.2f);

        hitsManager = FindObjectOfType<HitsManager>();
        board = FindObjectOfType<Board>();
        findMatches=FindObjectOfType<FindMatches>();

        moveTime = 0.6f;

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
    }

    //testing
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) {
            /*
            isAdjacentBomb = true;
            GameObject marker = Instantiate(
                AdjacentMarker, transform.position, Quaternion.identity);
            marker.transform.SetParent(transform);
            */
        }
    }
    

    private void Update() {
        TargetPosition =dotPosition;
        //change later (copy code )
        //x
        if(Mathf.Abs(TargetPosition.x-transform.position.x) > 0.1 ){
            //move towards the target
            TempPosition = new Vector2(TargetPosition.x, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, TempPosition, moveTime);
            if(board.allDots[dotPosition.x, dotPosition.y] != this.gameObject){
                board.allDots[dotPosition.x, dotPosition.y] = this.gameObject;
            }
            findMatches.FindAllMatched();
        }else{
            //directly set the position
            TempPosition = new Vector2(TargetPosition.x, transform.position.y);
            transform.position = TempPosition;
        }
        //y
        if(Mathf.Abs(TargetPosition.y-transform.position.y)> 0.1 ){
            //move towards the target
            TempPosition = new Vector2(transform.position.x, TargetPosition.y);
            transform.position = Vector2.Lerp(transform.position, TempPosition, moveTime);
            if(board.allDots[dotPosition.x, dotPosition.y] != this.gameObject){
                board.allDots[dotPosition.x, dotPosition.y] = this.gameObject;
            }
            findMatches.FindAllMatched();
        }else{
            //directly set the position
            TempPosition = new Vector2(transform.position.x, TargetPosition.y);
            transform.position = TempPosition;
        } 
    }

    private IEnumerator CheckMoveCo(){
        if(isColorBomb){
            //this piece is color bomb, and the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }else if(otherDot.GetComponent<Dot>().isColorBomb){
            //the other piece is a color bomb, and this piece has the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if(otherDot != null){
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched){
                otherDot.GetComponent<Dot>().dotPosition = dotPosition;
                dotPosition = dotPrevious;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }else{
                board.DestroyMatches();
            }
        }
    }
    private void OnMouseDown()
    {
        //Destroy the hint
        if (hitsManager != null)
        {
            hitsManager.DestroyHint();
        }
        if (board.currentState == GameState.move)
        {
            fistTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngel();
    }
    private void CalculateAngel(){
        if (Mathf.Abs(finalTouchPosition.y - fistTouchPosition.y) > swipeResist ||
        Mathf.Abs(finalTouchPosition.x - fistTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(
                finalTouchPosition.y - fistTouchPosition.y,
                finalTouchPosition.x - fistTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }else{
            board.currentState = GameState.move;
        }
    }

     private void MovePiecesActual(Vector2Int direction){
        otherDot = board.allDots[dotPosition.x + direction.x, dotPosition.y+direction.y];
        dotPrevious = dotPosition;
        if (otherDot != null)
        {
            otherDot.GetComponent<Dot>().dotPosition += -direction;
            dotPosition += direction;
            StartCoroutine(CheckMoveCo());
        }else{
            board.currentState = GameState.move;
        }
    }

    private void MovePieces(){
        if(swipeAngle > -45 && swipeAngle <= 45 && dotPosition.x< board.size.x-1){
            //swipe right
            MovePiecesActual(Vector2Int.right);
        }else if(swipeAngle > 45 && swipeAngle <= 135 && dotPosition.y< board.size.y-1){
            //swipe up
            MovePiecesActual(Vector2Int.up);

        }else if((swipeAngle > 135 || swipeAngle <= -135) && dotPosition.x>0){
            //swipe left
            MovePiecesActual(Vector2Int.left);
        }else if(swipeAngle < -45 && swipeAngle >= -135 && dotPosition.y>0){
            //swipe down
             MovePiecesActual(Vector2Int.down);
        }else
        {
            board.currentState = GameState.move;
        }

    }
    private void FindMatches(){
        if(dotPosition.x>0 && dotPosition.x<board.size.x-1){
            GameObject leftDot1 = board.allDots[dotPosition.x-1, dotPosition.y];
            GameObject rightDot1 = board.allDots[dotPosition.x+1, dotPosition.y];

            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == gameObject.tag && rightDot1.tag == gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if(dotPosition.y>0 && dotPosition.y<board.size.y-1){
            GameObject downDot1 = board.allDots[dotPosition.x, dotPosition.y-1];
            GameObject upDot1 = board.allDots[dotPosition.x, dotPosition.y+1];
            if (downDot1 != null && upDot1 != null)
            {
                if (downDot1.tag == gameObject.tag && upDot1.tag == gameObject.tag)
                {
                    downDot1.GetComponent<Dot>().isMatched = true;
                    upDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
    public void MakeRowBomb(){
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.SetParent(transform);
    }
    public void MakeColumnBomb(){
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.SetParent(transform);
    }

    public void MakeAdjacentBomb(){
        isAdjacentBomb = true;
            GameObject marker = Instantiate(
                AdjacentMarker, transform.position, Quaternion.identity);
            marker.transform.SetParent(transform);
    }
    public void MakeColorBomb(){
        isColorBomb = true;
        GameObject marker = Instantiate(
            colorBomb, transform.position, Quaternion.identity);
            marker.transform.SetParent(transform);
        gameObject.tag = "Color";
    }
}

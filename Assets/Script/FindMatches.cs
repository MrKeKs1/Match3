using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches;
    private void Start() {
        currentMatches = new List<GameObject>();
        board = FindObjectOfType<Board>();

    }
    public void RemoveMatches(GameObject dot){
        currentMatches.Remove(dot);
    }
    public void FindAllMatched(){
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(new Vector2Int(
                dot1.dotPosition.x, dot1.dotPosition.y)));
        }
        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(new Vector2Int(
                dot2.dotPosition.x, dot2.dotPosition.y)));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(new Vector2Int(
                dot3.dotPosition.x, dot3.dotPosition.y)));
        }

        return currentDots;
    }
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.dotPosition.y));
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.dotPosition.y));
        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.dotPosition.y));
        }

        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.dotPosition.x));
        }
        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.dotPosition.x));
        }
        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.dotPosition.x));
        }

        return currentDots;
    }
    private void AddToListAndMatch(GameObject dot){
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }
    private  void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3){
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllMatchesCo(){
        yield return new WaitForSeconds(0.2f);
        for (int ix = 0; ix < board.size.x; ix++)
        {
            for (int iy = 0; iy < board.size.y; iy++)
            {
                GameObject currentDot = board.allDots[ix, iy];
                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    //x
                    if (ix > 0 && ix < board.size.x - 1)
                    {
                        GameObject leftDot = board.allDots[ix - 1, iy];
                        GameObject rightDot = board.allDots[ix + 1, iy];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();


                            if (leftDot != null && rightDot != null)
                            {
                                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                                {

                                    currentMatches.Union(IsRowBomb(
                                        leftDotDot, currentDotDot, rightDotDot));

                                    currentMatches.Union(IsColumnBomb(
                                        leftDotDot, currentDotDot, rightDotDot));

                                    currentMatches.Union(IsAdjacentBomb(
                                        leftDotDot, currentDotDot, rightDotDot));

                                    GetNearbyPieces(leftDot, currentDot, rightDot);

                                }
                            }
                        }
                    }
                    //y
                    if (iy > 0 && iy < board.size.y - 1)
                    {
                        GameObject UpDot = board.allDots[ix, iy + 1];
                        GameObject downDot = board.allDots[ix, iy - 1];
                        if (UpDot != null && downDot != null)
                        {
                            Dot upDotDot = UpDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            if (downDot != null && UpDot != null)
                            {
                                if (downDot.tag == currentDot.tag && UpDot.tag == currentDot.tag)
                                {
                                    currentMatches.Union(IsRowBomb(
                                        upDotDot, currentDotDot, downDotDot));

                                    currentMatches.Union(IsColumnBomb(
                                        upDotDot, currentDotDot, downDotDot));
                                    
                                    currentMatches.Union(IsAdjacentBomb(
                                        upDotDot, currentDotDot, downDotDot));

                                    GetNearbyPieces(UpDot, currentDot, downDot);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
    //change
    public void MatchPiecesOfColor(string color){
        for (int ix = 0; ix < board.size.x; ix++)
        {
            for (int iy = 0; iy < board.size.y; iy++)
            {
                //check if that piece exists
                if(board.allDots[ix,iy] != null){
                    //check the tag on that dot
                    if(board.allDots[ix,iy].tag==color){
                        //set that dot to be matched
                        board.allDots[ix, iy].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(Vector2Int piecesPosition){
        List<GameObject> dots = new List<GameObject>();
        for (int ix = piecesPosition.x-1; ix <= piecesPosition.x+1; ix++)
        {
            for (int iy = piecesPosition.y-1; iy <= piecesPosition.y+1; iy++)
            {
                //check if the piece is inside the board
                if(ix >=0 && ix < board.size.x && iy >=0 && iy < board.size.y){
                    if (board.allDots[ix, iy] != null)
                    {
                        dots.Add(board.allDots[ix, iy]);
                        board.allDots[ix, iy].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }
    List<GameObject> GetColumnPieces(int column){
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.size.y; i++)
        {
            if(board.allDots[column, i] !=null){
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if(dot.isRowBomb){
                    dots.Union(GetRowPieces(i)).ToList();
                }
                
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row){
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.size.x; i++)
        {
            if(board.allDots[i, row] !=null){
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if(dot.isColorBomb){
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs(){
        // did the player move something
        if(board.currentDot != null){
            //is the piece they moved matched
            if(board.currentDot.isMatched){
                //make it unmatched
                board.currentDot.isMatched = false;

                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                || (board.currentDot.swipeAngle < -135 && board.currentDot.swipeAngle >= 135)){
                    board.currentDot.MakeRowBomb();
                }else{
                    board.currentDot.MakeColumnBomb();
                }            
            }
            //is the other piece matcher
            else if(board.currentDot.otherDot != null){
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //check is the other dot matched
                if(otherDot.isMatched){
                    //make it unmatched
                    otherDot.isMatched = false;

                if((otherDot.swipeAngle > -45 && otherDot.swipeAngle <= 45)
                || (otherDot.swipeAngle < -135 && otherDot.swipeAngle >= 135)){
                    otherDot.MakeRowBomb();
                }else{
                    otherDot.MakeColumnBomb();
                }
                }
            }
        }
    }
}

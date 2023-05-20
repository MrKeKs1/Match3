using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsManager : MonoBehaviour
{
    private Board board;
    [SerializeField] private float hintDelay;
    //private float hintDelaySeconds;
    [SerializeField] private GameObject hintParticle;
    [SerializeField] private GameObject currentHint;

    private void Start() {
        board = FindObjectOfType<Board>();
        StartCoroutine(HintLife());
    }

    IEnumerator HintLife(){
        yield return new WaitForSeconds(hintDelay);
        if(currentHint == null){
            MarkHint();
        }
        yield return HintLife();
    }

    //First, I want to find all possible matches on the board
    List<GameObject> FindAllMatches(){
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int ix = 0; ix < board.size.x; ix++)
        {
            for (int iy = 0; iy < board.size.y; iy++)
            {
                if (board.allDots[ix, iy] != null)
                {
                    if (ix < board.size.x - 1)
                    {
                        if(board.SwitchAndCheck(new Vector2Int(ix, iy), Vector2Int.right)){
                            possibleMoves.Add(board.allDots[ix, iy]);
                        }
                    }
                    if (iy < board.size.y - 1)
                    {
                        if(board.SwitchAndCheck(new Vector2Int(ix, iy), Vector2Int.up)){
                            possibleMoves.Add(board.allDots[ix, iy]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    //Pick one of those matches randomly
    GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0){
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
            return null;        
    }
    //Create the hint behind the chosen match
    private void MarkHint(){
        GameObject move = PickOneRandomly();
        if(move != null){
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }
    //Destroy the hint.
    public void DestroyHint()
    {
        if(currentHint != null){
            Destroy(currentHint);
            currentHint = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private List<BlankGoal> levelGoals;
    [SerializeField] private List<GoalPanel> currentGoals;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goalIntroParent;
    [SerializeField] private GameObject goalGameParent;

    private void Start() {
        currentGoals = new List<GoalPanel>();
        SetupGoals();
    }
    private void SetupGoals(){
        for (int i = 0; i < levelGoals.Count; i++)
        {
            //Create a new Goal panel at the goalIntroParent position
            GameObject goal= Instantiate(goalPrefab, goalIntroParent.transform.position, 
                Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);

            //Set the image and text of the goal;
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString ="0/"+ levelGoals[i].numberNeeded;

            //Create a new Goal panel at the goalGameParent position
            GameObject gameGoal= Instantiate(goalPrefab, goalGameParent.transform.position, 
                Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            currentGoals.Add(panel);

            //Set the image and text of the gameGoal;
            GoalPanel gamePanel = gameGoal.GetComponent<GoalPanel>();
            gamePanel.thisSprite = levelGoals[i].goalSprite;
            gamePanel.thisString ="0/"+ levelGoals[i].numberNeeded;
        }
    }
    public void UpdateGoals(){
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Count; i++)
        {
            currentGoals[i].thisText.text =
                levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
                if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded){
                    goalsCompleted++;
                    currentGoals[i].thisText.text=
                        levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if(goalsCompleted >=levelGoals.Count){
            //you win
        }
    }
    public void CompareGoal(string goalToCompare){
        for (int i = 0; i < levelGoals.Count; i++)
        {
            if(goalToCompare == levelGoals[i].MatchValue){
                levelGoals[i].numberCollected++;
            }
        }
    }
}

[System.Serializable]
public class BlankGoal{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string MatchValue;
}


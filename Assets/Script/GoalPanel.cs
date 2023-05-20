using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalPanel : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public TextMeshProUGUI thisText;
    public string thisString;

    private void Start() {
        Setup();
    }
    private void Setup(){
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }
}

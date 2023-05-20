using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelControler : MonoBehaviour
{
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private Animator gameInfoAnimator;

    public void Ok(){
        if (panelAnimator != null && gameInfoAnimator != null)
        {
            panelAnimator.SetBool("Out", true);
            gameInfoAnimator.SetBool("Out", true);
        }
    }
}

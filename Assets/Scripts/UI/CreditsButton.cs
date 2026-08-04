using System;
using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    public GameObject credits;
    public void ShowCredits(){
        credits.SetActive(true);
    }
    public void HideCredits(){
        credits.SetActive(false);
    }
}

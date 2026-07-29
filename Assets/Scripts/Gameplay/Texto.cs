using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Texto
{
    [TextArea (2,6)]
    public string[] textArray;
    public Sprite[] imageArray;
}

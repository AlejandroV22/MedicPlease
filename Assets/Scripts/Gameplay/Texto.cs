using System;
using System.Data;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[System.Serializable]
public class Texto
{
    [TextArea (2,6)]
    public string[] textArray;
//    public Sprite[] imageArray;
// TODO hacer que vayan a distintos caminos dependiendo de la decision
    public void GetTextToFile(TextAsset csvFile){
        string[] lines = csvFile.text.Split('\n');
        for(int i=0; i<lines.Length;i++){
            var rows = lines[i].Split(',');
            textArray = rows;
        }
        
    }
}

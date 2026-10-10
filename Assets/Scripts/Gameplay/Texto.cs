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
    public string[] lines;
    public int lineIndex = 0;
//    public Sprite[] imageArray;
// TODO hacer que vayan a distintos caminos dependiendo de la decision
    public void GetTextToFile(TextAsset csvFile){
        lines = csvFile.text.Split('\n');
        var rows = lines[lineIndex].Split(';');
        textArray = rows;
     

        
    }
    public void ChangeLine(TextAsset csvFile){
        lineIndex ++;
        if(lineIndex>lines.Length){
            lineIndex = 0;
        }
        GetTextToFile(csvFile);

    }
}

using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public string playerName; 
    public string actualCase;
    public List<string> decisionsMade = new List<string>();
    public List<string> relevantDecisionMade = new List<string>();
}

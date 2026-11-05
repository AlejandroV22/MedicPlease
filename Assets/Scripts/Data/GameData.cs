using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Datos que se guardan en el archivo JSON.
/// Estructura: GameData -> CaseRecord (uno por caso) -> AttemptRecord (uno por intento) -> DecisionRecord.
/// </summary>
[System.Serializable]
public class GameData
{
    public string playerName;
    public string actualCase;
    public List<CaseRecord> caseHistory = new List<CaseRecord>();
}

/// <summary>Una decisión tomada dentro de un intento.</summary>
[System.Serializable]
public class DecisionRecord
{
    public string decision;
    public bool isRelevant;

    public DecisionRecord(string decision, bool isRelevant)
    {
        this.decision = decision;
        this.isRelevant = isRelevant;
    }
}

/// <summary>Un intento del jugador dentro de un caso.</summary>
[System.Serializable]
public class AttemptRecord
{
    public int attemptNumber;
    public bool isFinished;
    public bool isAbandoned; // el jugador salio del caso sin llegar a diagnosticar
    public bool diagnosedCorrectly;
    public List<DecisionRecord> decisions = new List<DecisionRecord>();

    public int RelevantDecisionsCount => decisions.Count(d => d.isRelevant);
}

/// <summary>Historial completo de un caso (todos sus intentos).</summary>
[System.Serializable]
public class CaseRecord
{
    public string caseId;
    public List<AttemptRecord> attempts = new List<AttemptRecord>();

    /// <summary>True si en algún intento obtuvo el diagnóstico correcto.</summary>
    public bool DiagnosedCorrectly => attempts.Any(a => a.isFinished && a.diagnosedCorrectly);

    public AttemptRecord LastAttempt => attempts.Count > 0 ? attempts[attempts.Count - 1] : null;
}

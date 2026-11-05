using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameDataController : MonoBehaviour
{
    private const string SaveFileName = "saveFile.json";
    private const string DefaultPlayerName = "Mitzune";
    private const string DefaultCase = "Situacion 1";

    private string saveFile;
    private GameData data;
    private AttemptRecord currentAttempt;

    /// <summary>Datos actuales en memoria. Se cargan una sola vez.</summary>
    public GameData Data => LoadData();

    /// <summary>Intento activo (null si no hay ninguno en curso).</summary>
    public AttemptRecord CurrentAttempt => currentAttempt;
    public bool isSaveFileExist = false;
    private void Awake()
    {
        LoadData();
    }

    // ───────────────────────── Carga / guardado ─────────────────────────

    /// <summary>
    /// Devuelve los datos en memoria. Solo lee el archivo la primera vez
    /// (o si forceReload es true), para no perder las referencias al intento activo.
    /// </summary>
    public GameData LoadData(bool forceReload = false)
    {
        if (data != null && !forceReload) return data;

        saveFile = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(saveFile))
        {
            data = JsonUtility.FromJson<GameData>(File.ReadAllText(saveFile));
            isSaveFileExist = true;
            // Los guardados antiguos pueden no traer el historial
            if (data.caseHistory == null) data.caseHistory = new List<CaseRecord>();
        }
        else
        {
            isSaveFileExist = false;
            data = new GameData
            {
                playerName = DefaultPlayerName,
                actualCase = DefaultCase
            };
        }

        currentAttempt = null;
        return data;
    }
    public void DeleteData(){
        SetPlayerName(DefaultPlayerName);
        SetActualCase(DefaultCase);
    }

    public void SaveData()
    {
        File.WriteAllText(saveFile, JsonUtility.ToJson(Data, true));
    }

    // ───────────────────────── Datos generales ─────────────────────────

    public void SetPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;
        Data.playerName = playerName;
        SaveData();
    }

    public void SetActualCase(string caseId)
    {
        if (string.IsNullOrEmpty(caseId)) return;
        Data.actualCase = caseId;
        SaveData();
    }

    // ───────────────────────── Historial de casos ─────────────────────────

    /// <summary>Busca el historial de un caso. Devuelve null si el jugador nunca lo ha jugado.</summary>
    public CaseRecord GetCaseRecord(string caseId)
    {
        return Data.caseHistory.FirstOrDefault(c => c.caseId == caseId);
    }

    /// <summary>
    /// Inicia un nuevo intento del caso. El guion se reinicia cada vez que se carga el caso,
    /// por lo que cualquier intento que haya quedado sin cerrar se marca como abandonado.
    /// </summary>
    public AttemptRecord StartAttempt(string caseId)
    {
        foreach (CaseRecord c in Data.caseHistory)
        {
            foreach (AttemptRecord a in c.attempts)
            {
                if (!a.isFinished)
                {
                    a.isFinished = true;
                    a.isAbandoned = true;
                }
            }
        }

        CaseRecord record = GetCaseRecord(caseId);
        if (record == null)
        {
            record = new CaseRecord { caseId = caseId };
            Data.caseHistory.Add(record);
        }

        currentAttempt = new AttemptRecord { attemptNumber = record.attempts.Count + 1 };
        record.attempts.Add(currentAttempt);
        Data.actualCase = caseId;

        SaveData();
        return currentAttempt;
    }

    /// <summary>Registra una decisión en el intento activo.</summary>
    public void RegisterDecision(string decision, bool isRelevant)
    {
        if (currentAttempt == null)
        {
            Debug.LogWarning("RegisterDecision: no hay un intento activo. Llama primero a StartAttempt().");
            return;
        }

        currentAttempt.decisions.Add(new DecisionRecord(decision, isRelevant));
        SaveData();
    }

    /// <summary>Cierra el intento activo indicando si el diagnóstico fue correcto.</summary>
    public void FinishAttempt(bool diagnosedCorrectly)
    {
        if (currentAttempt == null)
        {
            Debug.LogWarning("FinishAttempt: no hay un intento activo.");
            return;
        }

        currentAttempt.isFinished = true;
        currentAttempt.diagnosedCorrectly = diagnosedCorrectly;
        currentAttempt = null;
        SaveData();
    }

    // ───────────────────────── Consultas útiles ─────────────────────────

    public bool HasDiagnosedCorrectly(string caseId)
    {
        CaseRecord record = GetCaseRecord(caseId);
        return record != null && record.DiagnosedCorrectly;
    }

    public int GetAttemptsCount(string caseId)
    {
        CaseRecord record = GetCaseRecord(caseId);
        return record == null ? 0 : record.attempts.Count;
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controla el archivo de guardado. Es un singleton persistente:
/// solo existe una instancia durante toda la sesión, aunque haya cambios de escena.
/// </summary>
public class GameDataController : MonoBehaviour
{
    public static GameDataController Instance { get; private set; }

    private const string SaveFileName = "saveFile.json";
    private const string DefaultPlayerName = "Esternocleidomastoideo";
    private const string DefaultCase = "Sivesestohubounerrorenenlosguardadossorry";

    private GameData data;
    private AttemptRecord currentAttempt;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    /// <summary>Datos actuales en memoria. Se cargan una sola vez.</summary>
    public GameData Data => LoadData();

    /// <summary>Intento activo (null si no hay ninguno en curso).</summary>
    public AttemptRecord CurrentAttempt => currentAttempt;

    public bool isSaveFileExist = false;
    public bool isSaveFileDefault = false;

    private void Awake()
    {
        Debug.Log("GameDataController Awake id=" + GetEntityId() + " esInstancia=" + (Instance == this));
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("Archivo de guardado: " + SaveFilePath);
        LoadData();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ───────────────────────── Carga / guardado ─────────────────────────

    /// <summary>
    /// Devuelve los datos en memoria. Solo lee el archivo la primera vez
    /// (o si forceReload es true).
    /// </summary>
    public GameData LoadData(bool forceReload = false)
    {
        if (data != null && !forceReload) return data;

        if (File.Exists(SaveFilePath))
        {
            data = JsonUtility.FromJson<GameData>(File.ReadAllText(SaveFilePath));

            // Archivo vacío o corrupto: se usa una partida nueva
            if (data == null)
                data = CreateDefaultData();

            isSaveFileExist = true;
            isSaveFileDefault = false;

            // Los guardados antiguos pueden no traer el historial
            if (data.caseHistory == null)
                data.caseHistory = new List<CaseRecord>();
        }
        else
        {
            data = CreateDefaultData();
            isSaveFileExist = false;
            isSaveFileDefault = true;
        }

        currentAttempt = null;
        return data;
    }

    /// <summary>Borra el progreso y crea una partida nueva desde cero.</summary>
    public void NewGame()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("NewGame: archivo anterior eliminado.");
        }

        data = CreateDefaultData();
        currentAttempt = null;
        isSaveFileExist = false;
        isSaveFileDefault = true;

        SaveData();
        Debug.Log("NewGame ejecutado. Casos en memoria: " + data.caseHistory.Count);
        Debug.Log("NewGame: partida nueva creada. Casos: " + data.caseHistory.Count);
    }
    /// <summary>Inicia una partida nueva. Se puede llamar desde OnClick sin referencias del Inspector.</summary>
    public static void StartNewGame()
        {
            if (Instance == null)
            {
                Debug.LogError("StartNewGame: no hay GameDataController activo.");
                return;
            }

            Instance.NewGame();
        }

    /// <summary>Se mantiene por compatibilidad. Equivale a NewGame().</summary>
    public void DeleteData()
    {
        NewGame();
    }

    private GameData CreateDefaultData()
    {
        return new GameData
        {
            playerName = DefaultPlayerName,
            actualCase = DefaultCase,
            caseHistory = new List<CaseRecord>()
        };
    }

    public void SaveData()
    {
        if (data == null) LoadData();

        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(data, true));
        isSaveFileDefault = false;
        isSaveFileExist = true;
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
    /// Inicia un nuevo intento del caso. Cualquier intento que haya quedado sin cerrar
    /// se marca como abandonado.
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
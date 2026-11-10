using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Envía el contenido de saveFile.json a la plataforma web.
/// Uso: SaveUploader.Instance.Upload();  (por ejemplo, al terminar un caso o al salir)
/// </summary>
public class SaveUploader : MonoBehaviour
{
    public static SaveUploader Instance { get; private set; }

    [Header("Servidor")]
    [Tooltip("URL completa del endpoint, p. ej. http://192.168.1.50/api/saves/")]
    [SerializeField] private string endpointUrl = "http://localhost/api/saves/";

    [Tooltip("Debe ser igual a GAME_API_KEY del archivo .env del servidor")]
    [SerializeField] private string apiKey = Environment.GetEnvironmentVariable("GAME_API_KEY");

    [Header("Archivo")]
    [SerializeField] private string fileName = "saveFile.json";

    [SerializeField] private int timeoutSeconds = 10;

    private bool isUploading;

    private string SavePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Sube el save actual. Los callbacks son opcionales.</summary>
    public void Upload(Action onSuccess = null, Action<string> onError = null)
    {
        if (isUploading) return;
        StartCoroutine(UploadRoutine(onSuccess, onError));
    }

    private IEnumerator UploadRoutine(Action onSuccess, Action<string> onError)
    {
        if (!File.Exists(SavePath))
        {
            onError?.Invoke("No existe el archivo de guardado: " + SavePath);
            yield break;
        }

        isUploading = true;
        string json = File.ReadAllText(SavePath, Encoding.UTF8);

        using (var request = new UnityWebRequest(endpointUrl, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-Key", apiKey);
            request.timeout = timeoutSeconds;

            yield return request.SendWebRequest();

            isUploading = false;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[SaveUploader] Partida enviada: " + request.downloadHandler.text);
                onSuccess?.Invoke();
            }
            else
            {
                string msg = $"[SaveUploader] Error {request.responseCode}: {request.error} {request.downloadHandler.text}";
                Debug.LogWarning(msg);
                onError?.Invoke(msg);
            }
        }
    }
}

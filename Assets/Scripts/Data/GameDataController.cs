using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;

public class GameDataController : MonoBehaviour
{
    public string saveFile;
    public GameData defaultGameData;

    private void Awake(){
        saveFile = Application.dataPath + "/saveFile.json";
        Debug.Log("arriba los logs");
        defaultGameData.playerName = "Mitzune";
    }
   /* private void Update()
    {
        if(Keyboard.current.dKey.isPressed){
            SaveData("xd");
        }
        if(Keyboard.current.xKey.isPressed){
            LoadData();
        }    
    }
    */
    public GameData LoadData(GameData gameData){
        if(File.Exists(saveFile)){
            string fileContent = File.ReadAllText(saveFile);
            gameData = JsonUtility.FromJson<GameData>(fileContent);
            Debug.Log("Nombre del papu :v" + gameData.playerName);
            return gameData;
        }else{
            Debug.Log("Y el archivo de guardado papu");
            return gameData;
        }
    }

    public void SaveData(string playerNameReceived){
        GameData newData = new GameData(){
            playerName = playerNameReceived //remplazar por el valor a guardar por el objeto encargado
        };
        string jsonChain = JsonUtility.ToJson(newData);
        File.WriteAllText(saveFile, jsonChain);
        Debug.Log("Archivo creado talvez");
    }
}

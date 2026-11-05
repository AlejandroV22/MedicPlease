using UnityEngine;

public class nameButton : MonoBehaviour
{
    public GameObject dataObject;
    private string playerName;
    private GameDataController dataMan;
    const int MaxLenght=14;

    private void Awake()
    {
        dataObject = GameObject.FindGameObjectWithTag("DataObject");
        dataMan = dataObject.GetComponent<GameDataController>();
    }
    public void SetPlayerName(string playerName){
        if(playerName.Length > MaxLenght){
            playerName = playerName.Substring(0, MaxLenght);    
        }
        this.playerName = playerName;
    }
    public void SaveName(){
        dataMan.SetPlayerName(playerName);
    }
    
}

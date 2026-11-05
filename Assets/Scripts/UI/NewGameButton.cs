using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    public GameObject newGamePanel;
    public GameObject warningOverlay;
    public GameObject newNameOverlay;
    private GameObject dataObject;
    public GameDataController dataMan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataObject = GameObject.FindGameObjectWithTag("DataObject");
        dataMan = dataObject.GetComponent<GameDataController>();
    }
    public void NewGame(){
        newGamePanel.SetActive(true);
        if(dataMan.isSaveFileExist){
            warningOverlay.SetActive(true);
            newNameOverlay.SetActive(false);
        }else{
            newNameOverlay.SetActive(true);
            warningOverlay.SetActive(false);
        }
    } 
    // Update is called once per frame
    void Update()
    {
        
    }
}

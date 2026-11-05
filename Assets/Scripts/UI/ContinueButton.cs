using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    private GameObject dataObject;
    public GameDataController dataMan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataObject = GameObject.FindGameObjectWithTag("DataObject");
        dataMan = dataObject.GetComponent<GameDataController>();
        if(dataMan.isSaveFileExist){
            gameObject.GetComponent<Button>().interactable = true;
        }else{
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dataMan.isSaveFileExist){
            gameObject.GetComponent<Button>().interactable = true;
        }else{
            gameObject.GetComponent<Button>().interactable = false;
        } 
    }
}

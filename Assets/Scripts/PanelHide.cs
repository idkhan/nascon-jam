using UnityEngine;
using UnityEngine.EventSystems;

public class PanelHide : MonoBehaviour
{

    void Update(){
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
                this.gameObject.SetActive(false);
        }
    }
}

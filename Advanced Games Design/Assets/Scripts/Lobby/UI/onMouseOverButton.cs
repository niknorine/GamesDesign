using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class onMouseOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject InfoBar;
    public string infotxt;
    public TextMeshProUGUI text;
    
  
    private void Awake()
    {
        GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(255, 0, 50, 255);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = infotxt;
       GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(255, 255, 255, 255);
        InfoBar.GetComponentInChildren<Image>().color = new Color32(140, 30, 69, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(255,0,50,255);
        text.text = "";
        InfoBar.GetComponentInChildren<Image>().color = new Color32(140, 30, 69, 0);

    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperLinks : MonoBehaviour, IPointerClickHandler {

	Camera cam;
	TMP_Text tmp_text;

	void Start()
	{
		cam = FindObjectOfType<Camera>();
        tmp_text = GetComponent<TextMeshProUGUI>().linkedTextComponent;
	}

    public void OnPointerClick(PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp_text, Input.mousePosition, cam);
		
        if( linkIndex != -1 ) { // was a link clicked?
            TMP_LinkInfo linkInfo = tmp_text.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
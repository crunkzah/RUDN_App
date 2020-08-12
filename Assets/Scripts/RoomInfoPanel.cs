using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomInfoPanel : MonoBehaviour 
{
	public TextMeshProUGUI text_tmp;
	public TextTranslator text_translator;
	public TextTranslator roomName_translator;
	public ButtonLinkOpener linkButton;

	public void SetLink(string _link, string label)
	{
		linkButton.gameObject.SetActive(true);
		linkButton.link = _link;
		linkButton.tmp_label.SetText(label);
	}

	public void SetRoomName(TranslatableString roomName_text_translatable)
	{
		roomName_translator.translatable_string = roomName_text_translatable;
		roomName_translator.TranslateString();
	}

	public void SetRoomInfo(TranslatableString roomInfo_text_translatable)
	{
		text_translator.translatable_string = roomInfo_text_translatable;	
		text_translator.TranslateString();
		Debug.Log("SetRoomInfo");
	}

	void Start()
	{
		Hide();
		
	}

	public void Show()
	{
        // string text = current_text.strings[(int)TextTranslator.current_language];
        // text_tmp.SetText(text);
        text_translator.TranslateString();
        text_translator.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
        roomName_translator.TranslateString();
        roomName_translator.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
		this.gameObject.SetActive(true);
	}

	public void Hide()
	{
		this.gameObject.SetActive(false);
	}
}

using UnityEngine;
using System.Collections.Generic;
using TMPro;



public class TextTranslator : MonoBehaviour {

	public enum LANG {RU, ENG};
	public static LANG current_language = LANG.RU;
	public const string LangKey = "Language";
	
	static List<TextTranslator> all_instances = new List<TextTranslator>();

	public TranslatableString translatable_string;

	void Awake()
	{
		all_instances.Add(this);
		current_language = (LANG) PlayerPrefs.GetInt(LangKey, 0);
		
	}

	public  delegate void LanguageChangeDelegate();
	public  static event LanguageChangeDelegate OnLanguageChanged; 
	public GameObject RoutePopUp;

	void Start()
	{
		SetLanguage(current_language);
		if(RoutePopUp != null)
			RoutePopUp.GetComponent<RoutePopUpDisable>().DisableThis();
	}
	

	public static void SetLanguage(LANG new_language)
	{
		current_language = new_language;
		
		FloorRouteLabel.OnLanguageChanged();
		
		for(int i = 0; i < all_instances.Count; i++)
			all_instances[i].TranslateString();
	}

	public void TranslateString()
	{
		
		int lang_index = (int)current_language;
		if(translatable_string == null || translatable_string.strings == null || translatable_string.strings.Length < lang_index)
		{
			Debug.LogError("Translatable_string has problems on " + this.gameObject.name);
			return;
		}

		TextMeshProUGUI tmp_gui = GetComponent<TextMeshProUGUI>();
		if(tmp_gui != null)
		{
			tmp_gui.SetText(translatable_string.strings[lang_index]);
            tmp_gui.ForceMeshUpdate(true);
		}
		else
		{
			TextMeshPro tmp = GetComponent<TextMeshPro>();
			if(tmp != null)
			{
				tmp.SetText(translatable_string.strings[lang_index]);
				tmp.ForceMeshUpdate(true);
			}
		}



	}

	public void ChangeLanguage()
	{
		LANG new_language = GetNextLang();
		
		SetLanguage(new_language);
		if(OnLanguageChanged != null)
			OnLanguageChanged();
	}

	LANG GetNextLang()
	{
		LANG result;
		switch(current_language)
		{
			case LANG.RU:
				result = LANG.ENG;
				break;
			case LANG.ENG:
				result = LANG.RU;
				break;
			default:
				result = LANG.RU;
				break;
		}

		return result;
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt(LangKey, (int)current_language);
	}

	void OnApplicationPause()
	{
		PlayerPrefs.SetInt(LangKey, (int)current_language);
	}

}

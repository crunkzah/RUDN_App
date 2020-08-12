using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {

	public Sprite[] flag_icons;

	
	void Awake()
	{
		TextTranslator.OnLanguageChanged += ChangeLanguageIcon;
	}

	void Start()
	{
		ChangeLanguageIcon();
	}

	

	public void ChangeLanguageIcon()
	{
		
		int icon_index = (int)TextTranslator.current_language;

		GetComponent<Image>().sprite = flag_icons[icon_index];
	}
}

using UnityEngine;
using TMPro;

public class TitlePanel : MonoBehaviour
{
   static TitlePanel _instance;
    
   public static TitlePanel Singleton()
    {
        if(_instance == null)
        {
            _instance = FindObjectOfType<TitlePanel>();
        }

        return _instance;
    }


    TextMeshProUGUI title;
    const float topTitle_timeToBeVisible = 3.0F;
    const float fadeSpeed = 0.85F;
    const float topTitle_alphaDelay = 0.0F;
    public float title_timer;
    public float title_alpha;

    void Awake()
    {
        title = GetComponentInChildren<TextMeshProUGUI>();

    }

    private void Start()
    {
        title_timer = 8;
        Color col = title.color;
        col.a = 0;
        title.color = col;
    }


    public void ShowWinterModeTitle(bool isOn)
    {
        title_alpha = topTitle_alphaDelay;
        title_timer = 0f;


        string txt;
        if(TextTranslator.current_language == TextTranslator.LANG.ENG)
        {
            txt = string.Format("Winter mode is {0}", isOn ? "on" : "off");
        }
        else
        {
            txt = string.Format("Зимний режим {0}", isOn ? "включен" : "выключен");
        }



        title.SetText(txt);

        Color col = title.color;
        col.a = 0;
        title.color = col;

        Debug.Log("<color=yellow>ShowTitle(): </color>");
    }

    void ProcessTitle(float dt)
    {
        if (title_timer < topTitle_timeToBeVisible)
        {
            title_timer += dt;
            title_alpha += fadeSpeed * dt;

            Color col = title.color;
            col.a = title_alpha;
            title.color = col;
        }
        else
        {
            if (title_alpha > 0)
            { 
                title_alpha += -fadeSpeed * dt;

                Color col = title.color;


                col.a = title_alpha;
                title.color = col;
            }
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        ProcessTitle(dt);
    }

    
}

using System.Text;
using UnityEngine;
using TMPro;

public class FloorRouteLabel : MonoBehaviour
{
	
	static FloorRouteLabel instance;
	public static FloorRouteLabel Singleton()
	{
		if(instance == null)
		{
			instance = FindObjectOfType<FloorRouteLabel>();
		}
		
		return instance;
	}
	
	public TextMeshProUGUI label;
	
	
	int currentFloorStart = 0;
	int currentFloorEnd = 0;
	
	public static void OnLanguageChanged()
	{
		SetFloorRouteText(Singleton().currentFloorStart, Singleton().currentFloorEnd);
	}
	
	public static void SetFloorRouteText(int floorStart, int floorEnd)
	{
		if(floorStart * floorEnd == 0)
		{
			Singleton().gameObject.SetActive(false);
			return;
		}
		else
		{
			Singleton().gameObject.SetActive(true);
		}
		
		
		StringBuilder sb = new StringBuilder();
		
		Singleton().currentFloorStart = floorStart;
		Singleton().currentFloorEnd = floorEnd;
		
		switch(TextTranslator.current_language)
		{
			case(TextTranslator.LANG.ENG):
			{
				if(floorStart == floorEnd)
				{
					sb.Append("You are on the right floor");
				}
				else
				{
					if(floorStart < floorEnd)
					{
						sb.Append(string.Format("Go up to the floor {0}", floorEnd));
					}
					else
					{
						sb.Append(string.Format("Come down to the floor {0}", floorEnd));
					}
				}
				break;
			}
			case(TextTranslator.LANG.RU):
			{
				if(floorStart == floorEnd)
				{
					sb.Append("Вы уже на нужном этаже");
				}
				else
				{
					if(floorStart < floorEnd)
					{
						sb.Append(string.Format("Поднимитесь на {0} этаж", floorEnd));
					}
					else
					{
						sb.Append(string.Format("Спуститесь на {0} этаж", floorEnd));
					}
				}
				break;
			}
		}
		
		
		Singleton().label.SetText(sb.ToString());
	}
	
}

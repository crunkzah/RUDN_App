using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridLayoutGroupCustom : MonoBehaviour {

	GridLayoutGroup grid;

	void Awake()
	{
		grid = GetComponent<GridLayoutGroup>();
	}

	void OnEnable()
	{
		int spacingOffset = (int)grid.cellSize.x * grid.constraintCount / 2;
		grid.padding.left = Screen.width / 2 - spacingOffset;
	}
}

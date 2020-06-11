using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
	public static PlayerResources _ { get; set; }

	private void Awake()
	{
		_ = this;
	}

	public bool IsHonjinDebug;
}

using UnityEngine;
using System.Collections;

public class CameraSetup : MonoBehaviour 
{
	void Awake () 
	{
		Camera.main.aspect =  1334f / 750f;
	}
}

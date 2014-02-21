using System;
using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	public Action<string> b1;
	void Start () {
		b1 += a1;
		b1 += a2;
		b1("nio");
	}
	private static void a2(string name) {
		Debug.Log("a2 "+name);
	}
	private static void a1(string name) {
		Debug.Log("a1 "+name);
	}
}

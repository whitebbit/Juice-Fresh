using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeShop : MonoBehaviour
{
	public int CostIfRefill = 12;
	// Use this for initialization
	void OnEnable ()
	{
		transform.Find ("Image/BuyLife/Price").GetComponent<Text> ().text = "" + CostIfRefill;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YG;

public class Offlineleadboard : MonoBehaviour {
    Text label;
	// Use this for initialization
	void OnEnable () {
        label = transform.Find( "Slot" ).Find( "Score" ).GetComponent<Text>();
        label.text = "" +  YandexGame.savesData.LevelScore.GetValueOrDefault( "Score" + YandexGame.savesData.openLevel);
	}
	
	// Update is called once per frame
	void OnDisable () {
        label.text = "";
	}
}

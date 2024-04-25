using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using YG;

public class Level : MonoBehaviour {
    public int number;
    public Text label;
    public GameObject lockimage;

	// Use this for initialization
	void Start () {
        if(YandexGame.savesData.LevelScore.GetValueOrDefault( "Score" + (number-1) ) > 0  )
        {
            lockimage.gameObject.SetActive( false );
            label.text = "" + number;
        }

        int stars = YandexGame.savesData.LevelStars.GetValueOrDefault($"Level.{number:000}.StarsCount", 0 );

        if( stars > 0 )
        {
            for( int i = 1; i <= stars; i++ )
            {
                transform.Find( "Star" + i ).gameObject.SetActive( true );
            }

        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartLevel()
    {
//        InitScript.Instance.OnLevelClicked(number);

    }
}

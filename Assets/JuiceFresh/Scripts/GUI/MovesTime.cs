using UnityEngine;
using System.Collections;
using DefaultNamespace;
using UnityEngine.UI;

public class MovesTime : MonoBehaviour
{
    public Sprite[] sprites;

    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(WaitForLoading());
    }

    IEnumerator WaitForLoading()
    {
        yield return LevelManager.THIS;
        GetComponent<Text>().text = LevelManager.THIS.limitType == LIMIT.MOVES
            ? LangYgUtils.Translate("Ходы", "MOVES")
            : LangYgUtils.Translate("Время", "Time");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
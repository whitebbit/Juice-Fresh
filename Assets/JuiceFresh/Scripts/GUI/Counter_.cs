using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine.UI;
using YG;

public class Counter_ : MonoBehaviour
{
    public int ingrTrackNumber;
    Text txt;
    private float lastTime;
    bool alert;
    public int totalCount;
    TargetGUI parentGUI;

    // Use this for initialization
    void Start()
    {
        txt = GetComponent<Text>();
        if (transform.parent.GetComponent<TargetGUI>() != null)
            parentGUI = transform.parent.GetComponent<TargetGUI>();
    }

    private void OnEnable()
    {
        lastTime = 0;
        alert = false;
        if (name != "TargetScore" || LevelManager.THIS == null) return;
        if (LevelManager.THIS.target == Target.SCORE)
            txt.text = "" + LevelManager.THIS.GetScoresOfTargetStars();
    }

    // Update is called once per frame
    private void Update()
    {
        if (name == "Score")
        {
            txt.text = "" + LevelManager.Score;
        }

        if (name == "LabelKeepPlay")
        {
            if (LevelManager.THIS.limitType == LIMIT.MOVES)
                txt.text = LangYgUtils.Translate("получить + ", "GET + ") + LevelManager.THIS.ExtraFailedMoves +
                           LangYgUtils.Translate(" ходов", " moves");
            else
                txt.text = LangYgUtils.Translate("получить + ", "GET + ") + LevelManager.THIS.ExtraFailedSecs +
                           LangYgUtils.Translate(" сек.", " secs");
        }

        if (name == "BestScore")
        {
            txt.text = LangYgUtils.Translate("Лучший счет:", "Best score:") +
                       YandexGame.savesData.LevelScore.GetValueOrDefault("Score" + YandexGame.savesData.openLevel);
        }

        if (name == "Limit")
        {
            if (LevelManager.Instance.limitType == LIMIT.MOVES)
            {
                txt.text = "" + LevelManager.THIS.Limit;
                txt.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-460, -42, 0);
                //txt.transform.localScale = Vector3.one;
                if (LevelManager.THIS.Limit <= 5)
                {
                    txt.color = Color.red;
                    //txt.GetComponent<Outline>().effectColor = new Color(214 / 255, 0, 196 / 255);
                    if (!alert)
                    {
                        alert = true;
                        SoundBase.Instance.PlaySound(SoundBase.Instance.alert);
                    }
                }
                else
                {
                    alert = false;
                    txt.color = new Color(214f / 255f, 0, 196f / 255f);
                    //txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
                }
            }
            else
            {
                var minutes = Mathf.FloorToInt(LevelManager.THIS.Limit / 60F);
                var seconds = Mathf.FloorToInt(LevelManager.THIS.Limit - minutes * 60);
                txt.text = "" + $"{minutes:00}:{seconds:00}";
                txt.transform.localScale = Vector3.one * 0.35f;
                txt.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-445, -42, 0);
                if (LevelManager.THIS.Limit <= 30 && LevelManager.THIS.gameStatus == GameState.Playing)
                {
                    txt.color = Color.red;
                    //txt.GetComponent<Outline>().effectColor = Color.white;
                    if (lastTime + 30f < Time.time)
                    {
                        lastTime = Time.time;
                        SoundBase.Instance.PlaySound(SoundBase.Instance.timeOut);
                    }
                }
                else
                {
                    txt.color = new Color(214f / 255f, 0, 196f / 255f);
                    //txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
                }
            }
        }

        if (name == "TargetBlocks")
        {
            txt.text = "" + (totalCount - LevelManager.THIS.targetBlocks) + "/" + totalCount;
            if (LevelManager.THIS.targetBlocks == 0)
                parentGUI.Done();
        }

        if (name == "TargetCages")
        {
            txt.text = "" + (totalCount - LevelManager.THIS.TargetCages) + "/" + totalCount;
            if (LevelManager.THIS.TargetCages == 0)
                parentGUI.Done();
        }

        if (name == "TargetBombs")
        {
            txt.text = "" + (LevelManager.THIS.TargetBombs) + "/" + totalCount;
            if (LevelManager.THIS.TargetBombs >= totalCount)
                parentGUI.Done();
        }

        if (name == "CountIngr")
        {
            txt.text = "" + (totalCount - LevelManager.THIS.ingrTarget[ingrTrackNumber].count) + "/" + totalCount;
            if (LevelManager.THIS.ingrTarget[ingrTrackNumber].count == 0)
                parentGUI.Done();
        }

        if (name == "CountStar")
        {
            txt.text = "" + (LevelManager.THIS.stars) + "/" + (int)LevelManager.THIS.starsTargetCount;
            if (LevelManager.THIS.stars == (int)LevelManager.THIS.starsTargetCount)
                parentGUI.Done();
        }

        if (name == "CountIngrForMenu")
        {
            txt.text = "" + totalCount;
            if (LevelManager.THIS.target == Target.BOMBS)
            {
                txt.text = "" + LevelManager.THIS.bombsCollect; //1.4.5
            }
        }

        // if (name == "TargetIngr2") {
        // 	txt.text = "" + LevelManager.THIS.ingrCountTarget [1];
        // }
        if (name == "Lifes")
        {
            txt.text = "" + InitScript.Instance.GetLife();
        }

        if (name == "Gems")
        {
            txt.text = "" + InitScript.Gems;
        }

        if (name == "Level")
        {
            txt.text = LangYgUtils.Translate("Уровень ", "LEVEL ") + YandexGame.savesData.openLevel;
        }

        if (name == "TargetDescription1")
        {
            txt.text = LevelManager.THIS.target switch
            {
                Target.SCORE => LevelManager.THIS.targetDescriptions[6].ToString()
                    .Replace("%n", "" + LevelManager.THIS.GetScoresOfTargetStars())
                    .Replace("%s", "" + (int)LevelManager.THIS.starsTargetCount),
                Target.BLOCKS => LevelManager.THIS.targetDescriptions[1].ToString(),
                Target.COLLECT => LevelManager.THIS.targetDescriptions[2].ToString(),
                Target.ITEMS => LevelManager.THIS.targetDescriptions[3].ToString(),
                Target.CAGES => LevelManager.THIS.targetDescriptions[4].ToString(),
                Target.BOMBS => LevelManager.THIS.targetDescriptions[5].ToString(),
                _ => txt.text
            };
        }
    }
}
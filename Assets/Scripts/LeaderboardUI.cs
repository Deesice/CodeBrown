using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public struct Train
    {
        public int rank;
        public string name;
        public int score;
        public Train(int i, string n, int s)
        {
            rank = i;
            name = n;
            score = s;
        }
    }
    public List<Train> aroundUsers = new List<Train>();
    public List<Train> globalUsers = new List<Train>();
    public Sprite globalSprite;
    public Sprite aroundSprite;
    public Text[] userRaws;
    public Text[] scoreRaws;
    // Start is called before the first frame update
    void Start()
    {
        ClearRaws();
    }
    string GetMyName()
    {
        return "";
        //return SteamFriends.GetPersonaName();
    }
    public void Refresh()
    {
        string curName = GetMyName();
        int curRank = 0;
        foreach (var i in aroundUsers)
            if (i.name == curName)
                curRank = i.rank;

        if (curRank < 10)
            RefreshLikeGlobal();
        else
            RefreshLikeAround();
    }
    void RefreshLikeGlobal()
    {
        GetComponent<Image>().sprite = globalSprite;
        ClearRaws();
        userRaws[0].text = "Top 9";

        int count = 1;
        foreach (var i in globalUsers)
        {
            userRaws[count].text = i.rank + ". " + i.name;
            scoreRaws[count].text = i.score.ToString();
            count++;
        }
    }
    void RefreshLikeAround()
    {
        GetComponent<Image>().sprite = aroundSprite;
        ClearRaws();
        userRaws[0].text = "Top 3";

        int count = 1;
        foreach (var i in globalUsers)
        {
            userRaws[count].text = i.rank + ". " + i.name;
            scoreRaws[count].text = i.score.ToString();
            count++;
            if (count == 4)
                break;
        }

        count = 5;
        foreach (var i in aroundUsers)
        {
            int maxScore = 0;
            //maxScore = FindObjectOfType<LeaderBoard>().maxScore;
            userRaws[count].text = i.rank + ". " + i.name;
            if (i.name == GetMyName() && (GameManager.Instance.data.firstScore
                + GameManager.Instance.data.secondScore
                + GameManager.Instance.data.thirdScore) > maxScore)
                scoreRaws[count].text = GameManager.Instance.data.language == GameManager.Language.RU ? "ЧИТЕР" : Translator.Translate("CHEATER", GameManager.Instance.data.language);
            else
                scoreRaws[count].text = i.score.ToString();
            count++;
        }
    }

    void ClearRaws()
    {
        foreach (var i in userRaws)
            i.text = "";
        foreach (var i in scoreRaws)
            i.text = "";
    }
}

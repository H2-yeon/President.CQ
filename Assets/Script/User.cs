using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class User : MonoBehaviour
{
    public List<string> userCard = new List<string>();
    public abstract string Name { get; set; }
    public List<GameObject> cardObjList = new List<GameObject>();
    public GameManager gm;

    public abstract void Submit(string cardcode);
    public abstract void Pass();

    public abstract void SetName();
    public abstract void SpreadCard();
    public int rank = 3;

    public Sprite rank1;
    public Sprite rank2;
    public Sprite rank3;
    public Sprite rank4;

    public Image face;

    public void setFace() {

        switch (rank) {
            case 1: face.sprite = rank1; break;
            case 2: face.sprite = rank2; break;
            case 3: face.sprite = rank3; break;
            case 4: face.sprite = rank4; break;
        }
        return;
    }


    public bool submitCard(string lastValue, string cardcode)
    {
        //D03


        int subCard = int.Parse(lastValue.Substring(1, 1));
        int myCard = int.Parse(cardcode.Substring(1, 1));


        if (myCard > subCard)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void changeColor(string lastValue)
    {
        //첫번째 턴일때
        if (lastValue != "no")
        {
            for (int i = 0; i < cardObjList.Count; i++)
            {
                string temp = "";
                //낼 수 없는 카드면 어둡게
                try
                {
                    temp = cardObjList[i].GetComponent<Card>().CardCode.Substring(1, 2);

                } catch {
                    temp = "no";
                }



                if (gm.tempCard.Count == 0)
                {
                    if (gm.currentDirection)
                    {
                        if ((temp != "no" && temp.CompareTo(lastValue) > 0) || temp != "CR" || temp != "BK")
                        {
                            cardObjList[i].GetComponent<Image>().color = new Color(52f / 255f, 52f / 255f, 52f / 255f, 255f / 255f);

                        }
                    }
                    else
                    {
                        if ((temp != "no" && temp.CompareTo(lastValue) < 0) || temp != "CR" || temp != "BK")
                        {
                            cardObjList[i].GetComponent<Image>().color = new Color(52f / 255f, 52f / 255f, 52f / 255f, 255f / 255f);

                        }
                    }
                }
                else
                {
                    if (temp != "no" && temp.CompareTo(lastValue) != 0 && temp != "CR" && temp != "BK")
                    {
                        cardObjList[i].GetComponent<Image>().color = new Color(52f / 255f, 52f / 255f, 52f / 255f, 255f / 255f);

                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class User : MonoBehaviour
{
    public List<string> userCard = new List<string>();
    public abstract string Name { get; set; }
    public List<GameObject> cardObjList = new List<GameObject>();

    public abstract void Submit(string cardcode);
    public abstract void Pass();

    public abstract void SetName();
    public abstract void SpreadCard();


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
        if (lastValue == "no")
        {
            for (int i = 0; i < cardObjList.Count; i++)
            {
                Debug.Log("돌았다!");
                //색 바뀌는 테스트! 지워야함
                cardObjList[i].GetComponent<Image>().color = new Color(52 / 255f, 52 / 255f, 52 / 255f, 255 / 255f);
            }
        }
        //아닐때
        else
        {
            for (int i = 0; i < cardObjList.Count; i++)
            {
                //낼 수 없는 카드면 어둡게
                if (!submitCard(lastValue, cardObjList[i].GetComponent<Card>().CardCode))
                {
                    cardObjList[i].GetComponent<Image>().color = new Color(52 / 255f, 52 / 255f, 52 / 255f, 255 / 255f);

                }
            }
        }
    }
}

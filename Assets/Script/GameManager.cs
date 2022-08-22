using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    public List<User> userList;

    [SerializeField]
    List<string> attenderList;

    List<string> CardDeck = new List<string>();

    public List<string> submittedCard = new List<string>();
    public List<string> tempCard = new List<string>();

    public GameObject myDeck;
    public GameObject deck;
    public GameObject card;

    public RectTransform deckPoint;
    public RectTransform myDeckPoint;

    public Button start_button;
    public Button end_button;
    public Button submit_button;
    public Button pass_button;

    public Text timeText;
    public Text turnText;

    public PhotonView pv;
    public User user;
    public bool ControlSwitch = false;// if not my turn, do not controll the card
    public bool stopSwitch = false;// if value is true, timer will stop and pass the trun to other user

    public string currentTurnUser = "";
    public string currentTurnTime = "";
    public string currentDirection = "";
    public int cot = 0; //count of turn
    private void init()
    {
        int i = 1;
        foreach (string s in attenderList)
        {
            if (PhotonNetwork.NickName == s) //when start test on PhotonNetwork, it must change PhotonNetwork.Nickname 
            {
                userList[0].Name = s;
                userList[0].SetName();
            }
            else
            {
                userList[i].Name = s;
                userList[i].SetName();
                i++;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        attenderList = GameObject.Find("RoomManager").GetComponent<RoomManager>().attenderList;
        if (attenderList.Count != 4) {
            for (int i = attenderList.Count; i < 4; i++) {
                attenderList.Add("AI_"+i);
            }
        }
        user = GameObject.Find("Me").GetComponent<User>();
        init();


        start_button.onClick.AddListener(() =>
        {
            RoundStart();
        });
        end_button.onClick.AddListener(() =>
        {
            RoundEnd();
        });
        submit_button.onClick.AddListener(() =>
        {
            Submit();
        });
        pass_button.onClick.AddListener(() =>
        {
            Pass();
        });
        if (PhotonNetwork.MasterClient.NickName == PhotonNetwork.NickName) {
            initCardDeck();
            giveCardToUser();
            pv.RPC("RoundStart",RpcTarget.All);
        }

    }


    public void initCardDeck()
    {
        CardDeck.Clear();
        CardDeck.Add("D01");
        CardDeck.Add("D02");
        CardDeck.Add("D03");
        CardDeck.Add("D04");
        CardDeck.Add("D05");
        CardDeck.Add("D06");
        CardDeck.Add("D07");
        CardDeck.Add("D08");
        CardDeck.Add("D09");
        CardDeck.Add("D10");
        CardDeck.Add("D11");
        CardDeck.Add("D12");
        CardDeck.Add("D13");

        CardDeck.Add("H01");
        CardDeck.Add("H02");
        CardDeck.Add("H03");
        CardDeck.Add("H04");
        CardDeck.Add("H05");
        CardDeck.Add("H06");
        CardDeck.Add("H07");
        CardDeck.Add("H08");
        CardDeck.Add("H09");
        CardDeck.Add("H10");
        CardDeck.Add("H11");
        CardDeck.Add("H12");
        CardDeck.Add("H13");

        CardDeck.Add("C01");
        CardDeck.Add("C02");
        CardDeck.Add("C03");
        CardDeck.Add("C04");
        CardDeck.Add("C05");
        CardDeck.Add("C06");
        CardDeck.Add("C07");
        CardDeck.Add("C08");
        CardDeck.Add("C09");
        CardDeck.Add("C10");
        CardDeck.Add("C11");
        CardDeck.Add("C12");
        CardDeck.Add("C13");

        CardDeck.Add("S01");
        CardDeck.Add("S02");
        CardDeck.Add("S03");
        CardDeck.Add("S04");
        CardDeck.Add("S05");
        CardDeck.Add("S06");
        CardDeck.Add("S07");
        CardDeck.Add("S08");
        CardDeck.Add("S09");
        CardDeck.Add("S10");
        CardDeck.Add("S11");
        CardDeck.Add("S12");
        CardDeck.Add("S13");

        CardDeck.Add("JBK");
        CardDeck.Add("JCR");
    }
    public IEnumerator CountTime() {
        float time = 1500.0f;

        while (true) {
            time -= Time.deltaTime;
            if (time <= 0.0f || stopSwitch) {

                //TurnEnd();
                break;
            }

            timeText.text = $"{time:N0}";            
            yield return null;
        }
        stopSwitch = false;
        Pass();
    }



    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/



    public void SubmittedRPC(string cardcode) {
        pv.RPC("Submitted", RpcTarget.Others, cardcode);
    }
    //1. 카드를 내면 (turn인 대상이 카드가 isIndeck)
    //1.1 temp Card에 카드가 추가되고
    //1.2 낸카드 정렬(deck)

    //2. 제출 버튼 누르면
    
    //3. 제출 버튼을 누르지 않으면(pass or countdown)
    
    public void Temp(string cardcode)
    {
        Debug.Log(cardcode + "덱에 올림");
        tempCard.Add(cardcode); //1.1
        if (submittedCard.Count==0)
        {
            ArrangeCard1(deckPoint, tempCard.Count, cardcode);//1.2
        }
        else ArrangeCard1(deckPoint, submittedCard.Count, cardcode);//1.2
    }
    public void Submit()
    {
        //2. 제출 버튼 누르면
        //2.1 User 카드리스트의 낸 카드는 비워지고

        //2.2 tempCard로 SubmittedCard는 채워지고
        //2.3 tempCard는 비워지고
        //2.4 SubmittedCard에 있는 카드들을 deck의 child로 
        //2.5 nextTurn

        for(int i=0; i<tempCard.Count; i++)
        {
            user.Submit(tempCard[i]);
            SubmittedRPC(tempCard[i]);
        }//2.1
        //2.2, 2.4
        tempCard.Clear();//2.3

        stopSwitch = true;  //2.5
    }
    public void Pass()
    {
        //3. 제출 버튼을 누르지 않으면(pass or countdown)
        //3.1 이동된 카드(tempCard에 있던 카드들 다시 mydeck의 child로 원 위치)
        //3.2 tempCard는 비워지고
        //3.3 nextTurn

        Transform[] deckChildren = deck.GetComponentsInChildren<Transform>();
        foreach (Transform child in deckChildren)
        {
            if (child.name != deck.name )
            {
                if (tempCard.Contains(child.name))
                {
                    Destroy(child.gameObject);
                }
            }
        }//덱에 올라와 있는 카드 안낼것이니 지워버리고(temp의 담겨있는 카드들만 골라서 지우기)
        Transform[] myDeckChildren = myDeck.GetComponentsInChildren<Transform>();
        foreach (Transform child in myDeckChildren)
        {
            if (child.name != myDeck.name)
            {
                Destroy(child.gameObject);
            }
        }//나의덱에 있는 카드 먼저 싹 지워버리고
        user.SpreadCard();//가지고 있는 카드로 업데이트

        tempCard.Clear();//3.2
        

        stopSwitch = true;
    }
    [PunRPC]
    public void Submitted(string cardcode)
    {
        Debug.Log(cardcode + "카드가 제출됨");
        submittedCard.Add(cardcode);
        ArrangeCard1(deckPoint, submittedCard.Count, cardcode);

    }
    [PunRPC]
    public void ArrangeCard()
    {
        GameObject temp = Instantiate(card, deckPoint.position, Quaternion.identity); //재생성

        temp.gameObject.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(deckPoint.position.x +  (submittedCard.Count-1) * 30, deckPoint.position.y, 0), Quaternion.identity);
        temp.GetComponent<RectTransform>().SetParent(deck.GetComponent<RectTransform>());
        temp.name = submittedCard[submittedCard.Count-1];
        temp.GetComponent<Card>().CardCode = submittedCard[submittedCard.Count-1];
        temp.GetComponent<Card>().setCardImg();

    }
    [PunRPC]
    public void ArrangeCard1(RectTransform rect, int count, string cardcode )// 일단 실험삼아 만들어놓음 나중에 이름 바꿀게
    {
        GameObject temp = Instantiate(card, rect.position, Quaternion.identity); //재생성

        temp.gameObject.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(rect.position.x + (count - 1) * 30, rect.position.y, 0), Quaternion.identity);
        temp.GetComponent<RectTransform>().SetParent(deck.GetComponent<RectTransform>());
        temp.name = cardcode;
        temp.GetComponent<Card>().CardCode = cardcode;
        temp.GetComponent<Card>().setCardImg();
    }

    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/

    [PunRPC] //01
    public void RoundStart()
    {
        //새로운 라운드 시작
        user.SpreadCard(); // 나의 덱에 카드를 뿌리고
        /*if (userList[0].userCard.Contains("D01")) {
            pv.RPC("TurnStart", RpcTarget.All, PhotonNetwork.NickName);
        }*/
        if(PhotonNetwork.MasterClient.NickName == PhotonNetwork.NickName)
        {
            pv.RPC("TurnStart", RpcTarget.All, PhotonNetwork.NickName);
        }

    }

    [PunRPC] //02
    public void TurnStart(string userName)
    {
        if (userName == PhotonNetwork.NickName)
        {
            pv.RPC("setTurn", RpcTarget.All, PhotonNetwork.NickName);
            ControlSwitch = true;
            user.changeColor(submittedCard.Count == 0 ? "no" : submittedCard[submittedCard.Count - 1]);
        }
        else
        {
            ControlSwitch = false;
        }
    }

    [PunRPC] //03
    public void setTurn(string username)
    {
        //currentTurnUser = username;
        turnText.text = username;
        StartCoroutine(CountTime());
    }



    [PunRPC]
    public void TurnNext()
    {
        int index;
        index = userList.FindIndex(x => x.Name == currentTurnUser);
        TurnStart(userList[(index + 1) % 4].Name);
    }
    [PunRPC]
    public void TurnEnd(string userName)
    {
        if (userName == PhotonNetwork.NickName)
        {
            ControlSwitch = false;
            pv.RPC("setTurn", RpcTarget.All, PhotonNetwork.NickName);
            //need manage sequence of turn method
            StopCoroutine(CountTime());
            TurnNext();
        }
    }


    [PunRPC]
    public void RoundEnd()
    {
        submittedCard.Clear();// 제출된 카드 리스트 clear

        for (int i = 0; i < userList.Count; i++)
        {
            userList[i].userCard.Clear();
        }
        //모든 유저의 카드 리스트 clear

        Transform[] deckChildren = deck.GetComponentsInChildren<Transform>();
        foreach (Transform child in deckChildren)
        {
            if (child.name != deck.gameObject.name)
            {
                Destroy(child.gameObject);
            }
        }
        Transform[] myDeckChildren = myDeck.GetComponentsInChildren<Transform>();
        foreach (Transform child in myDeckChildren)
        {
            if (child.name != myDeck.gameObject.name)
            {
                Destroy(child.gameObject);
            }
        }
        //덱(나의 덱, 게임 덱)에올라와 있는 카드를 쓸어담아서 (오브젝트 파괴)
        initCardDeck();// 카드 덱에 주워담아 정리해주고
    }




    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/
    /*---------------------------------------------------------------------------------------*/

    [PunRPC]
    public void giveCard(string userName, string cardcode)
    {
        for (int i = 0; i < userList.Count; i++)
        {
            if (userList[i].Name == userName)
            {
                userList[i].userCard.Add(cardcode);
            }
        }
    }
    void giveCardToUser()
    {
        for (int i = 0; i < 14; i++)
        {
            int r = Random.Range(0, CardDeck.Count);
            //giveCard(userList[0].name, CardDeck[r]);
            pv.RPC("giveCard", RpcTarget.All, userList[0].Name, CardDeck[r]);
            CardDeck.RemoveAt(r);
        }
        for (int i = 0; i < 14; i++)
        {
            int r = Random.Range(0, CardDeck.Count);
            //giveCard(userList[1].name, CardDeck[r]);
            pv.RPC("giveCard", RpcTarget.All, userList[1].Name, CardDeck[r]);
            CardDeck.RemoveAt(r);
        }
        for (int i = 0; i < 13; i++)
        {
            int r = Random.Range(0, CardDeck.Count);
            //giveCard(userList[2].name, CardDeck[r]);
            pv.RPC("giveCard", RpcTarget.All, userList[2].Name, CardDeck[r]);
            CardDeck.RemoveAt(r);
        }
        for (int i = 0; i < 13; i++)
        {
            int r = Random.Range(0, CardDeck.Count);
            //giveCard(userList[3].name, CardDeck[r]);
            pv.RPC("giveCard", RpcTarget.All, userList[3].Name, CardDeck[r]);
            CardDeck.RemoveAt(r);
        }

    }



    // Update is called once per frame
    void Update()
    {

    }
}

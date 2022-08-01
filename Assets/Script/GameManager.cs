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

    public GameObject myDeck;
    public GameObject deck;
    public GameObject card;

    public RectTransform deckPoint;

    public Button start_button;
    public Button end_button;

    public PhotonView pv;

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
        init();
        foreach (User u in userList)
        {
            u.printCardList();
        }

        start_button.onClick.AddListener(() =>
        {
            RoundStart();
        });
        end_button.onClick.AddListener(() =>
        {
            RoundEnd();
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

        CardDeck.Add("JB");
        CardDeck.Add("JC");
    }
    public IEnumerator CountTime() {
        float time = 15.0f;

        while (true) {
            time -= Time.deltaTime;
            if (time <= 0.0f || stopSwitch) {
                //TurnEnd();
                break;
            }
            
            yield return null;
        }
        stopSwitch = false;
    }
    [PunRPC]
    public void Submitted(string cardcode)
    {
        Debug.Log(cardcode + "제출됨");
        submittedCard.Add(cardcode);
        ArrangeCard();

    }
    [PunRPC]
    public void ArrangeCard()
    {
        GameObject temp = Instantiate(card, deckPoint.position, Quaternion.identity); //재생성

        temp.gameObject.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(deckPoint.position.x +  (submittedCard.Count-1) * 30, deckPoint.position.y, 0), Quaternion.identity);
        temp.GetComponent<RectTransform>().SetParent(deck.GetComponent<RectTransform>());
        temp.name = submittedCard[submittedCard.Count-1];
        temp.GetComponentInChildren<Card>().CardCode = submittedCard[submittedCard.Count-1];
        temp.GetComponentInChildren<Card>().setCardImg();

    }
    [PunRPC]
    public void setTurn(string username) {
        currentTurnUser = username;
        StartCoroutine(CountTime());
    }

    [PunRPC]
    public void TurnStart(string userName) {
        if (userName == PhotonNetwork.NickName)
        {
            pv.RPC("setTurn", RpcTarget.All, PhotonNetwork.NickName);
            ControlSwitch = true;
        }
        else {
            ControlSwitch = false;
        }
    }
    [PunRPC]
    public void TurnEnd(string userName)
    {
        if (userName == PhotonNetwork.NickName)
        {
            ControlSwitch = false;
            pv.RPC("setTurn", RpcTarget.All, PhotonNetwork.NickName);
            //need manage sequence of turn method

        }
    }

    [PunRPC]
    public void RoundStart()
    {
        //새로운 라운드 시작
        userList[0].SpreadCard(); // 나의 덱에 카드를 뿌리고
        if (userList[0].userCard.Contains("D3")) {
            pv.RPC("TurnStart", RpcTarget.All, PhotonNetwork.NickName);
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

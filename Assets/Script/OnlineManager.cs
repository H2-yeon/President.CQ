using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    public Text onlineMonitoringText;
    private string gameVersion = "1";
    public DatabaseManager db;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        db = GameObject.Find("UserInfo").GetComponent<DatabaseManager>();

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("First Login");

        }
        else if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
            Debug.Log("Already Login");
        }


        onlineMonitoringText.text = "������ ������..";
    }
    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            //������ �ҷ�����
        }
        else
        {
            //db.GetUserInformationFromFireBase();
            onlineMonitoringText.text = "�¶��� : �����ͺ��̽��� ��� ��...";

        }
    }

    public override void OnConnectedToMaster()
    {
        onlineMonitoringText.text = onlineMonitoringText.text = "�¶��� : ȯ���մϴ�! " + db.getNickName() + "��!";

        PhotonNetwork.JoinLobby();//������ ���� ����� �κ�� ����
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // �� ���� ��ư�� ��Ȱ��ȭ


        onlineMonitoringText.text = "�������� : ������ ������ ������� ����\n ���� ��õ� ��...";

        //������ �������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinedLobby()//�κ� ����� �۵�
    {
        Debug.Log("Joined Lobby");
        //PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000).ToString("0000");
        PhotonNetwork.NickName = db.getNickName();
    }

    public void Connect()
    {

        if (PhotonNetwork.IsConnected)
        {
            //�� ���� ����
            onlineMonitoringText.text = "�¶��� : ä�ο� ���� ��..";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {

            //������ �������� ������ �õ�
            onlineMonitoringText.text = "�������� : ������ ������ ������� ����\n ���� ��õ���...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        onlineMonitoringText.text = "�¶��� : Ȱ��ȭ�� ä�� ����. ���ο� ä�� ����";
        Debug.Log("Creating Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 0 });

    }

    public override void OnJoinedRoom()
    {
        //���� ���� ǥ��
        onlineMonitoringText.text = "�¶��� : ä�� ���� ����";
        Debug.Log("Join Room");
        PhotonNetwork.LoadLevel("Scene_Field");
        //LoadingSceneController.Instance.LoadScene("Scene_Field");

    }
}

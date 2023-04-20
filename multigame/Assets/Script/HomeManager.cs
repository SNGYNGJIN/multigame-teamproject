using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);     // �ػ� ����
        PhotonNetwork.ConnectUsingSettings();     // �����Ʈ��ũ ���� ����
    }

    public override void OnCreatedRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(null, roomOptions, null);

        SceneManager.LoadScene("Room");
    }

    public override void OnJoinedRoom()
    {

    }

    public override void OnLeftRoom()
    {
        
    }
   
}

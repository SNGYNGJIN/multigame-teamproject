using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전
    private string userId; //유저 닉네임

    public TMP_InputField userIdText; //유저 입력 닉네임
    public TMP_InputField roomNameText; //방 이름
    public TMP_InputField roomPassword; //방 비밀번호


    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>(); //방 목록 
    public GameObject roomPrefab; //방 프리팹
    public Transform scrollContent; //방 표시 콘텐츠

    public GameObject[] roomPlayer; //방 접속 플레이어 리스트

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        //씬 자동싱크 설정
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 설정
        PhotonNetwork.GameVersion = gameVersion;

        //포톤서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }


    void Start()
    {
        Debug.Log("포톤 매니저 시작");
        userId = PlayerPrefs.GetString("User_ID", $"User_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
        Screen.SetResolution(1920, 1080, false);
    }

    //포톤 마스터 서버에 접속
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 서버 접속");

        PhotonNetwork.JoinLobby();
    }

    //로비에 접속
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속");

        //룸화면 비활성화
        GameObject.Find("View").transform.Find("RoomView").gameObject.SetActive(false);
        //로비화면 할성화
        GameObject.Find("View").transform.Find("LobbyView").gameObject.SetActive(true);
    }

    //방 생성
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성");
    }

    //방 접속
    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속");

        //로비화면 비할성화
        GameObject.Find("View").transform.Find("LobbyView").gameObject.SetActive(false);
        //룸화면 활성화
        GameObject.Find("View").transform.Find("RoomView").gameObject.SetActive(true);


        //룸 플레이어 업데이트
        PlayerUpdate();
    }

    //방 접속 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 접속실패");

        //returnCode 32758 = 비밀번호 틀림
        if (returnCode == 32758)
        {
            //다른 터치 방지벽 활성화
            GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(true);

            //비밀번호 오류창 활성화
            GameObject.Find("Panel-BackGround").transform.Find("Panel-PasswordError").gameObject.SetActive(true);
        }
        GameObject.Find("Panel-BackGround").transform.Find("Panel-Password").gameObject.SetActive(false);
    }

    //방 퇴장
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장");
    }

    //새로운 플레이어 방에 접속
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"플레이어 {newPlayer.NickName} 가 방에 참가.");

        //룸 플레이어 업데이트
        PlayerUpdate();
    }

    //플레이어 방에서 퇴장
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"플레이어 {otherPlayer.NickName} 가 방에서 퇴장.");

        //룸 플레이어 업데이트
        PlayerUpdate();
    }


    //방 목록 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach(var room in roomList)
        {
            //방이 제거됐을 경우
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                //방이 생성됐을 경우
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                //방이 변경됐을 경우
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }

    //create 버튼 클릭
    public void CreateButtonClick()
    { 
        //다른 터치 방지벽 활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(true);

        //방만들기 메뉴 설정 활성화
        GameObject.Find("Panel-BackGround").transform.Find("Panel-CreateRoom").gameObject.SetActive(true);

        //방 옵션 입력창 초기화
        GameObject.Find("InputField-roomName").gameObject.GetComponent<TMP_InputField>().text = "";
        GameObject.Find("InputField-password").gameObject.GetComponent<TMP_InputField>().text = "";
    }


    //방 옵션 설정후 방만들기 버튼 클릭
    public void OnMakeRoomClick()
    {
        string roomNamePassword = roomNameText.text + "_" + roomPassword.text;

        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 4;

        PhotonNetwork.CreateRoom(roomNamePassword, ro);

        //방만들기 메뉴 설정 비활성화
        GameObject.Find("Panel-CreateRoom").SetActive(false);

        //다른 터치 방지벽 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(false);
    }

    //방 옵션 설정창 나가기
    public void CreateRoomExitClick()
    {
        //방 옵션 설정창 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Panel-CreateRoom").gameObject.SetActive(false);

        //다른 터치 방지벽 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(false);
    }


    //방 참가 버튼 클릭
    public void JoinButtonClick()
    {
        string tempName=GameObject.Find("JoinRoomName").gameObject.GetComponent<TextMeshProUGUI>().text;
        string tempPassword = GameObject.Find("InputField-Password").gameObject.GetComponent<TMP_InputField>().text;
        string tempRoom = tempName + "_" + tempPassword;
        PhotonNetwork.JoinRoom(tempRoom);

        //비밀번호 입력창 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Panel-Password").gameObject.SetActive(false);

        //다른 터치 방지벽 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(false);
    }

    //비밀번호 입력창 나가기
    public void PasswordExitClick()
    {
        //비밀번호 입력창 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Panel-Password").gameObject.SetActive(false);

        //다른 터치 방지벽 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(false);
    }

    //비밀번호 오류창 나가기
    public void PasswordErrorExitClick()
    {
        //비밀번호 오류창 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Panel-PasswordError").gameObject.SetActive(false);

        //다른 터치 방지벽 비활성화
        GameObject.Find("Panel-BackGround").transform.Find("Blocker").gameObject.SetActive(false);
    }

    //방 퇴장 버튼 클릭
    public void ExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    //게임 시작 버튼 클릭
    public void GameStartClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            PhotonNetwork.LoadLevel("Game");

        }
    }


    //플레이어 리스트 업데이트
    public void PlayerUpdate()
    {
        //초기화 
        for(int i=0; i<4; i++)
        {
            roomPlayer[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = " ";
            roomPlayer[i].transform.GetChild(2).gameObject.SetActive(false);
        }

        //설정
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            //유저 닉네임 표시
            roomPlayer[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
            //유저 이미지 표시
            roomPlayer[i].transform.GetChild(2).gameObject.SetActive(true);
        }

        //방에 4명이면 게임시작 버튼 활성화
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            GameObject.Find("GameStartButton").gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("GameStartButton").gameObject.GetComponent<Button>().interactable = false;
        }

    }


    //닉네임 변경
    public void NickNameChange()
    {
        PhotonNetwork.NickName = userIdText.text;
    }

    //인게임 씬 호출
    private void OnLevelWasLoaded(int level)
    {

        Debug.Log("온레벨로드 호출");
        InstantiatePlayer();
    }


    //캐릭터 생성
    public void InstantiatePlayer()
    {

        Debug.Log("플레이어생성" + PhotonNetwork.PlayerList[0].NickName + " " + PhotonNetwork.LocalPlayer.NickName);
        if (PhotonNetwork.PlayerList[0].NickName == PhotonNetwork.NickName)
        {
            PhotonNetwork.Instantiate("Badger_Jasper", new Vector3(-5, 1, -5), Quaternion.identity, 0);
        }
        else if (PhotonNetwork.PlayerList[1].NickName == PhotonNetwork.NickName)
        {
            PhotonNetwork.Instantiate("Frog_Shanks", new Vector3(-2, 1, -5), Quaternion.identity, 0);
        }
        else if (PhotonNetwork.PlayerList[2].NickName == PhotonNetwork.NickName)
        {
            PhotonNetwork.Instantiate("Panda_Apple", new Vector3(2, 1, -5), Quaternion.identity, 0);
        }
        else if (PhotonNetwork.PlayerList[3].NickName == PhotonNetwork.NickName)
        {
            PhotonNetwork.Instantiate("Rabbit_Sydney", new Vector3(5, 1, -5), Quaternion.identity, 0);
        }
    }
}

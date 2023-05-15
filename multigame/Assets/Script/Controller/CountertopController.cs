using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountertopController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    GameObject hand;
    PlayerController playerController;

    bool isPlayerEnter;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hand = GameObject.FindGameObjectWithTag("hand");
        Managers.Input.KeyAction -= OnKeyboard;
        Managers.Input.KeyAction += OnKeyboard;

        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnKeyboard()
    {
        //물건 올리기
        if (Input.GetKey(KeyCode.C) && isPlayerEnter && playerController.isPicking)
        {
            GameObject go = hand.GetComponentInChildren<Rigidbody>().gameObject;
            ObjectController objectController = go.GetComponent<ObjectController>();
            // 플레이어 손에 있는 물건을 연결은 끈고(dorp())
            playerController.drop();
            //그 물건을 이 물건 위에 올린다.
            objectController.setParent(transform);

        }
        //조합하기
        if (Input.GetKey(KeyCode.D) && isPlayerEnter)
        {
            //조리법의 유무 확인
            //있으면 위에 있는 물건들을 전부 제거하고 조리법에 따라 물건을 던진다.
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (player == other.gameObject)
        {
            isPlayerEnter = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (player == other.gameObject)
        {
            isPlayerEnter = false;
        }
    }
}

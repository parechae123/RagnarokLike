using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SocketIOClient;
using JetBrains.Annotations;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using TMPro;

public class SocketIOManager : MonoBehaviour
{
    public SocketIOUnity socket;
    public Button SendBTN;
    public InputField SendField;
    public RectTransform chattingContent;
    public Text chattingWindow;
    [SerializeField] private UserInfo userInfo;
    public Queue<string> chattingQueue = new Queue<string>();
    public Queue<string> playerLoadWaiting = new Queue<string>();
    public Queue<string> playerLeaveWaiting = new Queue<string>();
    public Queue<UserPos> movingList = new Queue<UserPos>();
    private Dictionary<string, Transform> playerTRList = new Dictionary<string, Transform>();
    public float timer = 0;
    private void Start()
    {
        SetSocket("http://localhost:3000");
        SendBTN.onClick.AddListener(() =>
        {
            SendPressed();
        });

    }
    private void Update()
    {
        if (chattingQueue.Count > 0)
        {
            if (chattingWindow.text == string.Empty)
            {
                chattingWindow.text += chattingQueue.Dequeue();
            }
            else
            {
                chattingWindow.text += "\n" + chattingQueue.Dequeue();
            }
            GetChattingLine(chattingWindow.text, chattingContent, 3);
        }
        if (playerLoadWaiting.Count > 0 && userInfo.userServerID != string.Empty)
        {
            string tempName = playerLoadWaiting.Dequeue();
            if (tempName != userInfo.userServerID)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = tempName;
                playerTRList.Add(obj.name, obj.transform);
            }
            else
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                playerTRList.Add(userInfo.userServerID, obj.transform);
                playerTRList[userInfo.userServerID].gameObject.AddComponent<PlayerController>();

                obj.name = userInfo.userServerID;
            }
        }
        if (movingList.Count > 0)
        {
            UserPosUpdate(movingList.Dequeue());
        }
        if (playerLeaveWaiting.Count > 0)
        {
            Destroy(playerTRList[playerLeaveWaiting.Dequeue()].gameObject);
        }
        timer += Time.deltaTime;
        if (timer > 0.2f && playerTRList.Count > 0)
        {
            PlayerPosPacket(playerTRList[userInfo.userServerID].position);
            timer = 0;
        }
    }

    private void SendPressed()
    {
        socket.Emit("chat message", userInfo.userServerID + " : " + SendField.text);
        SendField.text = string.Empty;
    }
    private void SetSocket(string uri)
    {
        SocketIOUnity tempIO = new SocketIOUnity(uri);
        tempIO.OnConnected += (sender, e) =>
        {
            Debug.Log(tempIO.ServerUri + "에 연결됨");


        };
        socket = tempIO;
        socket.Connect();
        socket.On("chat message", (msg) =>
        {
            Debug.Log(msg.ToString());
            string tempString = ServerReflectedJson(msg.ToString());
            tempString = ServerReflectedJson(tempString);
            chattingQueue.Enqueue(tempString);
        });
        socket.On("connectUser", (ClientName) =>
        {
            Debug.Log(ClientName);
            string tempName = ServerReflectedJson(ClientName.ToString());
            tempName = ServerReflectedJson(tempName);
            /*            string tempString = JsonConvert.DeserializeObject<string>(ClientName.ToString());

                        Debug.Log(ClientName);*/
            playerLoadWaiting.Enqueue(tempName);


        });
        socket.On("setUserInfo", (userServerInfo) =>
        {
            string tempJsonSTR = userServerInfo.ToString();
            tempJsonSTR = ServerReflectedJson(tempJsonSTR);
            UserInfo tempInfo = JsonConvert.DeserializeObject<UserInfo>(tempJsonSTR);
            Debug.Log("이름줄게");
            Debug.Log(tempJsonSTR + "이름");

            Debug.Log("추가됨");


            userInfo = tempInfo;

        });
        socket.On("PlayerPosPacket", (plrPos) =>
        {
            string tempSTR = plrPos.ToString();
            tempSTR = ServerReflectedJson(tempSTR);
            Debug.Log(tempSTR);
            UserPos convertedPos = JsonConvert.DeserializeObject<UserPos>(tempSTR);
            if (convertedPos.userName == userInfo.userServerID)
            {
                return;
            }
            movingList.Enqueue(convertedPos);
            Debug.Log(convertedPos);
        });
        socket.On("logOutUserInfo", (userName) =>
        {
            string tempName = ServerReflectedJson(userName.ToString());
            tempName = ServerReflectedJson(tempName);
            chattingQueue.Enqueue(tempName + "님이 로그아웃했습니다.");
            playerLeaveWaiting.Enqueue(tempName);
        });

    }
    private void UserPosUpdate(UserPos tempPosInfo)
    {
        Vector3 sumedPos = new Vector3(tempPosInfo.xPos, tempPosInfo.yPos, tempPosInfo.zPos);
        playerTRList[tempPosInfo.userName].position = sumedPos;
    }

    private string ServerReflectedJson(string tempJsonSTR)
    {
        tempJsonSTR = tempJsonSTR.Remove(tempJsonSTR.Length - 1, 1);
        tempJsonSTR = tempJsonSTR.Remove(0, 1);
        return tempJsonSTR;
    }
    private short GetChattingLine(string targetSTR, RectTransform targetWindow = null, short targetCount = -1)
    {
        string tempSTR = targetSTR;
        short tempCount = 0;
        Debug.Log(targetWindow.rect.width);
        while (tempSTR.IndexOf("\n") != -1)
        {

            tempSTR = tempSTR.Remove(tempSTR.IndexOf("\n"), 2);
            tempCount++;
            Debug.Log("와일문 속 숫자는 현재 " + tempSTR.IndexOf("\n"));
            if (tempSTR.IndexOf("\n") == -1)
            {
                if (targetWindow != null && targetCount > -1)
                {
                    if (tempCount > targetCount - 1)
                    {
                        float beforeScaleYDelta = targetWindow.sizeDelta.y;
                        targetWindow.sizeDelta = new Vector2(targetWindow.sizeDelta.x, chattingWindow.preferredHeight);
                        Debug.Log(chattingWindow.preferredHeight + "프리페어리드 하이트");
                        if (targetWindow.anchoredPosition.y >= beforeScaleYDelta / 2)
                        {
                            targetWindow.anchoredPosition += new Vector2(0, (targetWindow.sizeDelta.y - beforeScaleYDelta) / 2);
                            Debug.Log(targetWindow.anchoredPosition.y + "컨텐츠 위치");
                        }
                    }
                }
                return tempCount;
            }
        }
        return -1;
    }
    public void PlayerPosPacket(Vector3 vec3)
    {
        UserPos userPos = new UserPos
        {
            xPos = vec3.x,
            yPos = vec3.y,
            zPos = vec3.z,
            userName = userInfo.userServerID
        };
        //플레이어 tr대입해줘야함
        socket.Emit("PlayerPosPacket", JsonConvert.SerializeObject(userPos));
    }
    private void OnApplicationQuit()
    {
        socket.Emit("RemoveUserInList", JsonConvert.SerializeObject(userInfo));
        socket.Disconnect();
        Debug.Log("연결 종료");
    }
}
[System.Serializable]
public class UserInfo
{
    public int userListIndex;
    public string userServerID;
}
[System.Serializable]
public class UserPos
{
    public float xPos;
    public float yPos;
    public float zPos;
    public string userName;
}
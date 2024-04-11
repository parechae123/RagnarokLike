using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class SocketTest : MonoBehaviour
{
    public SocketIOUnity socket;
    public string ServerURL = "http://localhost:3000";
    // Start is called before the first frame update
    void Start()
    {
        SetSocket(ServerURL);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            socket.Emit("InputSomeThing","ClaToServer");
            Debug.Log("흠");
        }
    }

    public void SetSocket(string URL)
    {
        SocketIOUnity tempIO = new SocketIOUnity(URL);
        tempIO.OnConnected += (sender, e) =>
        {
            Debug.Log(tempIO.ServerUri + "에 연결됨");
        };
        socket = tempIO;
        socket.Connect();
        socket.On("InputSomeThing", (msg) =>
        {
            Debug.Log("지금 내 클라에서 누름");
        });
    }
}

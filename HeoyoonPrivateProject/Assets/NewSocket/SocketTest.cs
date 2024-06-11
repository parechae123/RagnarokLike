using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class SocketTest : MonoBehaviour
{
    public SocketIOUnity socket;
    public string ServerURL = "http://localhost:3000";
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            socket.Emit("chat message", "ClaToServer");
            Debug.Log("Èì");
        }
    }

    void Start()
    {
        SetSocket(ServerURL);
    }

    public void SetSocket(string URL)
    {
        SocketIOUnity tempIO = new SocketIOUnity(URL);
        tempIO.OnConnected += (sender, e) =>
        {
            Debug.Log(tempIO.ServerUri + "¿¡ ¿¬°áµÊ");
        };
        socket = tempIO;
        socket.Connect();
        socket.On("chat message", (msg) =>
        {
            Debug.Log(msg);
        });
    }
}

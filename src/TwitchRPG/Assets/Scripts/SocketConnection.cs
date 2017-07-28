using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SimpleJSON;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];
    private LinkedList<TwitchBotRequest> waitingRequests = new LinkedList<TwitchBotRequest>();
    
    //TODO: make this a dictionairy have a list
    private Dictionary<string, IResponseProcessor> responders = new Dictionary<string, IResponseProcessor>();


    public string IP = "127.0.0.1"; //
    public int Port = 8124;

    bool keepSending = true;

    #region Singleton
    private static SocketConnection instance;
    public static SocketConnection Instance
    {
        get
        {
            return instance;
        }
        protected set
        {
            if (instance)
                throw new Exception("SocketConnection has already been assigned!");
            else
                instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private void OnEnable()
    {
        AddResponder(new LogResponse("test"));

        ConnectToServer();
        //GetAllPlayers();
        //StartCoroutine(SendBytes());

    }

    private void OnDisable()
    {
        keepSending = false;
        _clientSocket.Disconnect(false);

    }

    IEnumerator SendBytes()
    {
        byte[] chars = System.Text.Encoding.UTF8.GetBytes("Hello blah blah");
        while (keepSending)
        {
            SendData(chars);
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Disconnecting...");
    }

    public void AddResponder(IResponseProcessor responder)
    {
        if(!responders.ContainsKey(responder.TypeName))
            responders.Add(responder.TypeName, responder);
    }

    public void RemoveReponder(IResponseProcessor responder)
    {
        responders.Remove(responder.TypeName);
    }

    private void ConnectToServer()
    {
        try
        {
            _clientSocket.Connect(IP, Port);
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _clientSocket.EndReceive(AR);

        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        string stringdata = System.Text.Encoding.Default.GetString(recData);
        Debug.Log("Received: " + stringdata);

        try
        {
            JSONNode node = JSON.Parse(stringdata);
            if (node["type"].IsString && node["request"].IsBoolean && node["data"].IsObject)
            {
                JsonRequest request = new JsonRequest(node["type"], node["data"], node["request"]);

                ProcessResponse(request);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error parsing data to JSON: " + e);
        }

        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void ProcessResponse(JsonRequest request)
    {
        Debug.Log("Processing Response!");
        if (responders.ContainsKey(request.type))
        {
            //TODO: loop through all the responders on this command, when the list is implemented
            bool shouldContinue = responders[request.type].ProcessResponse(request);
            if (!shouldContinue)
                return;
        }

        //TODO: Check if the incomming message is a request or response before doing this
        if (waitingRequests.Count > 0)
        {
            LinkedListNode<TwitchBotRequest> lastWaiter = null;
            for (int i = 0; i < waitingRequests.Count; i++)
            {
                LinkedListNode<TwitchBotRequest> waiter;
                if (lastWaiter == null)
                    waiter = waitingRequests.First;
                else
                {
                    waiter = lastWaiter.Next;
                }

                if (waiter.Value.Request.type == request.type)
                {
                    waiter.Value.Response = request;
                    break;
                }

                lastWaiter = waiter;
            }
        }
    }

    private void SendData(byte[] data)
    {
        StartCoroutine(SendQueue(data));
    }
    public void GetAllPlayers()
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes("getallplayers");
        SendData(data);
    }
    IEnumerator SendQueue(byte[] data)
    {
        while (!_clientSocket.Connected)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Retrying Connection");
        }

        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
        Debug.Log("sent message: " + data);
    }

    public void SendRequest(TwitchBotRequest request)
    {
        waitingRequests.AddLast(request);
        SendData(request.Request.GetBytes());
    }
}
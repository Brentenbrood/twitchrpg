using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketConnection : MonoBehaviour
{
    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];

    public string IP = "127.0.0.1"; //
    public int Port = 8124;

    bool keepSending = true;

    private void Start()
    {
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
        Debug.Log(stringdata);
        JsonUtility.FromJson<PlayerData>(stringdata);

        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void SendData(byte[] data)
    {
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
    }
    public void GetAllPlayers()
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes("getallplayers");
        StartCoroutine(SendQueue(data));
    }
    IEnumerator SendQueue(byte[] data)
    {
        if (_clientSocket.Connected)
        {
            SendData(data);
            Debug.Log("sending message");
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SendQueue(data));
            Debug.Log("Retrying Connection");
        }

        
        
    }
}
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpClientManager
{
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private Thread receiveThread;
    private bool isRunning;
    private Logger logger;

    public event Action<string> OnMessageReceived;

    public TcpClientManager(string serverIp, int port)
    {
        logger = Logger.Instance;
        ConnectToServer(serverIp, port);
    }

    private void ConnectToServer(string serverIp, int port)
    {
        try
        {
            _tcpClient = new TcpClient(serverIp, port);
            _networkStream = _tcpClient.GetStream();
            isRunning = true;
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
            Debug.Log("서버에 연결되었습니다.");
        }
        catch (SocketException e)
        {
            Debug.LogError("서버 연결 실패: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("서버 연결 중 예외 발생: " + e.Message);
        }
    }

    public void SendMessageToServer(string message)
    {
        if (_networkStream == null)
        {
            Debug.LogError("_networkStream is null in SendMessageToServer");
            return;
        }

        try
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            _networkStream.Write(messageBytes, 0, messageBytes.Length);
            Debug.Log("보낸 메시지: " + message);
        }
        catch (SocketException e)
        {
            Debug.LogError("메시지 전송 중 소켓 예외 발생: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("메시지 전송 중 예외 발생: " + e.Message);
        }
    }

    private void ReceiveMessages()
    {
        int bytesRead;
        byte[] buffer = new byte[1024];
        try
        {
            while (isRunning && (bytesRead = _networkStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                logger.Log("받은 메시지: " + message);
                OnMessageReceived?.Invoke(message);
            }
        }
        catch (SocketException e)
        {
            if (isRunning)
            {
                Debug.LogError("서버 연결 종료: " + e.Message);
            }
        }
        catch (Exception e)
        {
            if (isRunning)
            {
                Debug.LogError("메시지 수신 중 예외 발생: " + e.Message);
            }
        }
    }

    public void Disconnect()
    {
        isRunning = false;
        receiveThread?.Join();
        _networkStream?.Close();
        _tcpClient?.Close();
    }
}

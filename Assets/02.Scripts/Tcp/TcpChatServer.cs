using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpChatServer : MonoBehaviour
{
    private TcpListener _server;
    private bool _isRunning;
    private Thread serverThread;

    [SerializeField]
    private string serverIp = "0.0.0.0"; // 모든 인터페이스에서 접근 가능하도록 설정
    [SerializeField]
    private int port = 8080;

    private void Start()
    {
        StartServer();
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }

    private void OnDisable()
    {
        StopServer();
    }

    private void OnDestroy()
    {
        StopServer();
    }

    public void StartServer()
    {
        if (serverIp == "0.0.0.0")
        {
            _server = new TcpListener(IPAddress.Any, port); // 모든 인터페이스에서 접근 가능
        }
        else
        {
            _server = new TcpListener(IPAddress.Parse(serverIp), port);
        }
        _server.Start();
        _isRunning = true;
        Debug.Log("서버가 시작되었습니다.");
        serverThread = new Thread(Run);
        serverThread.Start();
    }

    private void Run()
    {
        try
        {
            while (_isRunning)
            {
                var client = _server.AcceptTcpClient();
                Debug.Log("클라이언트가 연결되었습니다.");
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("소켓 예외 발생: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("예외 발생: " + e.Message);
        }
    }

    private void HandleClient(object obj)
    {
        var client = (TcpClient)obj;
        var stream = client.GetStream();
        var buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("받은 메시지: " + message);
                var response = Encoding.UTF8.GetBytes(message);
                stream.Write(response, 0, response.Length);
                Debug.Log("응답 메시지: " + message);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("소켓 예외 발생: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("예외 발생: " + e.Message);
        }
        finally
        {
            client.Close();
            Debug.Log("클라이언트 연결 종료");
        }
    }

    public void StopServer()
    {
        _isRunning = false;
        if (_server != null)
        {
            _server.Stop();
        }
        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Join();
        }
        Debug.Log("서버가 종료되었습니다.");
    }
}

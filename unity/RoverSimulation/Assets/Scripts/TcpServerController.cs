using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpServerController : MonoBehaviour
{
    private TcpListener listener;
    private Thread serverThread;
    private bool isRunning = false;
    private MovementController movementController;
    private string logFilePath;

    private ConcurrentQueue<string> incomingStrings = new ConcurrentQueue<string>();
    private ConcurrentQueue<string> outgoingStrings = new ConcurrentQueue<string>();

    void Start()
    {
        Application.runInBackground = true;

        movementController = GetComponent<MovementController>();
        logFilePath = Path.Combine(Application.dataPath, "../gateway_log.jsonl");
        StartServer();
    }

    private void StartServer()
    {
        serverThread = new Thread(ServerLoop);
        serverThread.IsBackground = true;
        isRunning = true;
        serverThread.Start();
        Debug.Log("[TCP] Szerver elindult a 5555-ös porton.");
    }

    private void ServerLoop()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5555);
        listener.Start();

        while (isRunning)
        {
            try
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(10);
                    continue;
                }

                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, new UTF8Encoding(false)))
                using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true })
                {
                    // 1. BIZTOSÍTÉK: Ürítjük a beragadt válaszokat, hogy tiszta lappal induljunk!
                    while (outgoingStrings.TryDequeue(out _)) { }

                    string jsonInput = reader.ReadLine();
                    if (string.IsNullOrEmpty(jsonInput)) continue;

                    incomingStrings.Enqueue(jsonInput);

                    string jsonOutput = null;
                    int waitTime = 0;

                    // 2. BIZTOSÍTÉK: Sose várjunk a végtelenségig a fõszálra! (Max 2 másodperc)
                    while (waitTime < 2000)
                    {
                        if (!isRunning) return;
                        if (outgoingStrings.TryDequeue(out jsonOutput)) break;
                        Thread.Sleep(10);
                        waitTime += 10;
                    }

                    if (string.IsNullOrEmpty(jsonOutput))
                    {
                        jsonOutput = "{\"status\":\"error\", \"error_message\":\"Szerver oldali idõtúllépés a Unityben!\"}";
                    }

                    writer.WriteLine(jsonOutput);
                    writer.Flush(); // 3. BIZTOSÍTÉK: Garantáltan kitoljuk az adatot a hálózatra!

                    // 4. BIZTOSÍTÉK: Szépen zárjuk le a TCP kapcsolatot
                    client.Client.Shutdown(SocketShutdown.Both);

                    File.AppendAllText(logFilePath, $"REQUEST: {jsonInput} \nRESPONSE: {jsonOutput}\n");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TCP] Kapcsolati hiba a háttérszálon: {ex.Message}");
            }
        }
    }

    void Update()
    {
        if (incomingStrings.TryDequeue(out string jsonInput))
        {
            string jsonOutput = "";
            try
            {
                CommandMessage cmd = JsonUtility.FromJson<CommandMessage>(jsonInput);
                if (cmd == null) throw new Exception("Hibás JSON parancs.");

                // Tartomány validálása
                if (cmd.action == "move" && (cmd.payload_x < -1 || cmd.payload_x > 1 || cmd.payload_y < -1 || cmd.payload_y > 1))
                {
                    throw new Exception("Payload értéke határokon kívül van.");
                }

                ResponseMessage response = new ResponseMessage
                {
                    request_id = cmd.request_id,
                    timestamp = DateTime.UtcNow.ToString("o"),
                    version = cmd.version,
                    status = "success",
                    position = transform.position
                };

                if (cmd.action == "move")
                {
                    if (movementController != null) movementController.ExecuteMove(cmd.payload_x, cmd.payload_y);
                }
                else if (cmd.action == "stop")
                {
                    if (movementController != null) movementController.StopMovement();
                }
                else if (cmd.action == "reset")
                {
                    if (movementController != null) movementController.ResetPosition();
                }

                jsonOutput = JsonUtility.ToJson(response);
            }
            catch (Exception ex)
            {
                ResponseMessage errResponse = new ResponseMessage
                {
                    request_id = "unknown",
                    status = "error",
                    error_message = ex.Message
                };
                jsonOutput = JsonUtility.ToJson(errResponse);
            }

            if (string.IsNullOrEmpty(jsonOutput))
            {
                jsonOutput = "{\"status\":\"error\", \"error_message\":\"Kritikus belsõ hiba\"}";
            }

            outgoingStrings.Enqueue(jsonOutput);
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        listener?.Stop();
        serverThread?.Abort();
    }
}
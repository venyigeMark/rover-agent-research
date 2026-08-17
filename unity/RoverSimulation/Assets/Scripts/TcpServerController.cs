using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class TcpServerController : MonoBehaviour
{
    [System.Serializable]
    public class RoverRequest { public string request_id; public string version; public string action; public float payload_value; }

    [System.Serializable]
    public class RoverResponse { public string request_id; public string status; public string error_code; public string rover_state; public Vector3 position; public BottomColorSensor.SensorState sensor; public LidarSensor.LidarData lidar; }

    public int port = 5556;
    private TcpListener listener;
    private bool isRunning = false;
    private MovementController movementController;
    private List<string> processedRequests = new List<string>();
    public BottomColorSensor colorSensor;
    public LidarSensor lidarSensor;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        if (movementController == null)
        {
            Debug.LogError("[TCP KRITIKUS HIBA] Nincs MovementController a Roveren! A szerver nem fog mûködni.");
        }

        Application.runInBackground = true;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        isRunning = true;
        if (colorSensor == null) colorSensor = FindAnyObjectByType<BottomColorSensor>();
        if (lidarSensor == null) lidarSensor = FindAnyObjectByType<LidarSensor>();
        Debug.Log($"[TCP] Szuperbiztos Szerver elindult a {port}-es porton.");
    }

    void Update()
    {
        if (!isRunning || listener == null || !listener.Pending()) return;

        TcpClient client = listener.AcceptTcpClient();
        client.ReceiveTimeout = 2000;
        client.SendTimeout = 2000;

        try
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            // Fontos javítás: UTF8Encoding(false) eltávolítja a BOM-ot, ami összezavarhatja a Pythont!
            using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true })
            {
                Debug.Log("[TCP] Kliens csatlakozott. Várakozás adatra...");
                string json = reader.ReadLine();
                Debug.Log($"[TCP] Kapott adat: '{json}'");

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning("[TCP] Üres adatot kaptunk, kapcsolat zárása.");
                    return;
                }

                RoverRequest req = null;
                try { req = JsonUtility.FromJson<RoverRequest>(json); }
                catch (System.Exception ex) { Debug.LogWarning("[TCP] JSON parse hiba: " + ex.Message); }

                string responseJson = "";

                if (req == null || string.IsNullOrEmpty(req.request_id) || string.IsNullOrEmpty(req.action))
                {
                    responseJson = CreateErrorJson(req != null ? req.request_id : "unknown", "ERR_INVALID_FORMAT");
                }
                else
                {
                    responseJson = ProcessCommand(req);
                }

                Debug.Log($"[TCP] Küldött válasz: '{responseJson}'");
                writer.WriteLine(responseJson);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[TCP] Fatális hiba a kapcsolat alatt: " + e.Message);
        }
        finally
        {
            client.Close();
            Debug.Log("[TCP] Kapcsolat lezárva.");
        }
    }

    string ProcessCommand(RoverRequest req)
    {
        if (movementController == null) return CreateErrorJson(req.request_id, "ERR_SYSTEM_FAIL");

        RoverResponse res = new RoverResponse
        {
            request_id = req.request_id,
            rover_state = movementController.currentState.ToString(),
            position = transform.position
        };

        // ÚJ SOR: Lekérjük a szenzor aktuális adatát, és betesszük a válaszba
        if (colorSensor != null)
        {
            res.sensor = colorSensor.GetCurrentSensorState();
        }

        if (lidarSensor != null)
        {
            res.lidar = lidarSensor.GetLidarData();
        }

        if (processedRequests.Contains(req.request_id))
        {
            res.status = "success"; res.error_code = "";
            return JsonUtility.ToJson(res);
        }

        processedRequests.Add(req.request_id);
        if (processedRequests.Count > 100) processedRequests.RemoveAt(0);

        res.status = "success"; res.error_code = "";

        switch (req.action.ToLower())
        {
            case "move":
                if (movementController.currentState != MovementController.RoverState.IDLE) { res.status = "busy"; res.error_code = "ERR_BUSY"; }
                else if (Mathf.Abs(req.payload_value) > movementController.maxDistance) { res.status = "error"; res.error_code = "ERR_OUT_OF_BOUNDS"; }
                else { movementController.ExecuteMove(req.payload_value); }
                break;
            case "turn":
                if (movementController.currentState != MovementController.RoverState.IDLE) { res.status = "busy"; res.error_code = "ERR_BUSY"; }
                else if (Mathf.Abs(req.payload_value) > movementController.maxAngle) { res.status = "error"; res.error_code = "ERR_OUT_OF_BOUNDS"; }
                else { movementController.ExecuteTurn(req.payload_value); }
                break;
            case "stop": movementController.StopMovement(); break;
            case "observe": case "get_status": movementController.PingWatchdog(); break;
            case "reset": movementController.ResetPosition(); break;
            default: res.status = "error"; res.error_code = "ERR_INVALID_FORMAT"; break;
        }

        res.rover_state = movementController.currentState.ToString();
        res.position = transform.position;
        return JsonUtility.ToJson(res);
    }

    string CreateErrorJson(string reqId, string errorCode)
    {
        string safeReqId = string.IsNullOrEmpty(reqId) ? "unknown" : reqId;
        return $"{{\"request_id\":\"{safeReqId}\",\"status\":\"error\",\"error_code\":\"{errorCode}\",\"rover_state\":\"ERROR\",\"position\":{{\"x\":0,\"y\":0,\"z\":0}}}}";
    }

    void OnApplicationQuit() { isRunning = false; if (listener != null) listener.Stop(); }
}
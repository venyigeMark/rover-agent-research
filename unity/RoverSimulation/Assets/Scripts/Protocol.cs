using System;
using UnityEngine;

// A bejövõ (Python -> Unity) üzenet szerkezete
[Serializable]
public class CommandMessage
{
    public string request_id;
    public string timestamp;
    public string version;
    public string action; // "observe", "move", "stop"
    public float payload_x; // Irány X (-1.0 és 1.0 között)
    public float payload_y; // Irány Y (-1.0 és 1.0 között)
}

// A kimenõ (Unity -> Python) üzenet szerkezete
[Serializable]
public class ResponseMessage
{
    public string request_id;
    public string timestamp;
    public string version;
    public string status; // "success" vagy "error"
    public string error_message;
    public Vector3 position;
}
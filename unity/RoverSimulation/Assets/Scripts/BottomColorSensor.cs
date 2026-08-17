using UnityEngine;
using System.Collections.Generic;

public class BottomColorSensor : MonoBehaviour
{
    [Header("Szenzor Konfiguráció")]
    public Transform sensorOrigin;
    public bool useThreeSensors = false;
    public float sensorOffset = 0.5f;

    [Header("Bizonytalanság Modellezése (Zaj, Kimaradás, Késés)")]
    public float noiseLevel = 0.05f;
    [Range(0f, 1f)]
    public float dropoutRate = 0.02f;
    public int latencyFrames = 3;

    [Range(0f, 1f)]
    public float whiteThreshold = 0.5f;
    public int noiseSeed = 42;

    private Queue<SensorState> latencyQueue = new Queue<SensorState>();
    private LineRenderer trackRenderer;

    [System.Serializable]
    public struct SensorData { public float intensity; public bool isWhite; }

    [System.Serializable]
    public struct SensorState { public SensorData left; public SensorData center; public SensorData right; }

    void Start()
    {
        Random.InitState(noiseSeed);
        trackRenderer = FindAnyObjectByType<LineRenderer>();
    }

    public SensorState GetCurrentSensorState()
    {
        SensorState state = new SensorState();
        Vector3 originPos = sensorOrigin != null ? sensorOrigin.position : transform.position;
        Vector3 rightDir = sensorOrigin != null ? sensorOrigin.right : transform.right;

        state.center = SimulateSensorHit(originPos);

        if (useThreeSensors)
        {
            Vector3 leftPos = originPos - rightDir * sensorOffset;
            Vector3 rightPos = originPos + rightDir * sensorOffset;
            state.left = SimulateSensorHit(leftPos);
            state.right = SimulateSensorHit(rightPos);
        }

        latencyQueue.Enqueue(state);
        if (latencyQueue.Count > latencyFrames) return latencyQueue.Dequeue();
        return state;
    }

    void Update()
    {
        GetCurrentSensorState();
    }

    private SensorData SimulateSensorHit(Vector3 origin)
    {
        SensorData data = new SensorData();

        if (Random.value < dropoutRate)
        {
            data.intensity = -1f;
            data.isWhite = false;
            Debug.DrawRay(origin, Vector3.down * 1f, Color.gray);
            return data;
        }

        float rawIntensity = 0f;

        // KIS MINTAVÉTELI FOLT ALAPÚ SZENZOR: Távolságmérés a vonaltól
        if (trackRenderer != null && trackRenderer.positionCount > 0)
        {
            float minDistance = float.MaxValue;
            // Megkeressük, milyen messze vagyunk a vonal legközelebbi pontjától
            for (int i = 0; i < trackRenderer.positionCount; i++)
            {
                Vector3 trackPoint = trackRenderer.GetPosition(i);
                float dist = Vector2.Distance(new Vector2(origin.x, origin.z), new Vector2(trackPoint.x, trackPoint.z));
                if (dist < minDistance) minDistance = dist;
            }

            // Ha közelebb vagyunk, mint a vonal vastagságának a fele, akkor rajta vagyunk!
            if (minDistance <= (trackRenderer.startWidth / 2f))
            {
                rawIntensity = 1.0f;
            }
        }

        float noise = Random.Range(-noiseLevel, noiseLevel);
        data.intensity = Mathf.Clamp01(rawIntensity + noise);
        data.isWhite = data.intensity >= whiteThreshold;

        Color debugColor = data.isWhite ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector3.down * 1f, debugColor);

        return data;
    }
}
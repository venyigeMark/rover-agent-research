using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ScenarioManager : MonoBehaviour
{
    [System.Serializable]
    public class TrackData { public float line_width; public float curvature_frequency; public float curvature_amplitude; }

    [System.Serializable]
    public class ObstaclesData { public int seed; public float spawn_rate; public int max_concurrent; }

    [System.Serializable]
    public class ScenarioData { public string scenario_name; public float time_scale; public string background_color; public TrackData track; public ObstaclesData obstacles; }

    [Header("Configuration")]
    public string scenarioFileName = "train_scenario.json";

    private LineRenderer lineRenderer;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private TrackData currentTrackData;
    private ObstaclesData currentObsData;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();

        LoadScenario();
    }

    void LoadScenario()
    {
        string path1 = Path.GetFullPath(Path.Combine(Application.dataPath, "../../scenarios", scenarioFileName));
        string path2 = Path.GetFullPath(Path.Combine(Application.dataPath, "../scenarios", scenarioFileName));

        string finalPath = "";
        if (File.Exists(path1)) finalPath = path1;
        else if (File.Exists(path2)) finalPath = path2;

        if (!string.IsNullOrEmpty(finalPath))
        {
            string json = File.ReadAllText(finalPath);
            ScenarioData data = JsonUtility.FromJson<ScenarioData>(json);
            ApplyScenario(data);
        }
        else
        {
            Debug.LogError($"[M06] KRITIKUS HIBA: Nem található a szcenárió fájl! Kerestem itt:\n1. {path1}\n2. {path2}");
        }
    }

    void ApplyScenario(ScenarioData data)
    {
        Debug.Log($"[M06] Szcenárió betöltése: {data.scenario_name}");
        currentTrackData = data.track;
        currentObsData = data.obstacles;

        // 1. Idõskála és Háttér
        Time.timeScale = data.time_scale;
        if (ColorUtility.TryParseHtmlString(data.background_color, out Color bgColor))
        {
            Camera.main.backgroundColor = bgColor;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        // 2. Pálya generálása
        GenerateTrack(currentTrackData);

        // 3. A REPRODUKÁLHATÓSÁG KULCSA: Beállítjuk a Random Seed-et a JSON-bõl!
        Random.InitState(currentObsData.seed);
        Debug.Log($"[M06] Random Seed beállítva: {currentObsData.seed}");

        // 4. Elindítjuk az akadályok gyártását (Idõzített rutin)
        StartCoroutine(SpawnObstaclesRoutine());
    }

    void GenerateTrack(TrackData trackConfig)
    {
        lineRenderer.startWidth = trackConfig.line_width;
        lineRenderer.endWidth = trackConfig.line_width;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;

        int segments = 300;
        lineRenderer.positionCount = segments;
        float baseRadius = 15f;

        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * 2 * Mathf.PI;
            float radius = baseRadius + Mathf.Sin(angle * trackConfig.curvature_frequency) * trackConfig.curvature_amplitude;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0.05f, z));
        }
    }

    IEnumerator SpawnObstaclesRoutine()
    {
        // Ez a ciklus a játék végéig fut, és folyamatosan adagolja az akadályokat
        while (true)
        {
            // Kitakarítjuk a listából azokat, amik már eltûntek
            activeObstacles.RemoveAll(item => item == null);

            // Csak akkor rakunk le újat, ha nem értük el a limitet
            if (activeObstacles.Count < currentObsData.max_concurrent)
            {
                SpawnObstacle();
            }

            // Várunk a következõ megjelenésig (JSON-bõl: spawn_rate)
            yield return new WaitForSeconds(currentObsData.spawn_rate);
        }
    }

    void SpawnObstacle()
    {
        // 1. Létrehozunk egy piros téglatestet (Cube)
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.GetComponent<Renderer>().material.color = Color.red;

        // 2. Véletlenszerû helyet keresünk neki pontosan a pálya vonalán!
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float baseRadius = 15f;
        float radius = baseRadius + Mathf.Sin(angle * currentTrackData.curvature_frequency) * currentTrackData.curvature_amplitude;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Letesszük az akadályt a földre (Y = 0.5, hogy ne süllyedjen el)
        obs.transform.position = new Vector3(x, 0.5f, z);

        // Kicsit véletlenszerû méret (0.5 és 2.0 között)
        float size = Random.Range(0.5f, 2.0f);
        obs.transform.localScale = new Vector3(size, size, size);

        // 3. Eltüntetés beállítása: Automatikusan megsemmisül egy véletlen idõ után (5-10 mp)
        float lifeTime = Random.Range(5f, 10f);
        Destroy(obs, lifeTime);

        activeObstacles.Add(obs);
    }
}
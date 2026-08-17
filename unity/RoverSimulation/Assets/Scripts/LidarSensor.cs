using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LidarSensor : MonoBehaviour
{
    [Header("LiDAR Konfiguráció")]
    public float fieldOfView = 180f; // Konfigurálható látómezõ[cite: 1]
    public int resolution = 30; // Felbontás (sugarak száma)[cite: 1]
    public float maxRange = 10f; // Maximális hatótáv[cite: 1]
    public float updateRateHz = 10f; // Ritkább frissítés (frekvencia)[cite: 1]

    [Header("Bizonytalanság (Noise & Dropout & Latency)")]
    public float noiseStdDev = 0.05f; // Távolsági zaj mértéke[cite: 1]
    [Range(0f, 1f)]
    public float dropoutProbability = 0.05f; // Méréskimaradás esélye[cite: 1]
    public int latencyFrames = 2; // ÚJ: Késés (Latency) modellezése
    private Queue<LidarData> latencyQueue = new Queue<LidarData>(); // ÚJ: Késleltetõ sor

    [Header("Szektorizálás")]
    public int numSectors = 5; // Szektorokra tömörítés[cite: 1]

    private float nextUpdateTime = 0f;

    [System.Serializable]
    public class LidarData
    {
        public float[] raw_distances; // Nyers sugárvektor[cite: 1]
        public bool[] validity_mask; // Érvényességi maszk[cite: 1]
        public float[] sector_min_distances; // Szektor minimum távolságok[cite: 1]
        public float[] sector_avg_distances; // Szektor átlag távolságok[cite: 1]
        public float max_range; // ÚJ: Maximális hatótáv explicit visszaadása
    }

    private LidarData lastData;

    void Start()
    {
        lastData = new LidarData
        {
            raw_distances = new float[resolution],
            validity_mask = new bool[resolution],
            sector_min_distances = new float[numSectors],
            sector_avg_distances = new float[numSectors]
        };
    }

    void Update()
    {
        // Ritkább frissítés modellezése[cite: 1]
        if (Time.time >= nextUpdateTime)
        {
            ScanEnvironment();
            nextUpdateTime = Time.time + (1f / updateRateHz);
        }
    }

    public LidarData GetLidarData()
    {
        return lastData;
    }

    private void ScanEnvironment()
    {
        float angleStep = fieldOfView / Mathf.Max(1, resolution - 1);
        float startAngle = -fieldOfView / 2f;

        List<float>[] sectorDistances = new List<float>[numSectors];
        for (int i = 0; i < numSectors; i++) sectorDistances[i] = new List<float>();

        for (int i = 0; i < resolution; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            // 1. Dropout modellezése[cite: 1]
            if (Random.value < dropoutProbability)
            {
                lastData.validity_mask[i] = false;
                lastData.raw_distances[i] = maxRange;
                Debug.DrawRay(transform.position, direction * maxRange, Color.gray);
                continue;
            }

            // 2. Raycast mérés[cite: 1]
            // 2. Raycast mérés
            lastData.validity_mask[i] = true;
            float measuredDistance = maxRange;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxRange))
            {
                string hitName = hit.collider.gameObject.name.ToLower();

                // Kiszûrjük a padlót ("plane", "floor") és magát a rovert. Minden más akadály!
                if (!hitName.Contains("plane") && !hitName.Contains("floor") && hit.collider.gameObject != this.gameObject)
                {
                    measuredDistance = hit.distance;
                    // Zaj hozzáadása
                    measuredDistance += Random.Range(-noiseStdDev, noiseStdDev);
                    measuredDistance = Mathf.Clamp(measuredDistance, 0f, maxRange);
                    Debug.DrawRay(transform.position, direction * measuredDistance, Color.red);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction * maxRange, Color.green);
                }
            }
            else
            {
                Debug.DrawRay(transform.position, direction * maxRange, Color.green);
            }

            lastData.raw_distances[i] = measuredDistance;

            // Szektor besorolás[cite: 1]
            int sectorIndex = Mathf.Clamp(Mathf.FloorToInt((float)i / resolution * numSectors), 0, numSectors - 1);
            sectorDistances[sectorIndex].Add(measuredDistance);
        }

        // 3. Szektor adatok kiszámítása (Min / Átlag)[cite: 1]
        for (int i = 0; i < numSectors; i++)
        {
            if (sectorDistances[i].Count > 0)
            {
                lastData.sector_min_distances[i] = sectorDistances[i].Min();
                lastData.sector_avg_distances[i] = sectorDistances[i].Average();
            }
            else
            {
                lastData.sector_min_distances[i] = maxRange;
                lastData.sector_avg_distances[i] = maxRange;
            }
        }

        lastData.max_range = maxRange; // Beállítjuk a max hatótávot[cite: 4]

        // Deep copy a késleltetéshez, hogy ne a referenciát írjuk felül
        LidarData queuedData = new LidarData
        {
            raw_distances = (float[])lastData.raw_distances.Clone(),
            validity_mask = (bool[])lastData.validity_mask.Clone(),
            sector_min_distances = (float[])lastData.sector_min_distances.Clone(),
            sector_avg_distances = (float[])lastData.sector_avg_distances.Clone(),
            max_range = lastData.max_range
        };

        latencyQueue.Enqueue(queuedData);
        if (latencyQueue.Count > latencyFrames)
        {
            lastData = latencyQueue.Dequeue();
        }
    }
}
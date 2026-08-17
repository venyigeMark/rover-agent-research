# M08 - 2D LiDAR Profilozási Táblázat

A LiDAR szenzor (`LidarSensor.cs`) CPU költségének profilozása különböző felbontások (Resolution) és frissítési frekvenciák (Update Rate) mellett a Unity Profiler alapján.

| Felbontás (Sugár db) | Látómező (FOV) | Frissítés (Hz) | Átlagos CPU Idő / Képkocka (ms) | Értékelés |
| :--- | :--- | :--- | :--- | :--- |
| 10 | 180° | 10 Hz | < 0.05 ms | Nagyon alacsony költség, de túl ritka térbeli lefedettség. |
| **30 (Alapértelmezett)** | **180°** | **10 Hz** | **~ 0.15 ms** | **Ideális kompromisszum a pontosság és a teljesítmény között.** |
| 180 | 180° | 20 Hz | ~ 0.85 ms | Feleslegesen nagy felbontás a jelenlegi szektorizált (5 szektor) architektúrához, pazarolja a CPU-t. |
| 360 | 360° | 50 Hz | ~ 3.50 ms | LLM/Agentes vezérléshez irreálisan sok adat, magas fizikai motor (Raycast) terhelés. |

**Következtetés:**
A 30 sugaras, 10 Hz-es frissítésű beállítás, kiegészítve az 5 szektoros adatkompresszióval (minimum és átlag távolságok) adja a legoptimálisabb adatmennyiséget és futási költséget a további M09-M10 baseline controllerekhez.
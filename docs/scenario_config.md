# M06 Szcenárió Konfigurációs Dokumentáció

A reprodukálható benchmark környezet (M06) JSON alapú konfigurációs fájlokat használ. 

## JSON Séma felépítése

* **scenario_name** (string): A tesztkörnyezet egyedi azonosítója.
* **time_scale** (float): Gyorsított futtatás paramétere (1.0 = normál idő, 2.0 = kétszeres sebesség). A Unity `Time.timeScale` paraméterét vezérli.
* **background_color** (string): A Unity kamera háttérszíne Hex formátumban (pl. `#1a1a1a`).
* **track**:
  * **line_width** (float): A fehér pálya-vonal vastagsága.
  * **curvature_frequency** (float): A pálya kanyargósságának sűrűsége (szinuszhullám frekvencia).
  * **curvature_amplitude** (float): A kanyarok kilengésének nagysága.
* **obstacles**:
  * **seed** (integer): A legfontosabb paraméter. Ez garantálja, hogy adott szcenárióban az akadályok mindig, másodpercre pontosan ugyanakkor és ugyanott jelennek meg.
  * **spawn_rate** (float): Két akadály megjelenése között eltelt idő másodpercben.
  * **max_concurrent** (int): A pályán egyszerre jelenlévő akadályok maximális száma.

  ## Headless (Grafika nélküli) és Gyorsított Futtatás
A rendszer támogatja a szimuláció CLI-ből történő, grafika nélküli indítását. A futtatandó szcenárió fájl a `--scenario` argumentummal adható át. A szimuláció sebességét a JSON fájl `time_scale` paramétere vezérli (pl. gyorsított teszteléshez).

**Példa headless indításra Windows alatt (lefordított bináris esetén):**
`RoverSimulation.exe -batchmode -nographics --scenario test_scenario.json`
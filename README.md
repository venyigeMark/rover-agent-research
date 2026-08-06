# Simulation-Blind Two-Timescale Rover Control

Ez a repozitórium egy AI-val támogatott, kétidőskálás rover vezérlési kutatás kódját és mérési eredményeit tartalmazza.

## Telepítési ellenőrzőlista (Környezetleírás)
A projekt klónozása után az alábbi lépésekkel állítható elő a reprodukálható munkakörnyezet:

- [ ] **Unity Hub és Editor:** Telepítsd a Unity Hubot, és győződj meg róla, hogy a `6000.5.2f1` verzió aktív.
- [ ] **Git:** Git kliens telepítve a verziókövetéshez.
- [ ] **IDE:** Visual Studio telepítve a C# szkriptekhez.
- [ ] **Python környezet:** `Python 3.10+` telepítve. 
- [ ] **Virtuális környezet aktiválása:** Futtasd a `source .venv/bin/activate` (Linux/Mac) vagy `.venv\Scripts\activate` (Windows) parancsot.

## Projektstruktúra
- `unity/`: Unity szimulációs környezet
- `gateway/`: TCP/JSON és agent-facing API adapterek
- `controllers/`: Hagyományos baseline és AI által generált vezérlők
- `scripts/`: Segédszkriptek (pl. rendszerellenőrzés)
- `prompts/`: Kutatási promptok
- `docs/`: Dokumentáció
- `experiments/`, `models/`, `training/`: Kísérleti adatok és ML modellek.

## Rendszerellenőrzés
A függőségek ellenőrzéséhez futtasd a doctor scriptet a projekt gyökeréből:
`python scripts/doctor.py`

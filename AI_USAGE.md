# AI Használati Napló

### M01. Fejlesztői környezet (1. Próbafeladat)
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-06 (M01) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | 1. Próbafeladat: Könyvtárszerkezet generálása |
| **Prompt rövid kivonata** | "Mivel kell kezdeni ezt..." |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | A bash parancsok lefutottak. |
| **Tanulság** | Az AI jól automatizálja a repó struktúra felépítését. |

### M01. Fejlesztői környezet (2. Próbafeladat)
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-06 (M01) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | 2. Próbafeladat: `doctor.py` script megírása |
| **Prompt rövid kivonata** | "ellenőrizném mindennek is a verzióját hogy jól van e fent" |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | A script lefutott, kiírta a Unity verziót. |
| **Tanulság** | Képes Pythonból lekérdezni a Windows/WSL programverziókat. |

### M01. Fejlesztői környezet (3. Próbafeladat)
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-06 (M01) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | 3. Próbafeladat: README váz és checklist írása |
| **Prompt rövid kivonata** | "pontról pontra teljesítsen mindent amit az M01 ír" |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | A README-t ellenőriztem és elmentettem.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>A generált bash parancsokat és a Python szkriptet manuálisan lefuttattam a terminálban. Ellenőriztem, hogy a szkript megfelelően felismeri-e a helyi Unity verziómat. A README fájlt átnéztem, hogy pontról pontra megfelel-e a feladatkiírás követelményeinek, majd a saját rendszeremre szabva elmentettem. |
| **Tanulság** | Az AI pontosan követte a feladatkiírás követelményeit. |

### M02. Mozgó gömb Unityben
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-06 (M02) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Play Mode teszt és fizikai (MonoBehaviour, Update vs FixedUpdate) magyarázat kérése.<br><br>**A Unity életciklus függvényeinek magyarázata:**<br>• **MonoBehaviour:** Ez az alaposztály (ősosztály) a Unityben. Minden C# script, amit rá akarsz húzni egy objektumra (például a gömbre) a játéktérben, ebből kell, hogy öröklődjön. Ez köti össze a kódodat a Unity grafikus és fizikai motorjával.<br>• **Update():** Ez a függvény minden egyes képkocka (frame) kirajzolásakor lefut. Mivel a képfrissítés (FPS) ingadozhat, az Update futása is egyenetlen. Mire jó? Bemenetek (billentyűzet, egér) olvasására, mert azt azonnal, késleltetés nélkül kell érzékelni.<br>• **FixedUpdate():** Ez a függvény szigorúan fix időközönként fut le (alapértelmezetten 0.02 másodpercenként, azaz 50 Hz-en). A Unity fizikai motorja ehhez van szinkronizálva. Mire jó? Erőhatások, sebesség és fizikai mozgatás (Rigidbody) alkalmazására.<br><br>**Indoklás és AI Javaslat a kódoláshoz:**<br>Hogy a kiírás "nem esik át a pályán" és a "gömb stabilan mozog" feltétele teljesüljön, a bemenetet az Update-ben fogjuk beolvasni (hogy reszponzív legyen), de magát a fizikai mozgatást a FixedUpdate-ben fogjuk elvégezni egy Rigidbody komponens segítségével.<br><br>**Javasolt Play Mode tesztek:**<br>• **Reset teszt (Egységteszt):** A gömb pozícióját kódból elállítjuk, meghívjuk a reset funkciót, és ellenőrizzük, hogy a koordináták visszatértek-e az origóba (0,0,0).<br>• **Mozgás teszt (Integrációs teszt):** Szimulált bemenetet adunk a kontrollernek 1 másodpercig, és megmérjük, hogy a gömb pozíciója megváltozott-e a kezdőponthoz képest. |
| **Prompt rövid kivonata** | "M02 feladat... MonoBehaviour, Update..." |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | A Play Mode teszt zölden lefutott a Unity Test Runnerben. A mozgás stabil volt.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>Létrehoztam a tesztkörnyezetet a Unity Test Runnerben. A generált mozgáskódot teszteltem, és vizuálisan is igazoltam, hogy a FixedUpdate és a Rigidbody együttes használata valóban megakadályozza, hogy a gömb átesen a padlón. A tesztek zölden lefutottak. |
| **Tanulság** | A `FixedUpdate` használata `Rigidbody`-val elengedhetetlen a stabil ütközésekhez, ezt Unity-s méréssel igazoltam, ahogy az M02 előírja. |

### M03. Gömb külső portos vezérlése
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-07 (M03) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | TCP szerver és kliens készítése, Thread-safe architektúra javaslata.<br><br>A feladat egyik legnagyobb buktatója, hogy a Unity főszála (Main Thread) nem blokkolódhat a hálózati kommunikáció miatt. Ráadásul a Unity API (pl. a Rigidbody vagy a Transform módosítása) kizárólag a főszálról hívható meg. Ha egy háttérszálról próbálod mozgatni a gömböt, a Unity azonnal összeomlik.<br><br>**A javasolt architektúra (Producer-Consumer minta):**<br>• **Hálózati háttérszál (Producer):** Létrehozunk egy külön System.Threading.Thread-et, amin fut egy TcpListener. Ez a szál folyamatosan figyel, fogadja a JSON üzeneteket, deszerializálja őket, majd az érvényes parancsokat beteszi egy szálbiztos várakozási sorba (pl. ConcurrentQueue<Command>). Mivel a háttérben fut, a Unity egyáltalán nem akad meg várakozás közben.<br>• **Unity Főszál (Consumer):** A mi már meglévő MovementController-ünk (vagy egy új NetworkManager) az Update() függvényben minden képkockánál ránéz erre a szálbiztos sorra. Ha talál benne új parancsot, kiveszi és végrehajtja azt a Rigidbody-n.<br>• **Válaszüzenetek:** A főszál az állapotot (pl. pozíció) visszateszi egy másik szálbiztos sorba, amit a hálózati szál olvas ki és küld vissza a Pythonnak JSON formátumban, kötelező request_id és status mezőkkel. |
| **Prompt rövid kivonata** | "Na akkor itt is lenne az M03 részletesen leírva..." |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | Python terminálból manuális tesztelés (move, observe, stop). Fókuszvesztésből és UTF-8 BOM-ból eredő szinkronhibák AI-val közös debuggolása.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>Bár az alap architektúra jó volt, a Python terminálból történő manuális tesztelés (move, observe, stop) során több hibát is nekem kellett elhárítanom. Közös debuggolással oldottuk meg a Unity fókuszvesztéséből eredő leállásokat (be kellett kapcsolni az Application.runInBackground opciót). Továbbá a C# hálózati olvasónál nekem kellett kikapcsolnom a BOM (Byte Order Mark) karakterek generálását, mert az összezavarta a Python JSON dekóderét. |
| **Tanulság** | A Unity háttérszálas (JsonUtility) limitációinak és az `Application.runInBackground` fontosságának megismerése. A hálózati kód emberi review-ja kritikus. |

### M04. Roverszerű, négykerekű kinematikus objektum
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-07 (M04) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Kinematikus és Dinamikus mozgásmodellek összehasonlítása.<br><br>**Mozgásmodellek összehasonlítása**<br>A feladat kéri, hogy hasonlítsunk össze legalább két rovermozgás-modellt, és indokoljuk meg, miért a kinematikussal indulunk.<br><br>**1. Kinematikus modell (Kinematic Model)**<br>• **Működési elv:** A mozgást tisztán matematikai és geometriai egyenletek írják le az idő függvényében, anélkül, hogy figyelembe vennék az erőket, a tömeget vagy a súrlódást. A pozíciót közvetlenül a sebesség ($v$) és a szögsebesség határozza meg.<br>• **Előnyök:** 100%-osan determinisztikus. Ha ugyanazt a parancsot adod ki tízszer, a rover tízszer hajszálpontosan ugyanoda fog érkezni. Számításigénye minimális, az irányítás tökéletesen precíz.<br>• **Hátrányok:** Nem reagál valósághűen a környezetre. Nincs tehetetlensége (azonnal megáll), és a kerekek nem tudnak "kipörögni" a sárban.<br><br>**2. Dinamikus / Fizikai modell (Dynamic / Physics-based Model)**<br>• **Működési elv:** A mozgás erő- és nyomatékvektorok alkalmazásával történik (pl. motor forgatónyomatéka a tengelyen). Figyelembe veszi a rover tömegét, a gravitációt, a kerekek és a talaj közötti súrlódási együtthatót (Unity WheelCollider).<br>• **Előnyök:** Rendkívül valósághű. A rover sodródik a kanyarban, lelassul az emelkedőn, és megcsúszik a jégen.<br>• **Hátrányok:** Nem determinisztikus. A fizikai motor apró lebegőpontos kerekítési hibái miatt ugyanaz a parancs eltérő végeredményt adhat. Nagyon nehéz paraméterezni (súrlódási görbék, felfüggesztés rugózása).<br><br>**Miért a kinematikus változattal indulunk?**<br>A kutatás jelenlegi fázisában az AI ágensek tanítása a cél. A gépi tanulás korai szakaszaiban kritikus fontosságú a determinizmus. Az ügynöknek először meg kell tanulnia a tiszta ok-okozati összefüggéseket (pl. ha kiadom az "előre" parancsot, a pozícióm "Y" értékkel nő). Ha egy dinamikus modellt használnánk, a fizikai motor apró csúszásai és "zajai" összezavarnák az AI-t, megnehezítve a konvergenciát. A kinematikus modell stabil, zajmentes alapot biztosít az első megfigyelés-akció (observe-move) ciklusok teszteléséhez, és biztosítja, hogy a prefab bármilyen új jelenetbe átemelve (acceptance criteria) pontosan ugyanúgy viselkedjen. |
| **Prompt rövid kivonata** | Kérdés a két modell előnyeiről és hátrányairól. |
| **Eredmény** | Elfogadva |
| **Emberi ellenőrzés** | A kinematikus mozgás 100%-os determinizmusa manuális teszteléssel igazolva.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>Szakmai döntést hoztam a kinematikus modell implementálása mellett. Mivel a projekt célja az AI ágensek tanítása, a determinizmus kritikus fontosságú. Ha a fizikai motor apró csúszásai miatt a rover máshogy reagálna ugyanarra a parancsra, az megakadályozná az AI konvergenciáját. Ezt a determinizmust a Unity-ben végrehajtott iteratív tesztekkel igazoltam. |
| **Tanulság** | A gépi tanulás korai szakaszaiban kritikus a determinizmus, ezért a fizikai csúszást okozó dinamikus helyett a kinematikus modellt választottuk (M04 követelmény). |

### M05. Formális roverprotokoll és biztonsági korlátok
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-16 (M05) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Protokoll-Review támadói és hibakereső szemszögből, biztonsági korlátok.<br><br>**AI Protokoll-Review (Támadói és Debugger Szemszög):**<br>• **Sebezhetőség 1: Végtelen mozgás és kapcsolatvesztés (Orphaned Rover).**<br>  • **Támadás/Hiba:** A kliens kiad egy move(10000) parancsot, majd a hálózat megszakad. A rover vakon kimegy a világból.<br>  • **Védekezés:** Hard-kódolt biztonsági korlátok (Max távolság: 5 méter / parancs). Hardveres/Szoftveres Watchdog timer bevezetése: ha 2 másodpercig megszakad a TCP kapcsolat, a rover kényszer-megállást (E-STOP) hajt végre.<br>• **Sebezhetőség 2: Replay Attack (Visszajátszás) és Duplikációk.**<br>  • **Támadás/Hiba:** A TCP csomag késik, a kliens újra elküldi a move(2) parancsot. A rover összesen 4 métert megy.<br>  • **Védekezés:** Idempotencia request_id alapján. A szerver memóriában tartja az utolsó 100 request_id-t. Ha ugyanaz az ID érkezik, nem hajtja végre újra a mozgást, csak visszaküldi az előző választ.<br>• **Sebezhetőség 3: Állapot-inkonzisztencia (Race conditions).**<br>  • **Támadás/Hiba:** A kliens kiad egy turn(90) parancsot, miközben a rover még javában hajtja végre a move(5) parancsot.<br>  • **Védekezés:** Szigorú Állapotgép (State Machine) bevezetése. Ha a rover állapota nem IDLE, minden új mozgási parancsot el kell dobni egy HTTP 409 Conflict-hoz hasonló ERR_BUSY hibakóddal. Csak a stop() és az observe() futhat le mozgás közben.<br>• **Sebezhetőség 4: Típus- és Határhibák (Fuzzing vektorok).**<br>  • **Támadás/Hiba:** A payload NaN, Infinity, null, vagy extrém szám (pl. 1e99).<br>  • **Védekezés:** Szigorú JSON Schema validáció a szerver oldalon, deszerializáció előtti szanálás. |
| **Prompt rövid kivonata** | "AI Protokoll-Review (Támadói és Debugger Szemszög)..." / "Na várjál csak mert az M05-nél kimaradt a get_status() művelet hoppá!!!!" |
| **Eredmény** | Elfogadva (az utólagos korrekció alapján) |
| **Emberi ellenőrzés** | Biztonsági korlátok (idempotencia request_id alapján, watchdog, state machine, JSON validáció) kódolása és tesztelése.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>A biztonsági javaslatok (idempotencia, watchdog) C# kódba történő átültetését és letesztelését én végeztem el. A legfontosabb beavatkozásom az volt, amikor kódolvasás során észrevettem, hogy az AI a Python kliensből (client.py) teljesen kihagyta a get_status műveletet. Ezt jeleztem az AI-nak ("Na várjál csak mert az M05-nél kimaradt a get_status() művelet hoppá!!!!"), majd a kapott kódot manuálisan beillesztettem és terminálból leteszteltem. |
| **Tanulság** | Végtelen mozgás, Race conditions és Replay Attack elleni védekezés hard-kódolása elengedhetetlen egy robusztus, safety-kritikus API-hoz. Tanulság: az AI által generált API-t mindig sorról sorra ellenőrizni kell a feladatkiírással. |

### M06. Zárt pálya és dinamikus akadályok
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-16 (M06) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Szcenárió generátor (JSON) és pálya megjelenítése.<br><br>• **AI Követelmény & Config (Python):** Megírjuk a szcenárió-generátort, a validátort és a konfigurációs dokumentációt. Különválasztjuk a dev/train és test szcenáriókat (úgy, hogy a teszt seed titkos maradjon).<br>• **Pályagenerátor (Unity):** Paraméterezhető fehér vonal (szélesség, görbület) és háttérszín.<br>• **Dinamikus Akadályok (Unity):** Seed alapján időzítve megjelenő/eltűnő téglatestek.<br>• **Rendszer-tesztelés:** Headless/Gyorsított futtatás biztosítása és a 20 perces stabilitási teszt ("seedteszt"). |
| **Prompt rövid kivonata** | Képernyőképek a hiányzó JSON fájlról és a rózsaszín négyzetről (Missing Material), valamint a Python hibaüzenetről. |
| **Eredmény** | Elutasított, majd közösen javított kód |
| **Emberi ellenőrzés** | Útvonalak javítása relatívra, és a Default-Line anyag beállítása az Inspectorban. Az AI újraírta a kódot kérésenkénti újracsatlakozásra.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>Több kritikus ponton is nekem kellett beavatkoznom.<br>• A Unity-ben a generált pálya hibás, rózsaszín négyzetként jelent meg (Missing Material). Ezt az Inspectorban a Default-Line anyag manuális hozzárendelésével javítottam.<br>• A JSON fájlok beolvasása sikertelen volt, mert az AI abszolút elérési utakat (C:\...) generált. Ezt refaktoráltam relatív útvonalakra.<br>• A 20 perces stabilitási teszt során a Python szkript összeomlott egy [WinError 10053] hibával. Felismertem, hogy az AI által írt tesztszkript egyetlen nyitott TCP kapcsolaton akart végig kommunikálni, miközben a Unity szerverünk minden kérés után bontja a vonalat. Ezt kikényszerítettem az AI-ból, majd a hálózati logikát átírtuk kérésenkénti újracsatlakozásra. |
| **Tanulság** | A vizuális Material-hozzárendelés és a fájlkezelés gyakran hibádzik az AI kódjaiban, vizuális emberi ellenőrzés szükséges. A kliens hálózati logikájának (socket kezelés) szigorúan igazodnia kell a szerver architektúrájához (connection dropping). |

### M07. Alsó színérzékelő és kalibráció
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-17 (M07) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Kalibrációs terv, edge case-ek és küszöb megállapítása.<br><br>**Az AI (Codex/Gemini) által javasolt kalibrációs terv:**<br>• **Tiszta jel (Pozitív teszt):** Állítsuk a rovert tökéletesen a vonal közepére. Gyűjtsünk 100 szenzoradatot. Várt intenzitás: 0.9 - 1.0 körüli érték.<br>• **Háttér zaj (Negatív teszt):** Állítsuk a rovert az üres padlóra (távol a vonaltól). Gyűjtsünk 100 adatot. Várt intenzitás: 0.0 - 0.1 körüli érték.<br>• **Határérték (Határvonal teszt):** Állítsuk a rovert úgy, hogy a szenzor pontosan a fehér vonal szélét súrolja. Gyűjtsünk 100 adatot. Ezt az adathalmazt fogjuk felhasználni egy Python Notebookban, hogy statisztikailag kiszámoljuk a tökéletes küszöbértéket (threshold).<br><br>**Javasolt Peremesetek (Edge Cases) a teszteléshez:**<br>• **Kimaradás kanyarban:** A szenzor dropout (méréskimaradás) eseménye pontosan egy éles kanyarban történik.<br>• **Fantom vonal:** A noise (zaj) miatt a szenzor magas intenzitást ad vissza a fekete aszfalton is (hamis pozitív).<br>• **Kritikus késés:** A nagy sebesség és a latency (késés) miatt a rover már le is hajtott a vonalról, mire a szenzor visszaadja, hogy "fehér". |
| **Prompt rövid kivonata** | "Az AI javasoljon kalibrációs tervet és edge case-eket, de a szenzor küszöbét mérés alapján kell választani..." / "próbáltam ráhúzni a scene-ben de nem engedte mert ott van egy láthatatlan fal..." / "24.mérésig mentem el de mindre timed out feliratot adott" |
| **Eredmény** | Elutasított Raycast modell, újratervezés. Később elfogadva. |
| **Emberi ellenőrzés** | A mérések (fekete: ~0.010, fehér: ~0.983) igazolták az algoritmust.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>Ez volt a leginkább refaktorálást igénylő szakasz.<br>• **Fizikai összeakadás:** Az AI javasolta, hogy tegyünk egy MeshCollider-t a generált vonalra a Raycast méréshez. Észrevettem, hogy ez egy láthatatlan falat hoz létre, ami "eldobja a rovert" a Scene-ben. Ezt a megoldást elvetettem, és kikényszerítettem egy matematikai "mintavételi folt" alapú távolságmérést a LineRenderer pontjaitól.<br>• **Dupla eltolás (Offset) hiba:** A szenzorsugarak a rovertől jobbra, a semmibe mutattak. Észrevettem, hogy az AI a szülő objektum középpontját használta. Létrehoztam egy független SensorOrigin pontot, és lenulláztam a ScenarioManager elcsúszott koordinátáit, helyreállítva a vizuális és fizikai tér szinkronját.<br>• **Port hiba:** A kalibrációs szkript Timeout-ot dobott. Észrevettem, hogy az AI a Python kódba hardkódolta az 5555-ös portot, míg a Unity Inspectorban én 5556-ot állítottam be. A portszámot javítottam. Végül a sikeres tesztek (fekete: ~0.010, fehér: ~0.983) után a White Threshold értékét saját hatáskörben, az adatok alapján 0.5-re állítottam be. |
| **Tanulság** | A fizikai Raycast használata generált 2D-s vonalakon instabil. A matematikai távolságszámítás robusztusabb. A vizuális és fizikai rétegek szétcsúszása ("Dupla eltolás") kritikus Unity hiba. A szenzorok kezdőpontját sosem szabad a modell geometriájára bízni. Hardkódolt portszámok használata veszélyes. |

### M08. 2D LiDAR-szimuláció
| Mező | Tartalom |
| :--- | :--- |
| **Dátum és mérföldkő** | 2026-08-17 (M08) |
| **Eszköz és modell** | Gemini (LLM) |
| **Cél** | Adatkompresszió és LiDAR implementáció. Geometriai tesztek generálása.<br><br>Mivel a roverünket egy LLM (Nyelvi Modell) fogja irányítani, nagyon nem mindegy, mennyi adatot zúdítunk rá. Egy 360 fokos, 1 fokos felbontású LiDAR 360 darab lebegőpontos számot adna vissza másodpercenként. Ez gyorsan felemésztené az LLM kontextusablakát. Íme az általam javasolt két alternatíva az összehasonlításhoz:<br><br>• **Alternatíva A: Nyers sugárvektor és érvényességi maszk (Raw Data)**<br>  • **Működés:** Minden egyes sugár pontos távolságát visszaadjuk egy tömbben, kiegészítve egy boolean tömbbel (maszkkal), ami megmondja, hogy az adott sugár érvényes-e vagy kimaradt (dropout).<br>  • **Előny/Hátrány:** Maximális részletesség, de hatalmas adatmennyiség. Hagyományos algoritmusoknak (B0 baseline) tökéletes, LLM-nek túl sok.<br>• **Alternatíva B: Szektorokra tömörített reprezentáció (Sectorized Data)**<br>  • **Működés:** A látómezőt (pl. 180 fok) felosztjuk N darab (pl. 3, 5 vagy 8) szektorra (pl. Bal, Közép-Bal, Közép, Közép-Jobb, Jobb). Minden szektorban kiszámoljuk a beeső sugarak minimum és átlag távolságát.<br>  • **Előny/Hátrány:** Drasztikus adatcsökkentés (360 szám helyett mondjuk csak 5 minimum érték). A minimum távolság tökéletes a biztonságos akadálykerüléshez, az LLM pedig könnyen megérti, hogy "Bal szektor minimum távolság: 1.2m". |
| **Prompt rövid kivonata** | "Mivel a roverünket egy LLM (Nyelvi Modell) fogja irányítani..." / "Hát de az akadályok random spawnolnak(piros kockák) és el is tünnek hamar. Ráadásul a time scale is 10-esen van..." |
| **Eredmény** | Elfogadva (javításokkal). Részben elutasított javaslat, emberi újratervezés. |
| **Emberi ellenőrzés** | Python tesztszkript futtatása, nyers vs. szektorizált JSON adatok elemzése.<br><br>**Emberi közbeavatkozás és refaktorálás:**<br>• **[KORREKCIÓ] Geometriai tesztek és dinamikus szcenárió ütközése:** Azonosítottam, hogy az AI által javasolt geometriai tesztesetek kivitelezhetetlenek a 10x-es gyorsított, mozgó akadályos pályán. Létrehoztam egy dedikált `LidarTestScene` tesztjelenetet, kikapcsoltam a `ScenarioManager`-t, és egy statikus teszt-kockával (`Obstacle_Test`) manuálisan, sikeresen leigazoltam a sugarak pontosságát minden irányból. |
| **Tanulság** | A nyers 360/30 fokos lebegőpontos tömb túl sok tokent fogyasztana; a szektorizált minimum-távolság tökéletes kompromisszum az AI agent számára. Az AI hajlamos elfelejteni a korábban beállított környezeti változókat (pl. időgyorsítás). A statikus mérési tesztekhez elengedhetetlen az elszigetelt, nyugodt tesztjelenet. Ezen felül nekem kellett kikényszerítenem a hiányzó `max_range` és késleltetés (latency) beépítését a C# kódba, hogy az API hiánytalan legyen. |
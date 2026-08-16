# AI Használati Napló

| Dátum és mérföldkő | Eszköz és modell | Cél | Prompt rövid kivonata | Eredmény | Emberi ellenőrzés | Tanulság |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| 2026-08-06 (M01) | Gemini (LLM) | 1. Próbafeladat: Könyvtárszerkezet generálása | "Mivel kell kezdeni ezt..." | Elfogadva | A bash parancsok lefutottak. | Az AI jól automatizálja a repó struktúra felépítését. |
| 2026-08-06 (M01) | Gemini (LLM) | 2. Próbafeladat: `doctor.py` script megírása | "ellenőrizném mindennek is a verzióját hogy jól van e fent" | Elfogadva | A script lefutott, kiírta a Unity verziót. | Képes Pythonból lekérdezni a Windows/WSL programverziókat. |
| 2026-08-06 (M01) | Gemini (LLM) | 3. Próbafeladat: README váz és checklist írása | "pontról pontra teljesítsen mindent amit az M01 ír" | Elfogadva | A README-t ellenőriztem és elmentettem. | Az AI pontosan követte a feladatkiírás követelményeit. |

| 2026-08-06 (M02) | Gemini (LLM) | Play Mode teszt és fizikai magyarázat kérése | "M02 feladat... MonoBehaviour, Update..." | Elfogadva | A Play Mode teszt zölden lefutott a Test Runnerben. | A FixedUpdate használata Rigidbody-val elengedhetetlen a stabil ütközésekhez, ezt méréssel (a padlón való átesés elkerülésével) igazoltam. |

| Dátum és mérföldkő | 2026-08-07, M03 |
| :--- | :--- |
| Eszköz és modell | Gemini |
| Cél | TCP szerver és Python CLI kliens hálózati kódjának elkészítése, Thread-safe architektúra javaslata és implementálása. |
| Prompt rövid kivonata | "Na akkor itt is lenne az M03 részletesen leírva. Arra kérlek hogy részletesen, mindent úgy csináljunk meg..." |
| Eredmény | Elfogadott javaslat. A szálbiztos (Producer-Consumer) architektúra és a JSON kommunikáció sikeresen felépült. |
| Emberi ellenőrzés | Python terminálból manuális tesztelés (`move`, `observe`, `stop`). Fókuszvesztésből és UTF-8 BOM-ból eredő szinkronhibák AI-val közös debuggolása. |
| Tanulság | A Unity háttérszálas (JsonUtility) limitációinak és az `Application.runInBackground` fontosságának megismerése. Sikeres hibakezelés (timeout, badjson) implementálása. |

Mozgásmodellek összehasonlítása

A feladat kéri, hogy hasonlítsunk össze legalább két rovermozgás-modellt, és indokoljuk meg, miért a kinematikussal indulunk.

1. Kinematikus modell (Kinematic Model)

Működési elv: A mozgást tisztán matematikai és geometriai egyenletek írják le az idő függvényében, anélkül, hogy figyelembe vennék az erőket, a tömeget vagy a súrlódást. A pozíciót közvetlenül a sebesség ($v$) és a szögsebesség  határozza meg.

Előnyök: 100%-osan determinisztikus. Ha ugyanazt a parancsot adod ki tízszer, a rover tízszer hajszálpontosan ugyanoda fog érkezni. Számításigénye minimális, az irányítás tökéletesen precíz.

Hátrányok: Nem reagál valósághűen a környezetre. Nincs tehetetlensége (azonnal megáll), és a kerekek nem tudnak "kipörögni" a sárban.

2. Dinamikus / Fizikai modell (Dynamic / Physics-based Model)

Működési elv: A mozgás erő- és nyomatékvektorok alkalmazásával történik (pl. motor forgatónyomatéka a tengelyen). Figyelembe veszi a rover tömegét, a gravitációt, a kerekek és a talaj közötti súrlódási együtthatót (Unity WheelCollider).

Előnyök: Rendkívül valósághű. A rover sodródik a kanyarban, lelassul az emelkedőn, és megcsúszik a jégen.

Hátrányok: Nem determinisztikus. A fizikai motor apró lebegőpontos kerekítési hibái miatt ugyanaz a parancs eltérő végeredményt adhat. Nagyon nehéz paraméterezni (súrlódási görbék, felfüggesztés rugózása).

Miért a kinematikus változattal indulunk? 

A kutatás jelenlegi fázisában az AI ágensek tanítása a cél. A gépi tanulás korai szakaszaiban kritikus fontosságú a determinizmus. Az ügynöknek először meg kell tanulnia a tiszta ok-okozati összefüggéseket (pl. ha kiadom az "előre" parancsot, a pozícióm "Y" értékkel nő). Ha egy dinamikus modellt használnánk, a fizikai motor apró csúszásai és "zajai" összezavarnák az AI-t, megnehezítve a konvergenciát. A kinematikus modell stabil, zajmentes alapot biztosít az első megfigyelés-akció (observe-move) ciklusok teszteléséhez, és biztosítja, hogy a prefab bármilyen új jelenetbe átemelve (acceptance criteria) pontosan ugyanúgy viselkedjen.


AI Protokoll-Review (Támadói és Debugger Szemszög):

    Sebezhetőség 1: Végtelen mozgás és kapcsolatvesztés (Orphaned Rover).

        Támadás/Hiba: A kliens kiad egy move(10000) parancsot, majd a hálózat megszakad. A rover vakon kimegy a világból.

        Védekezés: Hard-kódolt biztonsági korlátok (Max távolság: 5 méter / parancs). Hardveres/Szoftveres Watchdog timer bevezetése: ha 2 másodpercig megszakad a TCP kapcsolat, a rover kényszer-megállást (E-STOP) hajt végre.

    Sebezhetőség 2: Replay Attack (Visszajátszás) és Duplikációk.

        Támadás/Hiba: A TCP csomag késik, a kliens újra elküldi a move(2) parancsot. A rover összesen 4 métert megy.

        Védekezés: Idempotencia request_id alapján. A szerver memóriában tartja az utolsó 100 request_id-t. Ha ugyanaz az ID érkezik, nem hajtja végre újra a mozgást, csak visszaküldi az előző választ.

    Sebezhetőség 3: Állapot-inkonzisztencia (Race conditions).

        Támadás/Hiba: A kliens kiad egy turn(90) parancsot, miközben a rover még javában hajtja végre a move(5) parancsot.

        Védekezés: Szigorú Állapotgép (State Machine) bevezetése. Ha a rover állapota nem IDLE, minden új mozgási parancsot el kell dobni egy HTTP 409 Conflict-hoz hasonló ERR_BUSY hibakóddal. Csak a stop() és az observe() futhat le mozgás közben.

    Sebezhetőség 4: Típus- és Határhibák (Fuzzing vektorok).

        Támadás/Hiba: A payload NaN, Infinity, null, vagy extrém szám (pl. 1e99).

        Védekezés: Szigorú JSON Schema validáció a szerver oldalon, deszerializáció előtti szanálás.
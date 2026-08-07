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
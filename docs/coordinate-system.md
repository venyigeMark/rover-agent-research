# Rover Koordinátarendszer és Konvenciók (M04)

## 1. Koordinátarendszer (Unity Szabvány)
A projekt a Unity beépített bal kezes (left-handed) koordinátarendszerét használja:
*   **+X tengely:** Jobbra
*   **+Y tengely:** Felfelé (Gravitációval ellentétes irány)
*   **+Z tengely:** Előre (Forward)

## 2. Méretek és Egységek
Minden metrikus alapú, a SI mértékegységekhez közelítve.
*   **1 Unity Unit (Egység)** = 1 méter (m).
*   **Rover Alváz mérete:** 1.0m (szélesség) x 0.2m (magasság) x 1.5m (hosszúság).
*   **Kerekek sugara:** 0.5m.

## 3. Mozgás és Sebesség Konvenciók
A rover kinematikus modell alapján mozog. A vezérlőparancsok (`move <x> <y>`) normalizált $[-1.0, 1.0]$ tartományú értékeket várnak.

*   **Haladás (Lineáris sebesség - `payload_y`):**
    *   Előre: pozitív értékek (max +1.0) -> a lokális +Z tengely mentén.
    *   Hátra: negatív értékek (min -1.0) -> a lokális -Z tengely mentén.
    *   *Maximális sebesség:* 2.0 m/s.

*   **Fordulás (Szögsebesség - `payload_x`):**
    *   Jobbra: pozitív értékek (max +1.0) -> pozitív rotáció a lokális +Y tengely körül.
    *   Balra: negatív értékek (min -1.0) -> negatív rotáció a lokális +Y tengely körül.
    *   *Maximális fordulási sebesség:* 90 fok/másodperc.

## 4. Kerékanimáció
A kerekek forgása determinisztikusan követi a lineáris elmozdulást. A szögelfordulás (fokban) az elmozdulás és a kerék kerületének arányából számítódik ki, csúszás (slip) nélkül.
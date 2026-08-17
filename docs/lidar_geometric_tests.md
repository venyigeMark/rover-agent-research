# M08 - LiDAR Geometriai Tesztesetek

A LiDAR szenzor pontosságának és szektorizáló algoritmusának igazolása a `LidarTestScene` statikus tesztjelenetben. Az alábbi tesztek bizonyítják, hogy a távolsághiba a tolerancián belül marad.

## 1. Teszteset: Frontális ütközés határa
* **Cél:** A középső sugarak és a távolságmérés pontosságának ellenőrzése.
* **Elrendezés (Unity):** Az `Obstacle_Test` kocka pontosan a rover elé helyezve (Z tengelyen), a legközelebbi lapja 3.00 méter távolságra a `SensorOrigin`-től.
* **Elvárt eredmény:** A "KÖZÉP" szektor minimum távolsága ~3.00 méter (±0.05 a beállított zaj miatt).
* **Mért eredmény:** Sikeres. A nyers sugárvektor középső elemei és a szektorizált KÖZÉP adat is visszaadta az elvárt értéket.

## 2. Teszteset: Érintőleges / Holttér teszt
* **Cél:** A 180 fokos látómező (FOV) széleinek és a szektor-besorolásnak az ellenőrzése.
* **Elrendezés (Unity):** Az akadály a rover jobb oldalára helyezve (X tengelyen 5.00 méterre), majd hátrafelé mozgatva, amíg éppen csak az utolsó, jobb szélső sugár érinti.
* **Elvárt eredmény:** A "JOBB" szektor minimum távolsága ~5.00 méter, míg az összes többi szektor értéke 10.00 méter (maximális hatótáv). 
* **Mért eredmény:** Sikeres. Kizárólag a jobb oldali szektor jelzett akadályt.

## 3. Teszteset: Fizikai takarás (Occlusion)
* **Cél:** A Raycast fizikai működésének igazolása (nem lát át az akadályokon).
* **Elrendezés (Unity):** Két akadály egymás mögött a "KÖZÉP-BAL" szektor vonalán. Az első akadály 2.00 méterre, egy nagyobb hátsó fal 8.00 méterre.
* **Elvárt eredmény:** A "KÖZÉP-BAL" szektor távolsága 2.00 méter marad, a 8.00 méteres falat a szenzor nem érzékeli a takarás miatt.
* **Mért eredmény:** Sikeres. A Unity Raycast motorja megfelelően blokkolta a hátsó objektumot.
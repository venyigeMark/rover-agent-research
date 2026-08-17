# M07 - Alsó színérzékelő kalibráció

A kalibrációs méréseket a `tests/m07_calibration.py` (kalibrációs notebook/szkript) segítségével végeztük 50 mintán.

**Eredmények:**
* Üres padló (fekete/szürke) átlagos intenzitás: ~0.010
* Fehér vonal átlagos intenzitás: ~0.983

**Következtetés és beállított küszöbök:**
Mivel a két felület intenzitása jól elkülöníthető, a bináris (white/not_white) döntéshez a **White Threshold (küszöb) értékét 0.5-re állítottuk**. A bizonytalanságot ±0.05 zaj (noise) és 2%-os méréskimaradás (dropout) modellezi.
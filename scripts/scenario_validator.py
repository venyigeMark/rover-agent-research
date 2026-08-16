import json
import os
import sys

# Elvárt struktúra a Unity pályageneráláshoz
REQUIRED_KEYS = {
    "scenario_name": str,
    "time_scale": (int, float),          # Gyorsított futtatáshoz
    "background_color": str,             # Hex kód (pl. "#000000")
    "track": dict,
    "obstacles": dict
}

TRACK_KEYS = {
    "line_width": (int, float),          # Vonal szélessége
    "curvature_frequency": (int, float), # Görbület sűrűsége
    "curvature_amplitude": (int, float)  # Görbület nagysága
}

OBSTACLE_KEYS = {
    "seed": int,                         # A reprodukálhatóság kulcsa!
    "spawn_rate": (int, float),          # Milyen gyakran jelenjenek meg (mp)
    "max_concurrent": int                # Egyszerre max mennyi lehet a pályán
}

def validate_scenario(filepath):
    if not os.path.exists(filepath):
        print(f"[-] Hiba: A fájl nem található: {filepath}")
        return False

    with open(filepath, 'r', encoding='utf-8') as f:
        try:
            data = json.load(f)
        except json.JSONDecodeError as e:
            print(f"[-] Hiba: Érvénytelen JSON formátum - {e}")
            return False

    # Fő kulcsok ellenőrzése
    for key, expected_type in REQUIRED_KEYS.items():
        if key not in data:
            print(f"[-] Hiba: Hiányzó fő kulcs: '{key}'")
            return False
        if not isinstance(data[key], expected_type):
            print(f"[-] Hiba: '{key}' rossz típusú. Várt: {expected_type}")
            return False

    # Track kulcsok ellenőrzése
    for key, expected_type in TRACK_KEYS.items():
        if key not in data["track"]:
            print(f"[-] Hiba: Hiányzó track kulcs: '{key}'")
            return False

    # Obstacles kulcsok ellenőrzése
    for key, expected_type in OBSTACLE_KEYS.items():
        if key not in data["obstacles"]:
            print(f"[-] Hiba: Hiányzó obstacles kulcs: '{key}'")
            return False

    print(f"[+] Sikeres validáció: {filepath} tökéletes formátumú!")
    return True

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Használat: python scenario_validator.py <utvonal_a_json_hoz>")
    else:
        validate_scenario(sys.argv[1])
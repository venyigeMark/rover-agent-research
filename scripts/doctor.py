import sys
import subprocess
import os

# Kényszerítjük a kimenetet UTF-8-ra a Windows terminál hiba (UnicodeEncodeError) elkerülése végett
if sys.stdout.encoding.lower() != 'utf-8':
    sys.stdout.reconfigure(encoding='utf-8')

def check_command(command, name):
    try:
        # Futtatjuk a parancsot
        result = subprocess.run(command, capture_output=True, text=True, check=True)
        # Kiszedjük a verziószámot az első sorból
        version_info = result.stdout.strip().split('\n')[0]
        print(f"[OK] {name}: TELEPÍTVE -> {version_info}")
    except FileNotFoundError:
        print(f"[HIBA] {name}: NEM TALÁLHATÓ a rendszerben! (Nincs benne a PATH-ban)")
    except Exception as e:
        print(f"[FIGYELEM] {name}: Hiba az ellenőrzéskor -> {e}")

print("========================================")
print(" Rover Agent Research - M01 Rendszerellenorzes")
print("========================================\n")

# 1. Python verzió ellenőrzése
print(f"[OK] Python: TELEPÍTVE -> {sys.version.split(' ')[0]}")

# 2. Git verzió ellenőrzése
check_command(["git", "--version"], "Git")

# 3. Unity verzió ellenőrzése (Windows/WSL útvonalakon)
print("\n--- Unity Verzio Keresese ---")

# Ezeken az útvonalakon keresi a telepített verziókat
unity_wsl_path = "/mnt/c/Program Files/Unity/Hub/Editor"
unity_win_path = r"C:\Program Files\Unity\Hub\Editor"

def check_unity_path(path):
    if os.path.exists(path):
        # Kilistázzuk a mappában lévő Unity verziókat
        versions = [d for d in os.listdir(path) if os.path.isdir(os.path.join(path, d))]
        if versions:
            print(f"[OK] Unity: TELEPÍTVE a következő verzió(k) -> {', '.join(versions)}")
            return True
        else:
            print("[HIBA] Unity: A telepítési mappa létezik, de üres (nincs telepített editor verzió).")
            return True
    return False

# Megpróbáljuk mindkét útvonalat
found = check_unity_path(unity_wsl_path)
if not found:
    found = check_unity_path(unity_win_path)

if not found:
     print("[HIBA] Unity: Nem található a standard telepítési mappában (C:\\Program Files\\Unity\\Hub\\Editor).")

print("\n========================================")
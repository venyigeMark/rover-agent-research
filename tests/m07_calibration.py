import socket
import json
import time
import uuid

HOST = '127.0.0.1'
PORT = 5555 # JAVÍTVA: A Te Unity szervered portja!

def collect_calibration_data(samples=50):
    print(f"--- Kalibrációs adatgyűjtés indul ({samples} minta) ---")
    intensities = []
    
    for i in range(samples):
        try:
            # MINDEN mérésnél új TCP kapcsolatot nyitunk és zárunk
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                s.settimeout(2.0)
                s.connect((HOST, PORT))
                
                req = {
                    "request_id": str(uuid.uuid4()),
                    "version": "1.0",
                    "action": "observe",
                    "payload_value": 0.0
                }
                s.sendall((json.dumps(req) + "\n").encode('utf-8'))
                data = s.recv(1024)
                
                if data:
                    resp = json.loads(data.decode('utf-8-sig').strip())
                    # Ellenőrizzük, hogy benne van-e a szenzor adat a válaszban
                    if "sensor" in resp:
                        intensity = resp["sensor"]["center"]["intensity"]
                        # -1 a dropout (kimaradás) jele, azt most kihagyjuk az átlagolásból
                        if intensity >= 0: 
                            intensities.append(intensity)
                            
        except Exception as e:
            print(f"[-] Hiba a {i}. mérésnél: {e}")
        
        time.sleep(0.1) # 10 Hz-es mérés
            
    if not intensities:
        print("Hiba: Nem érkezett érvényes szenzoradat! Biztos, hogy fut a Unity?")
        return

    avg_int = sum(intensities) / len(intensities)
    min_int = min(intensities)
    max_int = max(intensities)
    
    print(f"Begyűjtött minták: {len(intensities)}")
    print(f"Minimum intenzitás: {min_int:.3f}")
    print(f"Maximum intenzitás: {max_int:.3f}")
    print(f"Átlagos intenzitás: {avg_int:.3f}")
    print("-" * 40)

if __name__ == "__main__":
    print("M07 - SZENZOR KALIBRÁCIÓ")
    print("1. LÉPÉS: Tedd a rovert a Unity-ben az ÜRES (fekete/szürke) PADLÓRA!")
    input("Nyomj Entert, ha kész...")
    collect_calibration_data()
    
    print("\n2. LÉPÉS: Tedd a rovert a Unity-ben PONTOSAN A FEHÉR VONALRA!")
    input("Nyomj Entert, ha kész...")
    collect_calibration_data()
    
    print("\n[AI Javaslat a küszöbre]: Állítsd a 'White Threshold' értéket a Unity-ben a két átlag közé (pl. 0.5-re)!")
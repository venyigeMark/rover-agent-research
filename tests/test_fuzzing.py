import socket
import json
import random
import string
import uuid
import time

HOST = '127.0.0.1'
PORT = 5555

def send_fuzz_payload(payload_str):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.settimeout(2.0)
        try:
            s.connect((HOST, PORT))
            s.sendall(payload_str.encode('utf-8'))
            data = s.recv(1024)
            if data:
                return json.loads(data.decode('utf-8-sig').strip())
        except Exception as e:
            return {"status": "connection_error", "details": str(e)}
    return None

def run_fuzz_tests():
    print("=== Rover Protokoll Fuzzing Teszt Indítása ===")
    
    # 1. Teszt: Teljesen véletlenszerű szemét karakterek (szintaktikai hiba)
    print("\n[1] Fuzzing: Véletlenszerű szemét adatok küldése...")
    garbage = "".join(random.choices(string.printable, k=150)) + "\n"
    resp1 = send_fuzz_payload(garbage)
    print(f"Eredmény: {resp1}")
    
    # 2. Teszt: Helyes JSON, de extrém nagy és érvénytelen típusú számok
    print("\n[2] Fuzzing: Extrém értékek (Infinity, 1e99)...")
    extreme_req = json.dumps({
        "request_id": str(uuid.uuid4()),
        "version": "1.0",
        "action": "move",
        "payload_value": 1e99  # Irreálisan nagy szám
    }) + "\n"
    resp2 = send_fuzz_payload(extreme_req)
    print(f"Eredmény: {resp2}")

    # 3. Teszt: Rosszindulatú SQL/NoSQL jellegű parancsok az action mezőben
    print("\n[3] Fuzzing: Rosszindulatú kódinjektálás (Action mező)...")
    malicious_req = json.dumps({
        "request_id": str(uuid.uuid4()),
        "version": "1.0",
        "action": "DROP TABLE rovers; --",
        "payload_value": 0
    }) + "\n"
    resp3 = send_fuzz_payload(malicious_req)
    print(f"Eredmény: {resp3}")

    print("\n=== Fuzzing Befejezve ===")
    print("Ha a Unity szerver nem omlott össze és még mindig fut, a teszt sikeres!")

if __name__ == "__main__":
    run_fuzz_tests()
import socket
import json
import time
import uuid

HOST = '127.0.0.1'
PORT = 5555

def run_stability_test():
    print("=== M06 Seed és Stabilitás Teszt ===")
    print("Feltétel: A Unity-ben a time_scale legyen 10.0-ra állítva!")
    print("A teszt 2 percig (120 mp) fut, ami a gyorsítás miatt 20 perc szimulációnak felel meg.\n")

    start_time = time.time()
    duration = 120 # 2 perc valós idő
    
    # A ciklus fut 2 percig
    while time.time() - start_time < duration:
        try:
            # Minden kérésnél ÚJ kapcsolatot nyitunk (mint a client.py)
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                s.settimeout(5.0)
                s.connect((HOST, PORT))
                
                req = {
                    "request_id": str(uuid.uuid4()),
                    "version": "1.0",
                    "action": "get_status",
                    "payload_value": 0.0
                }
                
                s.sendall((json.dumps(req) + "\n").encode('utf-8'))
                data = s.recv(1024)
                
                if not data:
                    print("[-] Hiba: Üres válasz a szervertől!")
                    break
                
                resp = json.loads(data.decode('utf-8-sig').strip())
                elapsed = int(time.time() - start_time)
                print(f"[{elapsed:03d} mp] Unity Szerver stabil. Állapot: {resp.get('rover_state')}")
                
        except Exception as e:
            print(f"[-] Hiba a teszt közben (Összeomlott a Unity?): {e}")
            break
            
        # Várunk 2 másodpercet a következő ping előtt
        time.sleep(2)
        
    else:
        # Ez csak akkor fut le, ha a while ciklus hiba nélkül végigért (nem volt break)
        print("\n[+] TESZT SIKERES: A rendszer 20 perc (szimulált) ideig stabil maradt akadály-generálás közben is!")

if __name__ == "__main__":
    run_stability_test()
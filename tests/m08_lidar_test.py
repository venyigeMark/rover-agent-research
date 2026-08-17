import socket
import json
import uuid

HOST = '127.0.0.1'
PORT = 5555 

def fetch_lidar_data():
    try:
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
            data = s.recv(4096) # Nagyobb buffer kell a 30 sugár miatt!
            
            if data:
                resp = json.loads(data.decode('utf-8-sig').strip())
                if "lidar" in resp:
                    return resp["lidar"]
                else:
                    print("Nincs 'lidar' mező a JSON-ben! Bekötötted a TcpServerController-be?")
    except Exception as e:
        print(f"Hálózati hiba: {e}")
    return None

if __name__ == "__main__":
    print("M08 - LiDAR Adatkompresszió Összehasonlítás\n")
    print("Állítsd a rovert úgy, hogy LÁSSON EGY AKADÁLYT (piros sugarak)!")
    input("Nyomj Entert a méréshez...")
    
    lidar_data = fetch_lidar_data()
    
    if lidar_data:
        raw_dist = lidar_data["raw_distances"]
        sec_min = lidar_data["sector_min_distances"]
        
        print("\n--- 1. ALTERNATÍVA: NYERS SUGÁRVEKTOR (RAW DATA) ---")
        print(f"Felbontás: {len(raw_dist)} db lebegőpontos szám")
        # Formázott kiírás 2 tizedesjegyre
        formatted_raw = [f"{d:.2f}" for d in raw_dist]
        print(f"Adat: {formatted_raw}")
        
        print("\n--- 2. ALTERNATÍVA: SZEKTORIZÁLT ADAT (SECTORIZED DATA) ---")
        print(f"Felbontás: {len(sec_min)} db lebegőpontos szám (Minimum távolságok)")
        sectors = ["BAL", "KÖZÉP-BAL", "KÖZÉP", "KÖZÉP-JOBB", "JOBB"]
        for i in range(len(sec_min)):
            print(f"{sectors[i]} szektor min távolság: {sec_min[i]:.2f} méter")
            
        print("\n[AI Következtetés]: LLM vezérléshez a Szektorizált (2.) alternatíva a megfelelő,")
        print("mert a nyers adathalmaz túlzottan pazarolná az LLM token-limitjét, míg a szektor-minimum")
        print("tökéletesen elegendő a biztonságos akadálykerüléshez.")
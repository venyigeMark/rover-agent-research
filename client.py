import socket
import json
import uuid
import datetime

HOST = '127.0.0.1'
PORT = 5555
VERSION = "1.0"

def send_command(action, x=0.0, y=0.0):
    message = {
        "request_id": str(uuid.uuid4()),
        "timestamp": datetime.datetime.now(datetime.timezone.utc).isoformat().replace("+00:00", "Z"),
        "version": VERSION,
        "action": action,
        "payload_x": x,
        "payload_y": y
    }
    
    json_str = json.dumps(message)
    
    try:
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(3.0)
            s.connect((HOST, PORT))
            s.sendall((json_str + '\n').encode('utf-8'))
            
            data = s.recv(1024)
            if not data:
                print("[HIBA] A szerver bontotta a kapcsolatot válasz nélkül (Üres adat).")
                return
                
            # RÖNTGENLÁTÁS: Kiírjuk, hogy pontosan mit kaptunk a hálózaton
            #print(f"\n[DEBUG] Nyers bájtok a Unity-től: {data}")
            
            raw_string = data.decode('utf-8-sig').strip()
            if not raw_string:
                print("[HIBA] A szerver csak egy üres sort vagy láthatatlan karaktereket küldött!")
                return
                
            response = json.loads(raw_string)
            print(f"[VÁLASZ] Státusz: {response.get('status')} | Pozíció: {response.get('position')}")
            if response.get('status') == 'error':
                print(f"[HIBA] {response.get('error_message')}")
                
    except socket.timeout:
         print("\n[HÁLÓZATI HIBA] Időtúllépés! A Unity nem válaszolt.")
    except Exception as e:
        print(f"\n[HÁLÓZATI HIBA] {e}")

def main():
    print("=== Rover CLI Kliens ===")
    print("Parancsok: observe, move <x> <y>, stop, exit, badjson (teszthez)")
    
    while True:
        cmd_input = input("\nParancs > ").strip().split()
        if not cmd_input: continue
        
        action = cmd_input[0].lower()
        
        if action == 'exit':
            break
        elif action == 'observe':
            send_command('observe')
        elif action == 'stop':
            send_command('stop')
        elif action == 'move':
            try:
                send_command('move', float(cmd_input[1]), float(cmd_input[2]))
            except (IndexError, ValueError):
                print("Használat: move <x> <y>")
        elif action == 'badjson':
            try:
                with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                    s.settimeout(3.0)
                    s.connect((HOST, PORT))
                    s.sendall(b'{"hibas_json: rossz szerver teszt\n')
                    data = s.recv(1024)
                    print("[NYERS VÁLASZ]:", data)
            except Exception as e:
                print("Hiba:", e)
        else:
            print("Ismeretlen parancs!")

if __name__ == "__main__":
    main()
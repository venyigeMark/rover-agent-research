import socket
import json
import uuid

HOST = '127.0.0.1'
PORT = 5555

def send_command(action, payload_value=0.0):
    request_data = {
        "request_id": str(uuid.uuid4()),
        "version": "1.0",
        "action": action,
        "payload_value": payload_value
    }
    
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        try:
            s.settimeout(2.0)
            s.connect((HOST, PORT))
            msg = json.dumps(request_data) + "\n"
            s.sendall(msg.encode('utf-8'))
            
            data = s.recv(1024)
            response = json.loads(data.decode('utf-8-sig').strip())
            print(f"Válasz: {json.dumps(response, indent=2)}")
        except Exception as e:
            print(f"Hiba a kapcsolatban: {e}")

def main():
    print("=== Rover CLI Kliens (v1.0 - M05) ===")
    print("Parancsok: observe, move <méter>, turn <fok>, stop, reset, exit")
    
    while True:
        try:
            cmd_input = input("\nParancs > ").strip().split()
            if not cmd_input: continue
            
            action = cmd_input[0].lower()
            
            if action == 'exit':
                break
            elif action in ['observe', 'stop', 'reset']:
                send_command(action)
            elif action in ['move', 'turn']:
                if len(cmd_input) > 1:
                    value = float(cmd_input[1])
                    send_command(action, value)
                else:
                    print("Hiányzó érték! Használat: move <méter> VAGY turn <fok>")
            else:
                print("Ismeretlen parancs.")
        except ValueError:
            print("Hibás számformátum!")
        except KeyboardInterrupt:
            break

if __name__ == "__main__":
    main()
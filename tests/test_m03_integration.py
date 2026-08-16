import socket
import json
import unittest
import uuid
import time

HOST = '127.0.0.1'
PORT = 5555

class TestRoverBasicIntegration(unittest.TestCase):
    def send_command(self, action, value=0.0):
        req = {
            "request_id": str(uuid.uuid4()),
            "version": "1.0",
            "action": action,
            "payload_value": value
        }
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(2.0)
            s.connect((HOST, PORT))
            s.sendall((json.dumps(req) + "\n").encode('utf-8'))
            data = s.recv(1024)
            return json.loads(data.decode('utf-8-sig').strip())

    def test_01_observe_command(self):
        """Alap integráció: Observe parancs működik és ad vissza pozíciót."""
        response = self.send_command("observe")
        self.assertEqual(response.get("status"), "success")
        self.assertIn("position", response)

    def test_02_valid_move(self):
        """Alap integráció: Szabályos move parancs elindítja a rovert."""
        response = self.send_command("move", 1.0)
        self.assertEqual(response.get("status"), "success")
        self.assertEqual(response.get("rover_state"), "MOVING")
        
        # A teszt végén megállítjuk, hogy tiszta lappal induljon a többi teszt
        time.sleep(0.1)
        self.send_command("stop")

if __name__ == '__main__':
    unittest.main()
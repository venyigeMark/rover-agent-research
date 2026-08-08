import socket
import json
import unittest

HOST = '127.0.0.1'
PORT = 5555

class TestRoverIntegration(unittest.TestCase):
    def send_command(self, payload):
        """Segédfüggvény a hálózati kérések küldéséhez és fogadásához."""
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(2.0)
            s.connect((HOST, PORT))
            s.sendall(payload)
            data = s.recv(1024)
            return data.decode('utf-8-sig').strip()

    def test_01_observe_command(self):
        """Teszteli, hogy a szerver helyes success státuszt ad-e egy observe kérésre."""
        msg = b'{"request_id":"test-1", "timestamp":"2026-08-08T10:00:00Z", "version":"1.0", "action":"observe", "payload_x":0.0, "payload_y":0.0}\n'
        
        response_str = self.send_command(msg)
        response = json.loads(response_str)
        
        self.assertEqual(response.get("status"), "success")
        self.assertEqual(response.get("request_id"), "test-1")
        self.assertIn("position", response)

    def test_02_bad_json_handling(self):
        """Teszteli, hogy a szerver nem omlik-e össze, és error-t ad-e rossz JSON esetén."""
        msg = b'{"hibas_json: lezaratlan string teszt\n'
        
        response_str = self.send_command(msg)
        response = json.loads(response_str)
        
        self.assertEqual(response.get("status"), "error")
        self.assertIn("error_message", response)

    def test_03_out_of_bounds_move(self):
        """Teszteli, hogy a szerver elutasítja-e a határokon túli értékeket."""
        msg = b'{"request_id":"test-2", "timestamp":"2026-08-08T10:00:00Z", "version":"1.0", "action":"move", "payload_x":5.0, "payload_y":0.0}\n'
        
        response_str = self.send_command(msg)
        response = json.loads(response_str)
        
        self.assertEqual(response.get("status"), "error")
        self.assertIn("error_message", response)

if __name__ == '__main__':
    unittest.main()
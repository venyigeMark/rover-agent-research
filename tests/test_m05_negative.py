import socket
import json
import unittest
import uuid
import time

HOST = '127.0.0.1'
PORT = 5555

class TestRoverM05Protocol(unittest.TestCase):
    def send_raw_command(self, payload_str):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(2.0)
            s.connect((HOST, PORT))
            s.sendall(payload_str.encode('utf-8'))
            data = s.recv(1024)
            return json.loads(data.decode('utf-8-sig').strip())

    def test_01_invalid_format(self):
        """Negatív teszt: Hiányzó action mező."""
        bad_json = json.dumps({"request_id": "test-err-1", "version": "1.0"}) + "\n"
        response = self.send_raw_command(bad_json)
        self.assertEqual(response.get("status"), "error")
        self.assertEqual(response.get("error_code"), "ERR_INVALID_FORMAT")

    def test_02_out_of_bounds(self):
        """Negatív teszt: Túl nagy távolság (biztonsági korlát)."""
        bad_request = json.dumps({
            "request_id": str(uuid.uuid4()),
            "version": "1.0",
            "action": "move",
            "payload_value": 10.0 # A limit 5.0
        }) + "\n"
        response = self.send_raw_command(bad_request)
        self.assertEqual(response.get("status"), "error")
        self.assertEqual(response.get("error_code"), "ERR_OUT_OF_BOUNDS")

    def test_03_busy_state(self):
        """Negatív teszt: Két mozgás egyszerre (Állapotgép teszt)."""
        # 1. Elindítjuk egy hosszú mozgásra (pl. 3 méter)
        req1 = json.dumps({
            "request_id": str(uuid.uuid4()), "version": "1.0", "action": "move", "payload_value": 3.0
        }) + "\n"
        resp1 = self.send_raw_command(req1)
        self.assertEqual(resp1.get("status"), "success")
        self.assertEqual(resp1.get("rover_state"), "MOVING")

        # 2. Azonnal ráküldünk egy kanyarodást, amíg még mozog
        req2 = json.dumps({
            "request_id": str(uuid.uuid4()), "version": "1.0", "action": "turn", "payload_value": 90.0
        }) + "\n"
        resp2 = self.send_raw_command(req2)
        
        # Ennek el kell buknia ERR_BUSY hibával
        self.assertEqual(resp2.get("status"), "busy")
        self.assertEqual(resp2.get("error_code"), "ERR_BUSY")
        
        # 3. Végül megállítjuk (hogy a többi teszt ne akadjon ki)
        req3 = json.dumps({
            "request_id": str(uuid.uuid4()), "version": "1.0", "action": "stop", "payload_value": 0.0
        }) + "\n"
        self.send_raw_command(req3)

if __name__ == '__main__':
    unittest.main()
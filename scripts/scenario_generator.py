import json
import os
import argparse

def generate_scenario(name, seed, time_scale, bg_color, line_width, curv_freq, curv_amp, spawn_rate, max_obs, output_dir):
    scenario = {
        "scenario_name": name,
        "time_scale": time_scale,
        "background_color": bg_color,
        "track": {
            "line_width": line_width,
            "curvature_frequency": curv_freq,
            "curvature_amplitude": curv_amp
        },
        "obstacles": {
            "seed": seed,
            "spawn_rate": spawn_rate,
            "max_concurrent": max_obs
        }
    }
    
    os.makedirs(output_dir, exist_ok=True)
    filepath = os.path.join(output_dir, f"{name}.json")
    
    with open(filepath, 'w', encoding='utf-8') as f:
        json.dump(scenario, f, indent=4)
        
    print(f"[+] Szcenárió legenerálva: {filepath}")
    return filepath

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="M06 Szcenárió Generátor")
    parser.add_argument("--name", required=True, help="Szcenárió neve (pl. train_1)")
    parser.add_argument("--seed", type=int, required=True, help="Random seed az akadályokhoz")
    parser.add_argument("--type", choices=["train", "test"], default="train", help="Szcenárió típusa")
    
    args = parser.parse_args()
    
    # Különböző alapbeállítások a train és test esetekhez
    if args.type == "train":
        generate_scenario(args.name, args.seed, time_scale=1.0, bg_color="#1a1a1a", 
                          line_width=0.5, curv_freq=2.0, curv_amp=5.0, spawn_rate=3.0, max_obs=5, output_dir="scenarios")
    else:
        # A test szcenárió lehet nehezebb (vékonyabb vonal, gyorsabb akadályok, gyorsított idő)
        generate_scenario(args.name, args.seed, time_scale=2.0, bg_color="#000000", 
                          line_width=0.3, curv_freq=3.5, curv_amp=7.0, spawn_rate=1.5, max_obs=8, output_dir="scenarios")
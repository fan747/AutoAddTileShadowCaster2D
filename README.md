# AutoAddTileShadowCaster2D 🌑

A simple Unity script that automatically generates ShadowCaster2D shapes from CompositeCollider2D components.

[![Original Script](https://img.shields.io/badge/Original_Script-Lumos_Github-blue.svg)](https://github.com/Lumos-Github/Auto-add-Shadow-Caster-2D-on-TileMap)  
*Modified version of Lumos's original script with additional improvements*

## 🚀 Getting Started

### Installation
1. Add `ShadowCaster2DCreator` to your Unity project
2. Attach the `ShadowCaster2DCreator` component to any GameObject containing:
   - CompositeCollider2D
   - Tilemap component (optional, but recommended)

### Usage
1. Select your GameObject with `ShadowCaster2DCreator` and `CompositeCollider2D`
2. Click the **Create** button in the inspector  
![Interface Preview](https://github.com/user-attachments/assets/7ab68678-791b-4671-bf49-228d13ec941f)

## ⚙️ Parameters
| Parameter | Description |
|-----------|-------------|
| `_extrudeDistance` | Vertex extrusion distance (controls shadow offset) |
| `_selfShadows` | Does the `ShadowCaster2D` component create self-shadows? |

## 📝 Notes
- Requires **Unity 2019.4+**
- Works best with 2DURP/2D HDRP projects
- Remember to enable "Used by Composite" on Tilemap Collider 2D

🔧 *For support or feature requests, please open an issue* 

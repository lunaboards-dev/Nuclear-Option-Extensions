# NOX - Nuclear Option eXtensions

## Squad markers

## RWR
This mod brings fully featured RWRs with threat identification (on most aircraft)

### RWR types

* **Band only:** The most basic RWR that can only identify the band of the radar emission, though it will correctly identify ARH missiles. It does not feature elevation indication. The Cricket and Compass have this type of RWR.
* **Helicopter:** This RWR only gives detailed threat ID on ground-based threats, and features no elevation indication. All helicopters have this RWR.
* **Fighter:** This RWR only gives detailed threat ID on other fighter radars, but also features relative elevation indication. The FS-12 and FS-20 have this type of RWR.
* **Full:** This RWR gives detailed threat ID on all sources and features relative elevation indication. The Brawler, Ifrit, Medusa, and Darkreach have this type of RWR.

### Threat types
* **AI:** (Air Intercept) A fighter radar
* **ATK:** (Attacker) An attacker or bomber radar
* **AEW:** (Airborne Early Warning) An AEW&C search radar
* **SAM:** Surface to Air Missile radar
* **AAA:** Anti-Aircraft Artillery. Currently unused.
* **NVL:** Naval radar.
* **FCS:** (Fire Control System) Illumination radar for a multi-vehicle SAM. Currently unused.
* **EWR:** Early Warning Radar.
* **MSL:** That's a missile you imbecile.
* **?:** Unknown radar contact. This usually just means I didn't add it to the threat library yet.


### Threat library

| Unit | Band | Class | ID |
| --- | --- | --- | --- |
| FS-12 Revoker | I | AI | `F12` |
| FS-20 Vortex | I | AI | `F20` |
| EW-25 Medusa | E | AEW | `E25` |
| KR-67 Ifrit | I | AI | `K67` |
| SFB-81 Darkreach | J | ATK | `B81` |
| T9K41 Boltstrike | K | SAM | `9K4` |
| Boltstrike RAM45 Launcher | K | SAM | `9K4` |
| HLT Radar Truck | C | EWR | `HLT` |
| MSV Radar | C | SAM | `R9` |
| MSV R9 Stratolance Lancer | J | SAM | `R9` |
| StratoLance R9 Launcher | J | SAM | `R9` |
| Radar Container | C | EWR | `EWR` |
| Radar Station | C | EWR | `EWR` |

**Note**: All ships show up as NVL or F band. All threats not added to the library show up as `?` or H band.
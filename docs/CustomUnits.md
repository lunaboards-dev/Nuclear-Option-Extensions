# Adding support for custom units to NOX

All unit defs should be in a folder called `.noxcfg` shipped with your mod, in the plugins folder.

## Fields

### `version`:integer
This is the version of the define that the define confirms to. This should always be set to `0`. 

### `deftype`:string
This is the type of define that the file is for. This should always be set to `unit`.

### `name`:string
This is the internal name of the unit.

### `rwr`:object (optional)
This contains all the information for the RWR.

#### `type`:string
Should be one of `band`, `helicopter`, `fighter`, `full`, or `mask`.

#### `mask`:array (optional)
A list of all threat classes that are able to be identified by the RWR. See the threat types section below. Only works on `mask` RWRs.

#### `elevation`:boolean (optional)
Should the RWR have elevation indication? Only works on `mask` RWRs.

### `threat`:object (optional)
This contains all the information for the radar threat ID. Only fill this out if the unit has a radar or fires SARH missiles.

#### `band`:string
The band of the radar. Must be a single letter from "A" to "Z".

#### `type`:string
The class of the threat. Must be one of the threat types. (See below)

#### `id`:string
The ID that shows up if the threat is propertly identified. Should not be longer than three letters.

## Threat types
* `air intercept` - Fighter radars
* `attacker` - Attackers and bombers.
* `aew` - Airborne early warning.
* `sam` - Surface to air missile.
* `aaa` - Anti-aircraft artillery.
* `naval` - Ships.
* `fcs` - Fire control system. This theoretically should be for the illumination radar, but that's not how SARH missiles work right now.
* `early warning` - Ground based early warning or search radar.

## Example: Ifrit
```jsonc
{
    // Always 0.
    "verison": 0,
    // Always "unit".
    "deftype": "unit",
    // Internal unit name. The Ifrit's is "Multirole1"
    "name": "Multirole1",
    "rwr": {
        "type": "full"
    },
    "threat": {
        "band": "J",
        "type": "air intercept",
        "id": "K67"
    }
}
```
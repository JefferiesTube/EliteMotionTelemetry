# EliteMotionTelemetry
A small helper application that reads motion telemetry from memory and serves them via UDP to be used by motion simulators.

## How it works

I utilize the Memory.dll library (https://newagesoldier.com/memory.dll/) to locate and read the motion data from the memory of Elite: Dangerous. The data is unchanged, no memory data is written or changed. The sole purpose is providing motion telemetry, no manipulation or cheating. Additionally, the memory location differs between Odyssey and Horizons as well as while using a ship or an SRV. Therefore, I use my other project LibElite (https://github.com/JefferiesTube/LibElite - Feedback appreciated) to use the Player Journal and the status.json written by Elite to detect any mode switches (e.g. entering a SRV).

All memory offsets are provided by a small JSON file that allows easy updating after any new Elite versions. And YES: Any new Elite update might break the functionality and require a new JSON file with new memory offsets.

## What data is provided

* Speed (magnitude, so far no clear distinction between forward/reverse)
* Roll
* Pitch
* Yaw
* Heave
* Sway
* Surge

## How to get the data

This project will not send the data directly to your motion system. It aims to provide a simple interface for you to write a plugin for your motion simulator. Currently, the data is sent via UDP (Port 4444) roughly every 50 milliseconds. Each datagram are the 7 proivded values (as floats) without any delimiters etc.

## Can I get banned?

Probably not, since a lot of motion system plugins use a similar approach. But since Fontier has no official statement regarding memory reading (reading only!), I will surely not guarantee that it's safe. If anyone of Frontier reads this: Let me know if I need to change anything to make this accepted. Or preferable: Make this project obsolete by finally providing an official telemetry API.

## Environment

Currently, Windows 64-Bit only. The project is built with WPF and .NET 4.7.2 - so it should run on almost any version of Windows without larger hassle. Unfortunately, no support for other OS/Consoles.

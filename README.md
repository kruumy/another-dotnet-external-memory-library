# another-dotnet-external-memory-library
### Common Usage
```
// Initialize Object
ProcessEx ex = new ProcessEx(System.Diagnostics.Process);

// Read Memory
T = ex.Read<T>(addr);
T[] = ex.Read<T>(addr, amount);

// Write Memory
ex.Write<T>(addr, value);
ex.Write<T>(addr, value[]);

// Get Module Base Address By Name
Base Address = ex["UnityPlayer.dll"];

// Add Offsets
Absolute Address = ex[0xFFFFFF];
Absolute Address = ex[0xFFFFFF,0x90,0x90,0x90,0x90];
Absolute Address = ex["UnityPlayer.dll",0xFFFFFF,0x90,0x90,0x90,0x90];

// Create Thread To Call Function
ex.UserCallx86(0xFFFFFF,"disconnect",0);
ex.Callx86(0xFFFFFF,0,"devmap mp_rust");

// Scan Memory
Addresses[] = ex.Scan(0x90,0x90,0x90,0x90);

// Window Control
ex.Window.X = 0
ex.Window.Y = 0
ex.Window.Width = 1920
ex.Window.Height = 1080

// and more
```
### Credits
https://github.com/Airyzz
https://github.com/shiversoftdev/External

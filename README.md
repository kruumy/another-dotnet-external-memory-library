# another-dotnet-external-memory-library
### Common Usage
```
// Initialize Object
ProcessEx ex = new ProcessEx(System.Diagnostics.Process)

// Read Memory
T = ex.Read<T>(addr)
T[] = ex.Read<T>(addr, amount)

// Write Memory
ex.Write<T>(addr, value)
ex.Write<T>(addr, value[])

// Get Module Base Address By Name
Base Address = ex["UnityPlayer.dll"]

// Add Offsets
Absolute Address = ex[0xFFFFFF]
Absolute Address = ex[0xFFFFFF,0x90,0x90,0x90,0x90]
Absolute Address = ex["UnityPlayer.dll",0xFFFFFF,0x90,0x90,0x90,0x90]

// Scan Memory
// WIP, might change later and havnt really tested
Addresses[] = ex.Scan(new byte[] {0x90,0x90,0x90,0x90})
Addresses[] = ex.Scan("E8 0A EC ? ? FF")
Addresses[] = ex.Scan<int>(999)

// and more
```
### Credits
https://github.com/shiversoftdev/External

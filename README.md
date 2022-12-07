# another-dotnet-external-memory-library
### Common Usage
```
Process process = Process.GetProcessesByName("iw4m").FirstOrDefault();

int result = process.Read<int>(0xFFFFFF);
byte[] results = process.Read<byte>(0xFFFFFF, 16);

process.Write<float>(0xFFFFFF, 120.0f);
process.Write<byte>(0xFFFFFF, 0x90, 0x90, 0x90, 0x90);

PointerEx[] results = process.Scan(0x0, PointerEx.MaxValue, 0x90, 0x90, 0x90, 0x90);

process.Call(0xFFFFFF, "disconnect", 0);
process.UserCall(0xFFFFFF, 0, "devmap mp_crash");

process.LoadLibraryA("totally_legit.dll");

PointerEx absoluteAddress = process.CalculatePointer(0x435F34, 0x15, 0x00, 0x55, 0x120);
PointerEx absoluteAddress = process.CalculatePointer("UnityPlayer.dll", 0x435F34, 0x15, 0x00, 0x55, 0x120);

process.GetMainWindow().X += 100;
process.GetMainWindow().Y += 100;
process.GetMainWindow().Width = 1920;
process.GetMainWindow().Height = 1080;
```

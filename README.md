# another-dotnet-external-memory-library (windows)
### Quick Overview
```
Process process = Process.GetProcessesByName("iw4m").FirstOrDefault();

int result = process.Read<int>(0xFFFFFFFF);
byte[] results = process.Read<byte>(0xFFFFFFFF, 16);

process.Write<float>(0xFFFFFFFF, 120.0f);
process.Write<byte>(0xFFFFFFFF, 0x90, 0x90, 0x90, 0x90);

UIntPtrEx[] results = process.Scan(0x0, UIntPtrEx.MaxValue, 0x90, 0x90, 0x90, 0x90);

process.Call(0xFFFFFFFF, 0, "disconnect");
process.UserCall(0xFFFFFFFF, "devmap mp_crash", 0);

process.LoadLibraryA("library.dll");

IntPtrEx absoluteAddress = process.CalculatePointer(0x435F34, 0x15, 0x00, 0x55, 0x120);
IntPtrEx absoluteAddress = process.CalculatePointer("UnityPlayer.dll", 0x435F34, 0x15, 0x00, 0x55, 0x120);

process.GetMainWindow().X += 100;
process.GetMainWindow().Y += 100;
process.GetMainWindow().Width = 1920;
process.GetMainWindow().Height = 1080;
```

# Installation

1. Cài đặt package 

`dotnet restore`

2. Build project 1 lần để hệ thống nhận diện file Playwright

`dotnet build`

3. Cài đặt công cụ Playwright CLI toàn cục (Chỉ làm 1 lần trên máy)

`dotnet tool install --global Microsoft.Playwright.CLI`

4. Tải tất cả các driver trình duyệt cần thiết về máy

`playwright install`

# Config

Hiện browser khi chạy test
- CMD: `set "HEADED=1" && dotnet test`
- PowerShell: `$env:HEADED=1; dotnet test`
- Linux/MacOS: `HEADED=1 dotnet test`
- Hoặc thêm vào file `appsettings.json`:
```json
{
  "PlaywrightSettings": {
	"Headed": true
  }
}
```
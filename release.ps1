# Build -> commit dist/BoxMetPlugin.dll -> push.
#
# Chạy không tham số (chuột phải > Run with PowerShell, hoặc .\release.ps1) → hiện MENU để chọn.
# Hoặc dùng tham số:
#   .\release.ps1 "msg"                 chạy cả 3 bước (build + commit + push)
#   .\release.ps1 -Build                chỉ build (DLL tự copy vào dist/)
#   .\release.ps1 -Commit "msg"         chỉ commit dist/BoxMetPlugin.dll (không build)
#   .\release.ps1 -Push                 chỉ push
#   .\release.ps1 -Build -Commit "msg"  ghép tùy ý
param(
    [Parameter(Position = 0)]
    [string]$Message = "",
    [switch]$Build,
    [switch]$Commit,
    [switch]$Push
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
$DefaultMessage = "Update BoxMetPlugin.dll"
$menuMode = ($PSBoundParameters.Count -eq 0)

try {
    if ($menuMode) {
        Write-Host ""
        Write-Host "=== BoxMet release ===" -ForegroundColor Cyan
        Write-Host "  1) Chỉ BUILD"
        Write-Host "  2) Chỉ COMMIT  (dist/BoxMetPlugin.dll, không build)"
        Write-Host "  3) Chỉ PUSH"
        Write-Host "  4) BUILD + COMMIT  (chưa push)"
        Write-Host "  5) BUILD + COMMIT + PUSH  (tất cả)"
        Write-Host "  0) Thoát"
        $choice = Read-Host "Chọn"
        switch ($choice) {
            "1" { $Build = $true }
            "2" { $Commit = $true }
            "3" { $Push = $true }
            "4" { $Build = $true; $Commit = $true }
            "5" { $Build = $true; $Commit = $true; $Push = $true }
            default { Write-Host "Thoát."; return }
        }
    }
    elseif (-not ($Build -or $Commit -or $Push)) {
        # Có truyền nội dung commit nhưng không chọn bước → chạy cả ba
        $Build = $Commit = $Push = $true
    }

    if ($Commit -and -not $Message) {
        if ($menuMode) {
            $in = Read-Host "Nội dung commit (Enter = '$DefaultMessage')"
            $Message = if ($in) { $in } else { $DefaultMessage }
        }
        else { $Message = $DefaultMessage }
    }

    if ($Build) {
        dotnet build boxvip/boxvip.csproj -c Debug
        if ($LASTEXITCODE -ne 0) { throw "Build lỗi, dừng." }
        # Bước post-build trong csproj đã copy DLL vào dist/
    }

    if ($Commit) {
        git add dist/BoxMetPlugin.dll
        git diff --cached --quiet
        if ($LASTEXITCODE -eq 0) {
            Write-Host "DLL không đổi, không có gì để commit."
        }
        else {
            git commit -m "$Message`n`nCo-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
            if ($LASTEXITCODE -ne 0) { throw "Commit lỗi." }
        }
    }

    if ($Push) {
        git push origin main
        if ($LASTEXITCODE -ne 0) { throw "Push lỗi." }
    }

    Write-Host "Xong." -ForegroundColor Green
}
catch {
    Write-Host $_.Exception.Message -ForegroundColor Red
}
finally {
    # Giữ cửa sổ mở khi chạy bằng double-click / Run with PowerShell
    if ($menuMode) { Read-Host "Nhấn Enter để đóng" | Out-Null }
}

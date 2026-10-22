# Build -> commit dist/BoxMetPlugin.dll -> push. Chọn từng bước riêng hoặc chạy cả ba.
#
#   .\release.ps1 "msg"                 chạy cả 3 bước (build + commit + push)
#   .\release.ps1 -Build                chỉ build (DLL tự copy vào dist/)
#   .\release.ps1 -Commit "msg"         chỉ commit dist/BoxMetPlugin.dll (không build)
#   .\release.ps1 -Push                 chỉ push
#   .\release.ps1 -Build -Commit "msg"  ghép tùy ý (build + commit, không push)
#
# Không ghi nội dung commit thì mặc định: "Update BoxMetPlugin.dll"
param(
    [Parameter(Position = 0)]
    [string]$Message = "Update BoxMetPlugin.dll",
    [switch]$Build,
    [switch]$Commit,
    [switch]$Push
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# Không chọn bước nào → chạy cả ba
if (-not ($Build -or $Commit -or $Push)) { $Build = $Commit = $Push = $true }

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

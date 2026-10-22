# Build -> commit dist/BoxMetPlugin.dll -> push.
# Dùng:  .\release.ps1 "nội dung commit"        (mặc định: "Update BoxMetPlugin.dll")
#        .\release.ps1 "msg" -NoPush            (chỉ commit, không push)
param(
    [string]$Message = "Update BoxMetPlugin.dll",
    [switch]$NoPush
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

dotnet build boxvip/boxvip.csproj -c Debug
if ($LASTEXITCODE -ne 0) { throw "Build lỗi, dừng." }

# Bước post-build trong csproj đã copy DLL vào dist/
git add dist/BoxMetPlugin.dll
git diff --cached --quiet
if ($LASTEXITCODE -eq 0) { Write-Host "DLL không đổi, không có gì để commit."; return }

git commit -m "$Message`n`nCo-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
if ($LASTEXITCODE -ne 0) { throw "Commit lỗi." }

if (-not $NoPush) {
    git push origin main
    if ($LASTEXITCODE -ne 0) { throw "Push lỗi." }
}

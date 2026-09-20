# BoxMetFlatV3

Plugin AutoCAD `BOXMET` táº¡o biÃªn dáº¡ng tráº£i pháº³ng há»™p kim loáº¡i táº¥m, gá»“m:

- BiÃªn dáº¡ng Ä‘Ã¡y vÃ  Ä‘Æ°á»ng NOBI dáº¡ng nÃ©t Ä‘á»©t.
- CÃ¡c flap cÃ³ chiá»u cao riÃªng cho tá»«ng cáº¡nh.
- Cung bo R ngoÃ i theo quy táº¯c hiá»‡n táº¡i cá»§a chÆ°Æ¡ng trÃ¬nh.
- Slit/lead-in lÃ  pháº§n cáº¯t riÃªng, khÃ´ng gá»™p vÃ o contour ngoÃ i.

## Cáº¥u trÃºc chÃ­nh

| ThÃ nh pháº§n | Vai trÃ² |
|---|---|
| `boxvip/Boxmetcommand.cs` | TÃ­nh vÃ  váº½ LINE/ARC, xá»­ lÃ½ hai hÆ°á»›ng tráº£i pháº³ng |
| `boxvip/Boxmetdialog.cs` | Há»™p thoáº¡i nháº­p L, W, T, váº­t liá»‡u, NOBI vÃ  chiá»u cao flap |
| `boxvip/NobiLookup.cs` | Tra cá»©u NOBI theo váº­t liá»‡u vÃ  chiá»u dÃ y |
| `references/dxf/MM.dxf` | Máº«u Ä‘á»‘i chiáº¿u hÃ¬nh há»c mong muá»‘n |
| `tests/fixtures/HT.dxf` | Máº«u kiá»ƒm tra hiá»‡n tráº¡ng, cÃ³ DIMENSION khoáº£ng slit `1.5` |
| `references/dxf/Mau_test.dxf` | Máº«u thá»­ trÆ°á»›c Ä‘Ã³ |
| `references/dxf/noilien.dxf` | Máº«u tham kháº£o contour LINE/ARC kÃ­n liÃªn tá»¥c |
| `references/nobi-standards.md` | Báº£ng tiÃªu chuáº©n NOBI tham kháº£o |

## Quy táº¯c hÃ¬nh há»c

- KÃ­ch thÆ°á»›c nháº­p vÃ o quyáº¿t Ä‘á»‹nh trá»±c tiáº¿p tá»a Ä‘á»™ biÃªn dáº¡ng; khÃ´ng sao chÃ©p tá»a Ä‘á»™ cá»‘ Ä‘á»‹nh tá»« `MM.dxf`.
- CÃ¡c cáº¡nh tháº³ng pháº£i cháº¡m Ä‘Ãºng Ä‘iá»ƒm tiáº¿p tuyáº¿n cá»§a cung R.
- Trong má»—i chuá»—i biÃªn dáº¡ng, Ä‘iá»ƒm cuá»‘i cá»§a LINE/ARC káº¿ tiáº¿p pháº£i trÃ¹ng vá»›i Ä‘iá»ƒm Ä‘áº§u cá»§a entity sau Ä‘á»ƒ táº¡o Ä‘Æ°á»ng liÃªn tá»¥c cho cáº¯t laser.
- Slit lÃ  chuá»—i cáº¯t riÃªng gá»“m LINE/ARC/LINE.
- Giá»¯ nguyÃªn quy táº¯c bÃ¡n kÃ­nh hiá»‡n táº¡i: `R=0.5`; trÆ°á»ng há»£p Ä‘áº·c biá»‡t cá»§a chÆ°Æ¡ng trÃ¬nh dÃ¹ng `R=2.0`.

## Build

YÃªu cáº§u AutoCAD .NET API assemblies Ä‘Æ°á»£c tham chiáº¿u trong mÃ´i trÆ°á»ng build. Build báº±ng:

```powershell
dotnet build .\boxvip\boxvip.csproj
```

Plugin Ä‘áº§u ra náº±m trong thÆ° má»¥c `boxvip/bin/` vÃ  Ä‘Æ°á»£c Git bá» qua.

## References vÃ  test fixtures

- `references/` chá»©a cÃ¡c máº«u Ä‘á»‘i chiáº¿u vÃ  tÃ i liá»‡u tiÃªu chuáº©n, khÃ´ng pháº£i Ä‘áº§u vÃ o runtime cá»§a plugin.
- `tests/fixtures/` chá»©a DXF dÃ¹ng Ä‘á»ƒ kiá»ƒm tra thá»§ cÃ´ng hoáº·c tá»± Ä‘á»™ng.
- `HT.dxf` giá»¯ khoáº£ng lá»‡ch `1.5` cá»§a slit; yÃªu cáº§u lÃ  ná»‘i liÃªn tá»¥c contour flap ngoÃ i, khÃ´ng kÃ©o slit sÃ¡t vÃ o gÃ³c Ä‘Ã¡y.
- `HT.bak` lÃ  báº£n dá»± phÃ²ng cá»§a fixture trÆ°á»›c khi kiá»ƒm tra.

HÆ°á»›ng dáº«n kiá»ƒm tra chi tiáº¿t náº±m táº¡i [`tests/README.md`](tests/README.md).

## Lá»‹ch sá»­ thay Ä‘á»•i

Xem lá»‹ch sá»­ báº±ng:

```powershell
git log --oneline --decorate --all
git diff HEAD~1..HEAD -- boxvip/Boxmetcommand.cs
```

Má»—i láº§n thay Ä‘á»•i hÃ¬nh há»c nÃªn táº¡o má»™t commit riÃªng, mÃ´ táº£ rÃµ nhÃ¡nh kÃ­ch thÆ°á»›c vÃ  loáº¡i ná»‘i Ä‘Ã£ sá»­a.

## PhiÃªn báº£n hiá»‡n táº¡i

### 0.2.0 â€” Sá»­a ná»‘i biÃªn dáº¡ng vÃ  bo R

- Sá»­a cÃ¡c cáº¡nh chÃ©o á»Ÿ flap pháº£i/trÃ¡i cá»§a nhÃ¡nh `Length >= Width`.
- Sá»­a cÃ¡c cung vÃ  Ä‘iá»ƒm tiáº¿p tuyáº¿n cá»§a nhÃ¡nh `Width > Length`.
- Giá»¯ slit lÃ  pháº§n cáº¯t riÃªng.
- Khá»Ÿi táº¡o Git Ä‘á»ƒ lÆ°u vÃ  Ä‘á»‘i chiáº¿u cÃ¡c phiÃªn báº£n.

### 0.2.1 â€” Sáº¯p xáº¿p references vÃ  test fixtures

- Gom DXF máº«u vÃ  tiÃªu chuáº©n vÃ o `references/`.
- Gom `HT.dxf` vÃ  báº£n dá»± phÃ²ng vÃ o `tests/fixtures/`.
- Giá»¯ khoáº£ng slit `1.5`, chá»‰ yÃªu cáº§u contour flap ngoÃ i ná»‘i tuyá»‡t Ä‘á»‘i.

### 0.1.0 â€” Má»‘c ban Ä‘áº§u

- PhiÃªn báº£n mÃ£ nguá»“n trÆ°á»›c khi sá»­a hÃ¬nh há»c bo R.


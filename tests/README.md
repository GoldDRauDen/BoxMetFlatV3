# Test fixtures

## `HT.dxf`

Fixture dÃ¹ng Ä‘á»ƒ kiá»ƒm tra biÃªn dáº¡ng sau khi cháº¡y lá»‡nh `BOXMET` trong AutoCAD.

Khi kiá»ƒm tra, cáº§n xÃ¡c nháº­n:

1. Khoáº£ng lá»‡ch cá»§a slit váº«n lÃ  `T=1.5` vÃ  slit váº«n lÃ  pháº§n cáº¯t riÃªng.
2. CÃ¡c Ä‘oáº¡n LINE á»Ÿ flap ngoÃ i cháº¡m Ä‘Ãºng Ä‘iá»ƒm tiáº¿p tuyáº¿n cá»§a cung R.
3. Äiá»ƒm cuá»‘i cá»§a LINE trÆ°á»›c trÃ¹ng vá»›i Ä‘iá»ƒm Ä‘áº§u hÃ¬nh há»c cá»§a ARC káº¿ tiáº¿p.
4. Sau cung R, Ä‘oáº¡n LINE tiáº¿p theo cÅ©ng báº¯t Ä‘áº§u Ä‘Ãºng táº¡i Ä‘iá»ƒm cuá»‘i cung.
5. Nháº­p bá»™ kÃ­ch thÆ°á»›c khÃ¡c thÃ¬ tá»a Ä‘á»™ thay Ä‘á»•i theo kÃ­ch thÆ°á»›c, khÃ´ng sao chÃ©p cá»‘ Ä‘á»‹nh tá»« `MM.dxf`.

`HT.bak` lÃ  báº£n dá»± phÃ²ng Ä‘á»ƒ Ä‘á»‘i chiáº¿u khi cáº§n phá»¥c há»“i fixture cÅ©.

## Quy trÃ¬nh kiá»ƒm tra thá»§ cÃ´ng

1. Build plugin báº±ng `dotnet build .\boxvip\boxvip.csproj`.
2. Náº¡p DLL vÃ o AutoCAD vÃ  cháº¡y lá»‡nh `BOXMET`.
3. Kiá»ƒm tra láº§n lÆ°á»£t cáº£ hai nhÃ¡nh `Length >= Width` vÃ  `Width > Length`.
4. DÃ¹ng `OSNAP Endpoint` vÃ  `DIST` Ä‘á»ƒ kiá»ƒm tra cÃ¡c Ä‘iá»ƒm ná»‘i LINE/ARC.
5. DÃ¹ng `PEDIT`/`JOIN` hoáº·c pháº§n má»m CAM Ä‘á»ƒ xÃ¡c nháº­n contour flap ngoÃ i nháº­n Ä‘Æ°á»£c lÃ  liÃªn tá»¥c.


# BoxMetFlatV3

Plugin AutoCAD `BOXMET` tạo biên dạng trải phẳng hộp kim loại tấm, gồm:

- Biên dạng đáy và đường NOBI dạng nét đứt.
- Các flap có chiều cao riêng cho từng cạnh.
- Cung bo R ngoài theo quy tắc hiện tại của chương trình.
- Slit/lead-in là phần cắt riêng, không gộp vào contour ngoài.

## Cấu trúc chính

| Thành phần | Vai trò |
|---|---|
| `boxvip/Boxmetcommand.cs` | Tính và vẽ LINE/ARC, xử lý hai hướng trải phẳng |
| `boxvip/Boxmetdialog.cs` | Hộp thoại nhập L, W, T, vật liệu, NOBI và chiều cao flap |
| `boxvip/NobiLookup.cs` | Tra cứu NOBI theo vật liệu và chiều dày |
| `references/dxf/MM.dxf` | Mẫu đối chiếu hình học mong muốn |
| `tests/fixtures/HT.dxf` | Mẫu kiểm tra hiện trạng, có DIMENSION khoảng slit `1.5` |
| `references/dxf/Mau_test.dxf` | Mẫu thử trước đó |
| `references/nobi-standards.md` | Bảng tiêu chuẩn NOBI tham khảo |

## Quy tắc hình học

- Kích thước nhập vào quyết định trực tiếp tọa độ biên dạng; không sao chép tọa độ cố định từ `MM.dxf`.
- Các cạnh thẳng phải chạm đúng điểm tiếp tuyến của cung R.
- Trong mỗi chuỗi biên dạng, điểm cuối của LINE/ARC kế tiếp phải trùng với điểm đầu của entity sau để tạo đường liên tục cho cắt laser.
- Slit là chuỗi cắt riêng gồm LINE/ARC/LINE.
- Giữ nguyên quy tắc bán kính hiện tại: `R=0.5`; trường hợp đặc biệt của chương trình dùng `R=2.0`.

## Build

Yêu cầu AutoCAD .NET API assemblies được tham chiếu trong môi trường build. Build bằng:

```powershell
dotnet build .\boxvip\boxvip.csproj
```

Plugin đầu ra nằm trong thư mục `boxvip/bin/` và được Git bỏ qua.

## References và test fixtures

- `references/` chứa các mẫu đối chiếu và tài liệu tiêu chuẩn, không phải đầu vào runtime của plugin.
- `tests/fixtures/` chứa DXF dùng để kiểm tra thủ công hoặc tự động.
- `HT.dxf` giữ khoảng lệch `1.5` của slit; yêu cầu là nối liên tục contour flap ngoài, không kéo slit sát vào góc đáy.
- `HT.bak` là bản dự phòng của fixture trước khi kiểm tra.

Hướng dẫn kiểm tra chi tiết nằm tại [`tests/README.md`](tests/README.md).

## Lịch sử thay đổi

Xem lịch sử bằng:

```powershell
git log --oneline --decorate --all
git diff HEAD~1..HEAD -- boxvip/Boxmetcommand.cs
```

Mỗi lần thay đổi hình học nên tạo một commit riêng, mô tả rõ nhánh kích thước và loại nối đã sửa.

## Phiên bản hiện tại

### 0.2.0 — Sửa nối biên dạng và bo R

- Sửa các cạnh chéo ở flap phải/trái của nhánh `Length >= Width`.
- Sửa các cung và điểm tiếp tuyến của nhánh `Width > Length`.
- Giữ slit là phần cắt riêng.
- Khởi tạo Git để lưu và đối chiếu các phiên bản.

### 0.2.1 — Sắp xếp references và test fixtures

- Gom DXF mẫu và tiêu chuẩn vào `references/`.
- Gom `HT.dxf` và bản dự phòng vào `tests/fixtures/`.
- Giữ khoảng slit `1.5`, chỉ yêu cầu contour flap ngoài nối tuyệt đối.

### 0.1.0 — Mốc ban đầu

- Phiên bản mã nguồn trước khi sửa hình học bo R.

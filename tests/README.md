# Test fixtures

## `HT.dxf`

Fixture dùng để kiểm tra biên dạng sau khi chạy lệnh `BOXMET` trong AutoCAD.

Khi kiểm tra, cần xác nhận:

1. Khoảng lệch của slit vẫn là `T=1.5` và slit vẫn là phần cắt riêng.
2. Các đoạn LINE ở flap ngoài chạm đúng điểm tiếp tuyến của cung R.
3. Điểm cuối của LINE trước trùng với điểm đầu hình học của ARC kế tiếp.
4. Sau cung R, đoạn LINE tiếp theo cũng bắt đầu đúng tại điểm cuối cung.
5. Nhập bộ kích thước khác thì tọa độ thay đổi theo kích thước, không sao chép cố định từ `MM.dxf`.

`HT.bak` là bản dự phòng để đối chiếu khi cần phục hồi fixture cũ.

## Quy trình kiểm tra thủ công

1. Build plugin bằng `dotnet build .\boxvip\boxvip.csproj`.
2. Nạp DLL vào AutoCAD và chạy lệnh `BOXMET`.
3. Kiểm tra lần lượt cả hai nhánh `Length >= Width` và `Width > Length`.
4. Dùng `OSNAP Endpoint` và `DIST` để kiểm tra các điểm nối LINE/ARC.
5. Dùng `PEDIT`/`JOIN` hoặc phần mềm CAM để xác nhận contour flap ngoài nhận được là liên tục.

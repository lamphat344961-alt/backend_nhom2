# backend_nhom2

1) /Data
AppDbContext.cs

Vai trò: “Trạm trung chuyển” giữa C# và SQL Server bằng EF Core.

Làm gì: Khai báo DbSet<T> cho từng bảng; áp dụng cấu hình (Fluent API); giữ kết nối/transaction khi SaveChanges().

Logic chính:

EF theo dõi entity ở 4 trạng thái (Added/Modified/Deleted/Unchanged) → translate thành INSERT/UPDATE/DELETE.

OnModelCreating() → ApplyConfiguration(new XxxConfig()) để gom quy tắc mapping.

Configurations/*.cs (Fluent API)

Vai trò: Nơi “định nghĩa schema” chi tiết cho mỗi entity.

Làm gì:

Chỉ định tên bảng/cột, kiểu dữ liệu (decimal(18,2)), HasMaxLength, ràng buộc PK/FK, khóa ghép.

Quy định hành vi xóa (DeleteBehavior.Cascade/Restrict) và quan hệ 1-n/n-n.

Logic từng file:

XeConfig.cs: Bảng XE (PK: BS_XE). Ràng buộc độ dài, quan hệ sang CT_DIEMGIAO.

DonHangConfig.cs: DONHANG (PK: MADON), kiểu tiền TONGTIEN, quan hệ tới CT_DONHANG và CT_DIEMGIAO.

DiemGiaoConfig.cs: DIEMGIAO (PK: D_DD), các cột tên/vị trí.

LoaiHangConfig.cs: LOAIHANG (PK: MALOAI) → 1-n với HANGHOA.

HangHoaConfig.cs: HANGHOA (PK: MAHH, FK MALOAI) → 1-n với CT_DONHANG.

CtDonHangConfig.cs: CT_DONHANG (PK ghép: MAHH+MADON), FK về HANGHOA & DONHANG, cột DONGIA, SL.

CtDiemGiaoConfig.cs: CT_DIEMGIAO (PK ghép: D_DD+BS_XE+MADON), FK về DIEMGIAO, XE, DONHANG, cột NGAYGIAO, TRANGTHAI.

Vì sao tách Config? Để Domain sạch, DbContext gọn, dễ bảo trì/migrate.

2) /Domain (Entities)

Vai trò: Mô hình hóa thực thể nghiệp vụ (bảng) dưới dạng class C#.

Làm gì:

Chứa thuộc tính tương ứng cột DB.

Chứa navigation properties để EF dựng quan hệ (vd DonHang.CtDonHangs).

Logic từng entity:

Xe: thông tin xe; BS_XE là PK; CtDiemGiaos để biết xe đó giao các điểm nào (qua bảng chi tiết).

DonHang: đơn hàng; gắn nhiều mặt hàng (CtDonHang) và nhiều điểm giao/xe (CtDiemGiao).

DiemGiao: nơi giao; liên kết nhiều record trong CT_DIEMGIAO.

LoaiHang: nhóm hàng; 1 loại có nhiều HangHoa.

HangHoa: mặt hàng; có thể nằm trong nhiều đơn (CtDonHang).

CtDonHang: chi tiết mặt hàng thuộc đơn (số lượng, đơn giá) – khóa ghép để không trùng cùng hàng trong cùng đơn.

CtDiemGiao: chi tiết điểm giao thuộc đơn và xe (ngày giao, trạng thái) – khóa ghép 3 cột để đảm bảo duy nhất 1 bản ghi cho (điểm, xe, đơn).

Domain không chứa logic truy cập DB (đó là việc của Data) → tách biệt business vs persistence.

3) /DTOs (Data Transfer Objects)

Vai trò: “Gói dữ liệu” chuyên cho API request/response (tránh lộ toàn bộ entity/tối ưu payload).

Làm gì:

DonHangCreateDto: dữ liệu khi tạo đơn (mã đơn, ngày lập, list item MAHH/SL/DONGIA).

DonHangReadDto: dữ liệu trả về khi xem đơn (thông tin header + list item).

Logic: Controller nhận CreateDto → map sang entity DonHang + CtDonHang → SaveChanges. Khi đọc, load include rồi map ra ReadDto.

Lợi ích: giữ API ổn định dù schema DB thay đổi nhỏ, tăng bảo mật và tính tương thích.

4) /Controllers (Web API)

Vai trò: Điểm vào HTTP.

Làm gì: Nhận request → gọi EF Core (qua AppDbContext) → trả JSON. Có thể thêm validate, transaction, cache, logging.

Logic từng controller (mẫu):

XeController

GET /api/xe: danh sách xe.

GET /api/xe/{id}: chi tiết xe.

POST: thêm xe.

PUT/{id}: sửa tên/trạng thái.

DELETE/{id}: xóa xe (bị chặn nếu còn dùng ở CT_DIEMGIAO do Restrict).

HangHoaController

CRUD hàng hóa; kiểm soát FK MALOAI còn hợp lệ; có thể thêm filter theo loại.

DonHangController

POST: tạo đơn (transaction) → chèn DonHang + nhiều CtDonHang → tính TONGTIEN.

GET/{id}: đọc đơn + include chi tiết, map ra DonHangReadDto.

DiemGiaoController

CRUD điểm giao; phục vụ màn hình quản lý/tra cứu.

CtDonHangController

Thêm/sửa/xóa mặt hàng trong đơn; kiểm tra tồn tại DonHang & HangHoa.

CtDiemGiaoController

POST upsert: gán (điểm giao + xe) vào đơn (MADON) + cập nhật NGAYGIAO, TRANGTHAI.

POST /complete?d_dd=&bs_xe=&madon=: đánh dấu hoàn thành điểm giao (ghi thời gian thực).

Sau này có thể bổ sung: Reorder route, Check-in GPS, Push notification.

Controller mỏng, không nhét business phức tạp: khi logic lớn, tách ra Service layer.

Dòng chảy nghiệp vụ tiêu biểu
A) Tạo đơn + gán hàng

FE gọi POST /api/donhang với DonHangCreateDto.

Controller tạo DonHang + CtDonHang (nhiều dòng).

Tính TONGTIEN = ∑ SL * DONGIA.

Lưu DB (1 transaction) → trả MADON.

B) Gán điểm giao cho đơn theo xe

FE chọn xe, điểm → gửi POST /api/ct_diemgiao với D_DD, BS_XE, MADON.

Nếu bản ghi chưa có → insert; có rồi → update ngày/trạng thái.

Sau này có thể chặn trùng khóa ghép hoặc validate rule (vd một điểm chỉ được 1 xe cho 1 đơn).

C) Tài xế hoàn thành điểm giao

App bấm “Hoàn thành” → POST /api/ct_diemgiao/complete?d_dd=&bs_xe=&madon=.

Backend set TRANGTHAI='HOANTHANH', NGAYGIAO=UtcNow.

(Mở rộng) Phát SignalR/FCM về cho chủ.

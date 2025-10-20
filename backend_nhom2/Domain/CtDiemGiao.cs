namespace backend_nhom2.Domain;


public class CtDiemGiao
{
    // Khóa/thuộc tính hiện có (đặt tên theo dự án của bạn)
    public string IdDD { get; set; } = default!;  // mã điểm giao
    public string MADON { get; set; } = default!; // mã đơn hàng
    public string? BS_XE { get; set; }
    public DateTime? NGAYGIAO { get; set; }
    public string TRANGTHAI { get; set; } = "CHO_GIAO";

    // ====== THÊM RÀNG BUỘC THỜI GIAN & THỜI GIAN PHỤC VỤ ======
    // epoch seconds; null = không ràng buộc
    public long? WindowStart { get; set; }
    public long? WindowEnd { get; set; }

    // phút phục vụ tại điểm; null/0 = mặc định 5-10 phút tuỳ bạn
    public int? ServiceMinutes { get; set; }

    // ====== Navigation (tuỳ dự án có/không) ======
    public DiemGiao DiemGiao { get; set; } = default!;
    public DonHang DonHang { get; set; } = default!;

    public Xe? Xe { get; set; }
}

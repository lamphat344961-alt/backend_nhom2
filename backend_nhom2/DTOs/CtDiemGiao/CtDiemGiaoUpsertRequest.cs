namespace backend_nhom2.DTOs.CtDiemGiao
{
    public class CtDiemGiaoUpsertRequest
    {
        public string IdDD { get; set; } = default!;     // FK -> DiemGiao
        public string MADON { get; set; } = default!;    // FK -> DonHang
        public DateTime? NGAYGIAO { get; set; }
        public string TRANGTHAI { get; set; } = "CHO_GIAO";

        // Ràng buộc thời gian (epoch giây, UTC); null = không ràng buộc
        public long? WindowStart { get; set; }
        public long? WindowEnd { get; set; }

        // Phút phục vụ tại điểm (null/0 => mặc định 10)
        public int? ServiceMinutes { get; set; }
    }
}

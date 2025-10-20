using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace backend_nhom2.Domain
{
    public class CtDiemGiao
    {
        // PK: MADON + IdDD (bảng nối Đơn <-> Điểm)
        public string MADON { get; set; } = default!;
        public string IdDD { get; set; } = default!;

        public DateTime? NGAYGIAO { get; set; }
        public string TRANGTHAI { get; set; } = "CHO_GIAO";

        // Ràng buộc thời gian (epoch giây, UTC); null = không ràng buộc
        public long? WindowStart { get; set; }
        public long? WindowEnd { get; set; }
        public int? ServiceMinutes { get; set; }

        // Navigation — phải nullable + bỏ khỏi validation/binding JSON
        [JsonIgnore, ValidateNever] public DonHang? DonHang { get; set; }
        [JsonIgnore, ValidateNever] public DiemGiao? DiemGiao { get; set; }
    }
}

namespace backend_nhom2.DTOs.DiemGiao
{
    public class DiemGiaoCreateRequest
    {
        // Mã điểm giao (tuỳ bạn để client gửi hoặc server tự sinh).
        public string IdDD { get; set; } = default!;
        public string TEN { get; set; } = default!;
        public string VITRI { get; set; } = default!; // địa chỉ văn bản

        // Lat/Lng để trống: server sẽ tự geocode
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}

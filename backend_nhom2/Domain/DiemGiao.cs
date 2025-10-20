namespace backend_nhom2.Domain;


public class DiemGiao
{
    // PK (ID_DĐ) -> đặt tên code: IdDD và map cột "D_DD"
    public string IdDD { get; set; } = string.Empty;
    public string? VITRI { get; set; }
    public string? TEN { get; set; }

    public double? Lat { get; set; }
    public double? Lng { get; set; }


    public ICollection<CtDiemGiao> CtDiemGiaos { get; set; } = new List<CtDiemGiao>();
}
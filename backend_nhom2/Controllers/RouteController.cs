using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_nhom2.Data;                // AppDbContext
using backend_nhom2.DTOs.Route;          // DTOs Route
using backend_nhom2.Models.Route;        // RoutePlan, RouteStop
using backend_nhom2.Services.Route;      // OsmClients, Optimizer
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly OsmClients _osm;
        private readonly IConfiguration _config;

        public RouteController(AppDbContext db, OsmClients osm, IConfiguration config)
        {
            _db = db;
            _osm = osm;
            _config = config;
        }

        // ===== Giữ nguyên endpoint optimize (FE truyền thẳng điểm nếu muốn) =====
        [HttpPost("optimize")]
        public async Task<ActionResult<OptimizeResult>> Optimize([FromBody] OptimizeRequest req)
        {
            if (req.points == null || req.points.Count == 0)
                return BadRequest("Danh sách điểm trống.");

            var depotSection = _config.GetSection("Depot");
            if (!depotSection.Exists())
                return BadRequest("Thiếu cấu hình 'Depot' trong appsettings.json.");
            string depotName = depotSection["Name"] ?? "Kho Trung Tâm";
            if (!double.TryParse(depotSection["Lat"], out double depotLat) ||
                !double.TryParse(depotSection["Lng"], out double depotLng))
                return BadRequest("Tọa độ kho không hợp lệ trong cấu hình Depot.");

            // Chèn depot
            var depot = new OptimizePoint(0, depotName, depotLat, depotLng, null, null, 0);
            req.points.Insert(0, depot);

            return await RunOptimizationAndPersistAsync(req.points, (int)req.departureEpoch);
        }

        // ===== NEW: tối ưu theo danh sách đơn, tự lấy ràng buộc thời gian từ CtDiemGiao =====
        [HttpPost("optimize-from-orders")]
        public async Task<ActionResult<OptimizeResult>> OptimizeFromOrders([FromBody] OptimizeFromOrdersRequest req)
        {
            if (req.orderIds == null || req.orderIds.Count == 0)
                return BadRequest("Thiếu danh sách mã đơn (orderIds).");

            var depotSection = _config.GetSection("Depot");
            if (!double.TryParse(depotSection["Lat"], out double depotLat) ||
                !double.TryParse(depotSection["Lng"], out double depotLng))
                return BadRequest("Tọa độ kho không hợp lệ trong cấu hình Depot.");
            string depotName = depotSection["Name"] ?? "Kho Trung Tâm";

            // Lấy tất cả (điểm giao, ràng buộc) của các đơn hàng
            var q = from ct in _db.CtDiemGiaos.AsNoTracking()
                    join dg in _db.DiemGiaos.AsNoTracking() on ct.IdDD equals dg.IdDD
                    where req.orderIds.Contains(ct.MADON)
                    select new
                    {
                        dg.IdDD,
                        dg.TEN,
                        dg.Lat,
                        dg.Lng,
                        ct.WindowStart,
                        ct.WindowEnd,
                        ct.ServiceMinutes
                    };

            var raw = await q.ToListAsync();
            if (raw.Count == 0)
                return BadRequest("Không tìm thấy điểm giao nào cho các đơn đã chọn.");

            // Gom theo IdDD để mỗi điểm xuất hiện 1 lần, ưu tiên time window chặt nhất
            var grouped = raw
                .Where(p => p.Lat.HasValue && p.Lng.HasValue)
                .GroupBy(p => new { p.IdDD, p.TEN, p.Lat, p.Lng })
                .Select(g =>
                {
                    // Chọn time-window chặt nhất trong group (nếu có nhiều đơn cùng điểm)
                    long? ws = g.Min(x => x.WindowStart);
                    long? we = g.Min(x => x.WindowEnd);
                    // ServiceMinutes: lấy max (bảo thủ) hoặc giá trị đầu tiên nếu null hết
                    int service = g.Max(x => x.ServiceMinutes ?? 0);
                    if (service <= 0) service = 10;

                    return new
                    {
                        g.Key.IdDD,
                        g.Key.TEN,
                        Lat = g.Key.Lat!.Value,
                        Lng = g.Key.Lng!.Value,
                        WindowStart = ws,
                        WindowEnd = we,
                        ServiceMinutes = service
                    };
                })
                .ToList();

            if (grouped.Count == 0)
                return BadRequest("Các điểm giao chưa có Lat/Lng. Vui lòng cập nhật toạ độ.");

            var points = new List<OptimizePoint>
            {
                new OptimizePoint(0, depotName, depotLat, depotLng, null, null, 0) // depot
            };

            int id = 1;
            foreach (var p in grouped)
            {
                points.Add(new OptimizePoint(
                    id: id++,
                    name: $"{p.TEN} ({p.IdDD})",
                    lat: p.Lat,
                    lng: p.Lng,
                    windowStart: p.WindowStart,   // <<=== ràng buộc bắt đầu
                    windowEnd: p.WindowEnd,       // <<=== ràng buộc kết thúc
                    serviceMinutes: p.ServiceMinutes
                ));
            }

            return await RunOptimizationAndPersistAsync(points, (int)req.departureEpoch);
        }

        // ===== Common: chạy tối ưu + lưu DB =====
        private async Task<ActionResult<OptimizeResult>> RunOptimizationAndPersistAsync(List<OptimizePoint> points, int departureEpoch)
        {
            var coords = points.Select(p => (p.lat, p.lng)).ToList();
            var matrix = await _osm.BuildTimeMatrixAsync(coords);

            var tws = points.Select(p => new Optimizer.TwNode
            {
                Start = p.windowStart ?? long.MinValue / 2,
                End = p.windowEnd ?? long.MaxValue / 2,
                ServiceSeconds = (p.serviceMinutes > 0 ? p.serviceMinutes : 10) * 60
            }).ToArray();

            var (order, totalSec) = Optimizer.SolveSingleVehicleTW(matrix, tws, 0, departureEpoch);

            var stops = new List<OptimizedStop>();
            long lastEta = departureEpoch;

            for (int i = 1; i < order.Length; i++)
            {
                int fromIdx = order[i - 1];
                int toIdx = order[i];

                var from = points[fromIdx];
                var to = points[toIdx];

                var polyline = await _osm.GetPolylineAsync(from.lat, from.lng, to.lat, to.lng);
                int segSec = matrix[fromIdx, toIdx];
                lastEta += segSec;

                stops.Add(new OptimizedStop(
                    pointId: to.id,
                    name: to.name,
                    lat: to.lat,
                    lng: to.lng,
                    etaEpoch: lastEta,
                    etaIso: DateTimeOffset.FromUnixTimeSeconds(lastEta).UtcDateTime.ToString("O"),
                    polyline: polyline
                ));
            }

            var route = new RoutePlan
            {
                TotalSeconds = totalSec,
                Stops = stops.Select((s, idx) => new RouteStop
                {
                    Order = idx + 1,
                    Name = s.name,
                    Lat = s.lat,
                    Lng = s.lng,
                    EtaEpoch = s.etaEpoch,
                    EtaIso = s.etaIso,
                    Polyline = s.polyline
                }).ToList()
            };

            _db.Add(route);
            await _db.SaveChangesAsync();

            var result = new OptimizeResult(
                route.Id,
                stops,
                totalSec,
                TimeSpan.FromSeconds(totalSec).ToString(@"hh\\:mm\\:ss")
            );

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoutePlan>> GetById(int id)
        {
            var route = await _db.Set<RoutePlan>()
                .Include(r => r.Stops)
                .OrderBy(r => r.Id)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return NotFound();
            return Ok(route);
        }
    }
}

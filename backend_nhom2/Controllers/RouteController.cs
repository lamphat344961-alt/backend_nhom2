using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_nhom2.Data;
using backend_nhom2.DTOs.Route;
using backend_nhom2.Models.Route;
using backend_nhom2.Services.Route;
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

        // ===== optimize giữ nguyên =====
        [HttpPost("optimize")]
        public async Task<ActionResult<OptimizeResult>> Optimize([FromBody] OptimizeRequest req)
        {
            if (req.points == null || req.points.Count == 0)
                return BadRequest("Danh sách điểm trống.");

            var depotSection = _config.GetSection("Depot");
            if (!double.TryParse(depotSection["Lat"], out double depotLat) ||
                !double.TryParse(depotSection["Lng"], out double depotLng))
                return BadRequest("Tọa độ kho không hợp lệ trong cấu hình Depot.");
            string depotName = depotSection["Name"] ?? "Kho Trung Tâm";

            var depot = new OptimizePoint(0, depotName, depotLat, depotLng, null, null, 0);
            req.points.Insert(0, depot);

            return await RunOptimizationAndPersistAsync(req.points, (int)req.departureEpoch);
        }

        // ===== NEW: optimize-from-orders (sử dụng DonHang + DiemGiao) =====
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

            // Lấy thông tin điểm giao từ DonHang
            var query = from d in _db.DonHangs.AsNoTracking()
                        join g in _db.DiemGiaos.AsNoTracking() on d.D_DD equals g.IdDD
                        where req.orderIds.Contains(d.MADON)
                        select new
                        {
                            g.IdDD,
                            g.TEN,
                            g.Lat,
                            g.Lng,
                            d.WindowStart,
                            d.WindowEnd,
                            d.ServiceMinutes
                        };

            var raw = await query.ToListAsync();
            if (raw.Count == 0)
                return BadRequest("Không tìm thấy điểm giao nào cho các đơn hàng.");

            var grouped = raw
                .Where(p => p.Lat.HasValue && p.Lng.HasValue)
                .GroupBy(p => new { p.IdDD, p.TEN, p.Lat, p.Lng })
                .Select(g =>
                {
                    long? ws = g.Min(x => x.WindowStart);
                    long? we = g.Min(x => x.WindowEnd);
                    int service = g.Max(x => x.ServiceMinutes ?? 10);
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
                return BadRequest("Không có điểm giao có toạ độ hợp lệ.");

            var points = new List<OptimizePoint>
            {
                new OptimizePoint(0, depotName, depotLat, depotLng, null, null, 0)
            };

            int id = 1;
            foreach (var p in grouped)
            {
                points.Add(new OptimizePoint(
                    id: id++,
                    name: $"{p.TEN} ({p.IdDD})",
                    lat: p.Lat,
                    lng: p.Lng,
                    windowStart: p.WindowStart,
                    windowEnd: p.WindowEnd,
                    serviceMinutes: p.ServiceMinutes
                ));
            }

            return await RunOptimizationAndPersistAsync(points, (int)req.departureEpoch);
        }

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
            var route = await _db.RoutePlans
                .Include(r => r.Stops)
                .OrderBy(r => r.Id)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return NotFound();
            return Ok(route);
        }
    }
}

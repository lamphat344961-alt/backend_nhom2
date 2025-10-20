
using Google.OrTools.ConstraintSolver;

namespace backend_nhom2.Services.Route
{
    public static class Optimizer
    {
        public class TwNode
        {
            public long Start;        // earliest epoch secs (hoặc long.MinValue/2)
            public long End;          // latest epoch secs (hoặc long.MaxValue/2)
            public int ServiceSeconds; // thời gian phục vụ tại điểm (giây)
        }

        /// <summary>
        /// Giải bài toán 1 xe, có time-window, dựa trên ma trận thời gian (giây).
        /// Node 0 là depot/kho; các node 1..N là điểm dừng.
        /// </summary>
        public static (int[] order, int totalSec) SolveSingleVehicleTW(
            int[,] travelTimeSec,
            TwNode[] tws,                // độ dài = N (ứng với node 1..N)
            int serviceSecondsDefault,   // service time mặc định khi TwNode không set
            int startEpoch               // epoch seconds lúc khởi hành
        )
        {
            int n = travelTimeSec.GetLength(0);
            if (n < 2) return (new[] { 0 }, 0);

            var manager = new RoutingIndexManager(n, 1, 0);
            var routing = new RoutingModel(manager);

            // callback thời gian di chuyển
            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                int fromNode = manager.IndexToNode(fromIndex);
                int toNode = manager.IndexToNode(toIndex);
                return travelTimeSec[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Thêm dimension thời gian
            routing.AddDimension(
                transitCallbackIndex,
                24 * 3600,            // allow waiting (slack)
                7 * 24 * 3600,        // max horizon
                false,                // không force start at zero
                "Time"
            );

            var timeDimension = routing.GetMutableDimension("Time");

            // Ràng buộc time-window cho mỗi node
            for (int i = 0; i < n; i++)
            {
                var idx = manager.NodeToIndex(i);
                long start = (i == 0) ? startEpoch : (tws[i - 1]?.Start ?? long.MinValue / 2);
                long end = (i == 0) ? long.MaxValue / 2 : (tws[i - 1]?.End ?? long.MaxValue / 2);
                timeDimension.CumulVar(idx).SetRange(start, end);
            }

            // Service time tại mỗi điểm (dùng SlackVar)
            for (int i = 1; i < n; i++)
            {
                int svc = tws[i - 1]?.ServiceSeconds ?? serviceSecondsDefault;
                timeDimension.SlackVar(manager.NodeToIndex(i)).SetValue(svc);
            }

            // Tham số tìm kiếm
            var searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            // Nếu môi trường thiếu Google.Protobuf, vẫn build vì gói đã add ở csproj
            searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration { Seconds = 5 };

            var solution = routing.SolveWithParameters(searchParameters);
            if (solution == null) return (new[] { 0 }, 0);

            // Đọc thứ tự thăm
            var order = new List<int>();
            long index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                order.Add(manager.IndexToNode(index));
                index = solution.Value(routing.NextVar(index));
            }
            order.Add(manager.IndexToNode(index)); // end node

            int totalSec = (int)solution.ObjectiveValue();
            return (order.ToArray(), totalSec);
        }
    }
}

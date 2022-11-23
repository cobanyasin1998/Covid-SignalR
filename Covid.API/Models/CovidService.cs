using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Covid.API.Models
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;
        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }
        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }


        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select tarih,[1],[2],[3],[4],[5] FROM (select[City],[Count], Cast([CovidDate] as date) as tarih FROM Covids) as covidT PIVOT (SUM(COUNT) For City IN([1],[2], [3],[4],[5]) ) as ptable order by tarih asc";

                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chart = new CovidChart
                            {
                                CovidDate = reader.GetDateTime(0).ToShortDateString(),
                            };

                            Enumerable.Range(1, 5).ToList().ForEach(x =>
                            {
                                if (System.DBNull.Value.Equals(reader[x]))
                                {
                                    chart.Counts.Add(0);
                                }
                                else
                                {
                                    chart.Counts.Add(reader.GetInt32(x));
                                }
                            });

                            covidCharts.Add(chart);

                        }
                    }

                    _context.Database.CloseConnection();
                    return covidCharts;
                }
                catch (System.Exception ex)
                {

                    throw;
                }
              
            };
        }
    }
}

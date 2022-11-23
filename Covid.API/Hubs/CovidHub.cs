using System.Threading.Tasks;
using Covid.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace Covid.API.Hubs
{
    public class CovidHub : Hub
    {
        private readonly CovidService _covidService;
        public CovidHub(CovidService covidService)
        {
            _covidService = covidService;
        }
        public async Task GetCovidList()
        {
            await Clients.All.SendAsync("ReceiveCovidList",
                _covidService.GetCovidChartList());

        }
    }
}

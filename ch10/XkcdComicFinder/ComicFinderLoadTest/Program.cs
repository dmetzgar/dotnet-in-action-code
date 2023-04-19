using NBomber.CSharp;
using NBomber.Http.CSharp;

using var httpClient = new HttpClient();

var scenario = Scenario.Create("search_mom", 
  async context =>
  {
    var request =
      Http.CreateRequest("GET", 
        "http://localhost:5123/search?searchText=Mom");

    var response = await Http.Send(httpClient, request);
    return response;
  })
.WithoutWarmUp()
.WithLoadSimulations(
  Simulation.Inject(
    rate: 100, 
    interval: TimeSpan.FromSeconds(1),
    during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();

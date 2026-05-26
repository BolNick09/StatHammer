using System.Windows;
using StatHammer.DesktopClient.Services;

namespace StatHammer.DesktopClient
{
    public partial class App : Application
    {
        public static ApiClient ApiClient { get; private set; } = null!;

        public static AuthService AuthService { get; private set; } = null!;

        public static UnitService UnitService { get; private set; } = null!;

        public static SimulationService SimulationService { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApiClient = new ApiClient();
            AuthService = new AuthService(ApiClient);
            UnitService = new UnitService(ApiClient);
            SimulationService = new SimulationService(ApiClient);
        }
    }
}
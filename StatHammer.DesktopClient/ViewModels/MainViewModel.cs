using StatHammer.DesktopClient.Services;

namespace StatHammer.DesktopClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly UnitService _unitService;
        private readonly SimulationService _simulationService;

        public MainViewModel(
            AuthService authService,
            UnitService unitService,
            SimulationService simulationService)
        {
            _authService = authService;
            _unitService = unitService;
            _simulationService = simulationService;
        }

        public string CurrentUserDescription =>
            _authService.CurrentUser == null
                ? "Пользователь не авторизован"
                : $"Пользователь: {_authService.CurrentUser.Email}";
    }
}
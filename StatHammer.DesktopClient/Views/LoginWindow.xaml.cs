using System.Windows;
using StatHammer.DesktopClient.Services;
using StatHammer.DesktopClient.ViewModels;

namespace StatHammer.DesktopClient.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly UnitService _unitService;
        private readonly SimulationService _simulationService;

        private readonly LoginViewModel _viewModel;

        public LoginWindow(
            AuthService authService,
            UnitService unitService,
            SimulationService simulationService)
        {
            InitializeComponent();

            _authService = authService;
            _unitService = unitService;
            _simulationService = simulationService;

            _viewModel = new LoginViewModel(authService);
            _viewModel.LoginSucceeded += OnLoginSucceeded;

            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.LoginSucceeded -= OnLoginSucceeded;

            base.OnClosed(e);
        }

        private void OnLoginSucceeded(object? sender, EventArgs e)
        {
            var mainWindow = new MainWindow(
                _authService,
                _unitService,
                _simulationService);

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();
            Close();
        }
    }
}
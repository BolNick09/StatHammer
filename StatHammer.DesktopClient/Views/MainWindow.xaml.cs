using System.Windows;
using StatHammer.DesktopClient.Services;
using StatHammer.DesktopClient.ViewModels;

namespace StatHammer.DesktopClient.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(
            AuthService authService,
            UnitService unitService,
            SimulationService simulationService)
        {
            InitializeComponent();

            _viewModel = new MainViewModel(
                authService,
                unitService,
                simulationService);

            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        protected override void OnClosed(EventArgs e)
        {
            Loaded -= OnLoaded;

            base.OnClosed(e);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            await _viewModel.InitializeAsync();
        }
    }
}
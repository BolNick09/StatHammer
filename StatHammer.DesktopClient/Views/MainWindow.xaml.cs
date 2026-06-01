using System.Windows;
using StatHammer.DesktopClient.Services;
using StatHammer.DesktopClient.ViewModels;

namespace StatHammer.DesktopClient.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(
            AuthService authService,
            UnitService unitService,
            SimulationService simulationService)
        {
            InitializeComponent();

            DataContext = new MainViewModel(
                authService,
                unitService,
                simulationService);
        }
    }
}
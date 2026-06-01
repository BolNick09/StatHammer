using StatHammer.DesktopClient.Commands;
using StatHammer.DesktopClient.Models.Simulations;
using StatHammer.DesktopClient.Models.Units;
using StatHammer.DesktopClient.Services;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace StatHammer.DesktopClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly UnitService _unitService;
        private readonly SimulationService _simulationService;

        private UnitListItemDto? _selectedUnitA;
        private UnitListItemDto? _selectedUnitB;

        private int _simulationCount = 1000;
        private int _maxTurns = 5;
        private bool _useParallel = true;
        private int _maxDegreeOfParallelism = Environment.ProcessorCount;
        private bool _saveResult = true;

        private bool _isBusy;
        private string? _statusMessage;
        private string? _errorMessage;

        private RunSimulationResponseDto? _lastResponse;

        public MainViewModel(
            AuthService authService,
            UnitService unitService,
            SimulationService simulationService)
        {
            _authService = authService;
            _unitService = unitService;
            _simulationService = simulationService;

            RunSimulationCommand = new AsyncRelayCommand(
                RunSimulationAsync,
                CanRunSimulation);
        }

        public ObservableCollection<UnitListItemDto> Units { get; } = new();

        public ObservableCollection<TurnSummaryRowViewModel> TurnRows { get; } = new();

        public ObservableCollection<WeaponSummaryRowViewModel> WeaponRows { get; } = new();

        public string CurrentUserDescription =>
            _authService.CurrentUser == null
                ? "Пользователь не авторизован"
                : $"Пользователь: {_authService.CurrentUser.Email}";

        public UnitListItemDto? SelectedUnitA
        {
            get => _selectedUnitA;
            set
            {
                if (SetProperty(ref _selectedUnitA, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public UnitListItemDto? SelectedUnitB
        {
            get => _selectedUnitB;
            set
            {
                if (SetProperty(ref _selectedUnitB, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public int SimulationCount
        {
            get => _simulationCount;
            set
            {
                if (SetProperty(ref _simulationCount, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public int MaxTurns
        {
            get => _maxTurns;
            set
            {
                if (SetProperty(ref _maxTurns, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool UseParallel
        {
            get => _useParallel;
            set => SetProperty(ref _useParallel, value);
        }

        public int MaxDegreeOfParallelism
        {
            get => _maxDegreeOfParallelism;
            set
            {
                if (SetProperty(ref _maxDegreeOfParallelism, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool SaveResult
        {
            get => _saveResult;
            set => SetProperty(ref _saveResult, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RunSimulationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string? StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public RunSimulationResponseDto? LastResponse
        {
            get => _lastResponse;
            private set
            {
                if (SetProperty(ref _lastResponse, value))
                {
                    OnPropertyChanged(nameof(HasResult));
                    OnPropertyChanged(nameof(UnitAWins));
                    OnPropertyChanged(nameof(UnitBWins));
                    OnPropertyChanged(nameof(Draws));
                    OnPropertyChanged(nameof(AverageCompletedTurns));
                    OnPropertyChanged(nameof(AverageUnitAFinalAliveModels));
                    OnPropertyChanged(nameof(AverageUnitBFinalAliveModels));
                    OnPropertyChanged(nameof(SavedResultDescription));
                }
            }
        }

        public bool HasResult => LastResponse != null;

        public int UnitAWins => LastResponse?.Result.UnitAWins ?? 0;

        public int UnitBWins => LastResponse?.Result.UnitBWins ?? 0;

        public int Draws => LastResponse?.Result.Draws ?? 0;

        public double AverageCompletedTurns =>
            LastResponse?.Result.AverageCompletedTurns ?? 0;

        public double AverageUnitAFinalAliveModels =>
            LastResponse?.Result.AverageUnitAFinalAliveModels ?? 0;

        public double AverageUnitBFinalAliveModels =>
            LastResponse?.Result.AverageUnitBFinalAliveModels ?? 0;

        public string SavedResultDescription =>
            LastResponse?.SavedSimulationResultId is int savedId
                ? $"Результат сохранён в БД. Id: {savedId}"
                : "Результат не сохранялся в БД.";

        public AsyncRelayCommand RunSimulationCommand { get; }

        public async Task InitializeAsync()
        {
            ErrorMessage = null;
            StatusMessage = "Загрузка списка юнитов...";

            try
            {
                var units = await _unitService.GetUnitsAsync();

                Units.Clear();

                foreach (var unit in units.OrderBy(u => u.Name))
                {
                    Units.Add(unit);
                }

                if (Units.Count > 0)
                {
                    SelectedUnitA = Units[0];
                }

                if (Units.Count > 1)
                {
                    SelectedUnitB = Units[1];
                }

                StatusMessage = $"Загружено юнитов: {Units.Count}.";
            }
            catch (HttpRequestException)
            {
                ErrorMessage =
                    "Не удалось загрузить список юнитов. " +
                    "Проверьте соединение с сервером.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки юнитов: {ex.Message}";
            }
        }

        private bool CanRunSimulation()
        {
            return !IsBusy
                   && SelectedUnitA != null
                   && SelectedUnitB != null
                   && SelectedUnitA.Id != SelectedUnitB.Id
                   && SimulationCount > 0
                   && MaxTurns is >= 1 and <= 5
                   && MaxDegreeOfParallelism > 0;
        }

        private async Task RunSimulationAsync()
        {
            if (!CanRunSimulation())
            {
                return;
            }

            ErrorMessage = null;
            StatusMessage = "Выполняется расчёт...";
            IsBusy = true;

            try
            {
                var request = new RunSimulationRequestDto
                {
                    UnitAId = SelectedUnitA!.Id,
                    UnitBId = SelectedUnitB!.Id,
                    SimulationCount = SimulationCount,
                    MaxTurns = MaxTurns,
                    UseParallel = UseParallel,
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                    SaveResult = SaveResult
                };

                LastResponse = await _simulationService.RunSimulationAsync(request);

                FillTurnRows(LastResponse.Result);
                FillWeaponRows(LastResponse.Result);

                StatusMessage = "Расчёт завершён.";
            }
            catch (HttpRequestException)
            {
                ErrorMessage =
                    "Не удалось подключиться к серверу во время расчёта.";
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Непредвиденная ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FillTurnRows(BattleSimulationBatchResultDto result)
        {
            TurnRows.Clear();

            foreach (var turn in result.Turns.OrderBy(t => t.TurnNumber))
            {
                if (turn.SideA != null)
                {
                    TurnRows.Add(CreateTurnRow(turn.TurnNumber, turn.SideA));
                }

                if (turn.SideB != null)
                {
                    TurnRows.Add(CreateTurnRow(turn.TurnNumber, turn.SideB));
                }
            }
        }

        private void FillWeaponRows(BattleSimulationBatchResultDto result)
        {
            WeaponRows.Clear();

            foreach (var turn in result.Turns.OrderBy(t => t.TurnNumber))
            {
                AddWeaponRows(turn.TurnNumber, turn.SideA);
                AddWeaponRows(turn.TurnNumber, turn.SideB);
            }
        }

        private void AddWeaponRows(
            int turnNumber,
            AverageSideTurnStatDto? sideStat)
        {
            if (sideStat == null)
            {
                return;
            }

            foreach (var weapon in sideStat.WeaponStats)
            {
                WeaponRows.Add(new WeaponSummaryRowViewModel
                {
                    TurnNumber = turnNumber,
                    Side = sideStat.Side,
                    WeaponName = weapon.WeaponName,
                    ProfileName = weapon.WeaponProfileName ?? "—",
                    AverageCount = weapon.AverageCount,
                    AverageAttacks = weapon.AverageAttacks,
                    AverageHits = weapon.AverageHits,
                    AverageWounds = weapon.AverageWounds,
                    AverageFinalDamage = weapon.AverageFinalDamage
                });
            }
        }

        private static TurnSummaryRowViewModel CreateTurnRow(
            int turnNumber,
            AverageSideTurnStatDto sideStat)
        {
            return new TurnSummaryRowViewModel
            {
                TurnNumber = turnNumber,
                Side = sideStat.Side,
                AverageAttacks = sideStat.AverageAttacks,
                AverageHits = sideStat.AverageHits,
                AverageWounds = sideStat.AverageWounds,
                AverageSuccessfulSaves = sideStat.AverageSuccessfulSaves,
                AverageFinalDamage = sideStat.AverageFinalDamage,
                AverageModelsKilled = sideStat.AverageModelsKilled
            };
        }
    }
}
using StatHammer.DesktopClient.Commands;
using StatHammer.DesktopClient.Services;
using System.Net.Http;

namespace StatHammer.DesktopClient.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string? _errorMessage;
        private bool _isBusy;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;

            LoginCommand = new AsyncRelayCommand(
                LoginAsync,
                CanLogin);
        }

        public event EventHandler? LoginSucceeded;

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public AsyncRelayCommand LoginCommand { get; }

        private bool CanLogin()
        {
            return !IsBusy
                   && !string.IsNullOrWhiteSpace(Email)
                   && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            ErrorMessage = null;
            IsBusy = true;

            try
            {
                await _authService.LoginAsync(Email.Trim(), Password);

                Password = string.Empty;

                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (HttpRequestException)
            {
                ErrorMessage =
                    "Не удалось подключиться к серверу. " +
                    "Проверьте, что StatHammer.Server запущен.";
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
    }
}
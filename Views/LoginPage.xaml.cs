using ReserveFlow.Core.ViewModels;

namespace reserve_flow_ai_2026;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;

    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = _vm;

        SignInBtn.Clicked += async (_, __) =>
        {
            _vm.Username = UsernameEntry.Text ?? string.Empty;
            _vm.Password = PasswordEntry.Text ?? string.Empty;
            var ok = await _vm.LoginAsync();
            if (ok)
                await Shell.Current.GoToAsync("//dashboard");
            else
                await DisplayAlert("Login", "Invalid credentials", "OK");
        };
    }
}

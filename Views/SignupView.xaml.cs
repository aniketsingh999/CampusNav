using CampusNav.ViewModels;

namespace CampusNav.Views;

public partial class SignupView : ContentPage
{
    SignupViewModel vm;

    public SignupView()
	{
		InitializeComponent();
        vm = new SignupViewModel();
		BindingContext = vm;
	}

    protected override bool OnBackButtonPressed()
    {
        Task<bool> answer = DisplayAlert("Exit", "Do you want to quit?", "Yes", "No");
        answer.ContinueWith(task =>
        {
            if (task.Result)
            {
                Application.Current.Quit();
            }
        });
        return true;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage");
    }
}
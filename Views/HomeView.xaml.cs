using CampusNav.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace CampusNav.Views;

public partial class HomeView : ContentPage
{
    HomeViewModel vm;
	public HomeView(HomeViewModel homeViewModel)
	{
		InitializeComponent();
        vm = homeViewModel;
        BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        await vm.AddUser();
        await vm.CheckLocation(map);
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

    async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await vm.HandleMapClick(map, e.Location.Latitude, e.Location.Longitude);
        // System.Diagnostics.Debug.WriteLine($"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");
    }

    async void OnDirectionsClicked(object sender, EventArgs e)
    {
        await vm.ShowDirections(map);
    }
}
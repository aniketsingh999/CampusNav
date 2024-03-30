using CampusNav.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms.Sync;

namespace CampusNav.ViewModels
{
    public partial class SignupViewModel : BaseViewModel

    {
        public SignupViewModel()
        {
            EmailText = "test@gmail.com";
            PasswordText = "test123";
            Name = "Test_user";
        }

        [ObservableProperty]
        string emailText;

        [ObservableProperty]
        string passwordText;

        [ObservableProperty]
        string name;


        [RelayCommand]
        public async void CreateAccount()
        {
            try
            {
                await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(EmailText, PasswordText);
                await Login();

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error creating account!", "Error: " + ex.Message, "Close");
            }
        }

        [RelayCommand]
        public async Task Login()
        {
            try
            {
                var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(EmailText, PasswordText));

                if (user != null)
                {
                    await Shell.Current.GoToAsync("///HomePage", 
                        new Dictionary<string, object>()
                        {
                            { "Data", new UserInfo{ 
                                       Name = Name,
                                       Email = EmailText,
                                       Password = PasswordText,
                            }
                           }
                        });
                    EmailText = "";
                    PasswordText = "";
                    Name = "";
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Login unsuccessful!", ex.Message, "Close");
            }

        }

    }
}

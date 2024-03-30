using CampusNav.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms.Sync;

namespace CampusNav.ViewModels
{
    public partial class LoginViewModel : BaseViewModel

    {
        public LoginViewModel()
        {
            EmailText = "test@gmail.com";
            PasswordText = "test123";
        }

        [ObservableProperty]
        string emailText;

        [ObservableProperty]
        string passwordText;
        

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
                                       
                                       Email = EmailText,
                                       Password = PasswordText,
                            }
                           }
                        });
                    EmailText = "";
                    PasswordText = "";
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

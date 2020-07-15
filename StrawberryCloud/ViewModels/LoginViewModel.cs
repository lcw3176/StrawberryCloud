using StrawberryCloud.Commands;
using StrawberryCloud.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StrawberryCloud.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public ICommand loginCommand { get; set; }
        private LoginModel model;

        public string userId
        {
            get { return model.UserId; }
            set
            {
                model.UserId = value;
                OnPropertyChanged("userId");
            }
        }

        public LoginViewModel()
        {
            ChangeViewModel(this);
            model = new LoginModel();
            model.failed += LoginFailed;
            model.success += LoginSuccess;
            loginCommand = new RelayCommand(LoginExecuteMethod);
        }

        private void LoginFailed()
        {
            MessageBox.Show("실패함");
        }

        private void LoginSuccess()
        {
            Release();
            HomeViewModel home = new HomeViewModel();
            ChangeViewModel(home);
        }

        private void LoginExecuteMethod(object param)
        {
            PasswordBox passwordBox = param as PasswordBox;
            string password = passwordBox.Password;

            model.TryLogin(password);
        }

        private void Release()
        {
            model.failed -= LoginFailed;
            model.success -= LoginSuccess;
        }

    }
}

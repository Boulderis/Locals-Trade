﻿using System;
using MSupportYourLocals.Services;
using MSupportYourLocals.ViewModels;
using MSupportYourLocals.Views.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSupportYourLocals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPanelView : TopBarView
    {
        private JsonWebTokenHolder tokenService = DependencyService.Get<JsonWebTokenHolder>();

        public UserPanelView(UserViewModel userViewModel)
        {
            InitializeComponent();
            BindingContext = userViewModel;
            if (tokenService.Token != null)
            {
                LoginOptions.IsVisible = false;
                Settings.IsVisible = true;
            }
            else
            {
                LoginOptions.IsVisible = true;
                Settings.IsVisible = false;
            }
        }

        public async void Login(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        public async void SignUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterUserView());
        }

        public async void ChangeEmail(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangeEmailView());
        }

        public async void ChangePassword(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangePasswordView());
        }

        public async void Logout(object sender, EventArgs e)
        {
            tokenService.Logout();
            await Navigation.PushAsync(new MainPage());
        }

    }
}

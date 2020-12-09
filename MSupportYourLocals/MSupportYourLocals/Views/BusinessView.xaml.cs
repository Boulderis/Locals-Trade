﻿using MSupportYourLocals.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSupportYourLocals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusinessView : ContentPage
    {
        public BusinessView(BusinessViewModel businessesViewModel)
        {
            InitializeComponent();
            BindingContext = businessesViewModel;
        }
    }
}
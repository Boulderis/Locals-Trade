﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MSupportYourLocals.Infrastructure;
using MSupportYourLocals.Models;
using MSupportYourLocals.Services;
using MSupportYourLocals.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSupportYourLocals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedbackView : ContentPage
    {

        private IFeedbackService feedbackService = DependencyService.Get<IFeedbackService>();
        private Feedback chosenFeedback;
        private long businessId;
        private ActionEnum action;
        private FeedbackViewModel feedbackViewModel;

        public FeedbackView(long businessId, FeedbackViewModel feedbackViewModel)
        {
            InitializeComponent();
            BindingContext = feedbackViewModel;
            this.businessId = businessId;
            this.feedbackViewModel = feedbackViewModel;
        }

        public void DeleteAll(object sender, EventArgs e)
        {
            Confirmation.IsVisible = true;
            action = ActionEnum.DeleteAll;
        }

        public async Task ConfirmedDeleteAll()
        {
            Confirmation.IsVisible = false;
            bool success = await feedbackService.DeleteAllFeedbacks(businessId);
            if (success)
            {
                feedbackViewModel?.Feedbacks.Clear();
                FeedbackList.ItemsSource = feedbackViewModel?.Feedbacks;
                await this.DisplaySuccess("Every single feedback successfully deleted");
            }
            else
            {
                await this.DisplayFailure();
            }
        }

        public async Task ConfirmedDeleteOne()
        {
            Confirmation.IsVisible = false;
            bool success = await feedbackService.DeleteFeedback(chosenFeedback.ID);
            if (success)
            {
                feedbackViewModel?.Feedbacks.Remove(chosenFeedback);
                FeedbackList.ItemsSource = feedbackViewModel?.Feedbacks;
                await this.DisplaySuccess("Feedback successfully deleted");
            }
            else
            {
                await this.DisplayFailure();
            }
        }

        public async void Cancel(object sender, EventArgs e)
        {
            Controls.IsVisible = false;
        }

        public async void Delete(object sender, EventArgs e)
        {
            Confirmation.IsVisible = true;
            action = ActionEnum.Delete;
        }

        public async void ConfirmDelete(object sender, EventArgs e)
        {
            Confirmation.IsVisible = false;
            if (action == ActionEnum.Delete)
            {
                await ConfirmedDeleteOne();
            }
            else if (action == ActionEnum.DeleteAll)
            {
                await ConfirmedDeleteAll();
            }
        }

        public void CancelDelete(object sender, EventArgs e)
        {
            Confirmation.IsVisible = false;
        }

        public void FeedbackSelected(object sender, SelectionChangedEventArgs e)
        {
            object chosen = e.CurrentSelection.FirstOrDefault();
            if (chosen is Feedback)
            {
                chosenFeedback = chosen as Feedback;
                Controls.IsVisible = true;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using LocalsTradeBot.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace LocalsTradeBot.Bots

{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    public class EchoBot : ActivityHandler
    {
        private readonly BotState _conversationState;
        private readonly LocalAppClient _client;

        public EchoBot(ConversationState conversationState)
        {
            _conversationState = conversationState;
            _client = new LocalAppClient();
        }

        public class Conversation
        {

            public string Email { get; set; }
            public string Question { get; set; }
            public CurrentConversationState State { get; set; } = CurrentConversationState.Question;

            public void reset()
            {
                State = CurrentConversationState.Question;
                Email = null;
                Question = null;
            }

            public enum CurrentConversationState
            {
                Question,
                Email
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var conversationStateAccessors = _conversationState.CreateProperty<Conversation>(nameof(Conversation));
            var conversation = await conversationStateAccessors.GetAsync(turnContext, () => new Conversation(), cancellationToken);

            await FillOutUserProfileAsync(conversation, turnContext, cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private async Task FillOutUserProfileAsync(Conversation conversation, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var input = turnContext.Activity.Text?.Trim();
            string message;

            switch (conversation.State)
            {
                case Conversation.CurrentConversationState.Question:
                    if(ValidateQuestion(input, out var question, out message))
                    {
                        var responseMessage = "Thank you for your question. The question will be answered by administrator as soon as possible. " +
                       "Please enter an email address to whom we will send a response";
                        await turnContext.SendActivityAsync(responseMessage, null, null, cancellationToken);
                        conversation.State = Conversation.CurrentConversationState.Email;
                        conversation.Question = question;
                        break;
                    }
                    else 
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
                case Conversation.CurrentConversationState.Email:
                    if (ValidateEmail(input, out var email, out message))
                    {
                        await turnContext.SendActivityAsync("Thank you! We will send response to given email in 5 business days. Have a nice day!", null, null, cancellationToken);
                        await _client.CreateQuestion(email, conversation.Question);
                        conversation.reset();
                        await turnContext.SendActivityAsync("Do you have any more questions?", null, null, cancellationToken);
                        break;
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
                        break;
                    }
            }
        }
        private static bool ValidateQuestion(string input, out string question, out string message)
        {
            question = null;
            message = null;

            if (input.Trim().Length == 0)
            {
                message = "Please enter not empty question";
            }
            else
            {
                question = input.Trim();
            }

            return message is null;
        }

        private static bool ValidateEmail(string input, out string email, out string message)
        {
            email = null;
            message = null;

            if (!new EmailAddressAttribute().IsValid(input))
            {
  
                message = "Please enter a correct email";
            }
            else
            {
                email = input.Trim();
            }

            return message is null;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Hello! Do you have any questions you Want to ask?"), cancellationToken);
                }
            }
        }
    }
}

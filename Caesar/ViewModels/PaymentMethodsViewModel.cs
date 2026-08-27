// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Extensions;
using Caesar.Messages;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class PaymentMethodsViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        [ObservableProperty]
        public partial ObservableCollection<PaymentMethod> PaymentMethods { get; set; }

        public PaymentMethodsViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            PaymentMethods = [];
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            ICollection<PaymentMethod> paymentMethods = await caesarDatabaseService.GetEntitiesAsync(c => c.PaymentMethods);

            PaymentMethods.Clear();

            foreach (
                PaymentMethod paymentMethod in
                paymentMethods
                .OrderBy(x => x.Name)
                .ToList())
            {
                await caesarDatabaseService.LoadRelatedEntitiesAsync(paymentMethod, e => e.Items);
                PaymentMethods.Add(paymentMethod);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddAsync()
        {
            await caesarDatabaseService.AddEntityAsync(new PaymentMethod());
            await LoadAsync();
        }

        private static bool CanInvokeOrRemove(PaymentMethod? paymentMethod)
        {
            return paymentMethod is not null;
        }

        [RelayCommand(CanExecute = nameof(CanInvokeOrRemove))]
        private void Detail(PaymentMethod? paymentMethod)
        {
            if (paymentMethod is null)
                return;

            WeakReferenceMessenger.Default.Send(new PaymentMethodInvokedMessage(paymentMethod));
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanInvokeOrRemove))]
        private async Task RemoveAsync(PaymentMethod? paymentMethod)
        {
            if (paymentMethod is null)
                return;

            await caesarDatabaseService.RemoveEntityAsync(paymentMethod);
            await LoadAsync();
        }

        private static bool CanSearch(string searchPhrase)
        {
            return !string.IsNullOrWhiteSpace(searchPhrase);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
        private async Task SearchAsync(string searchPhrase)
        {
            if (!string.IsNullOrWhiteSpace(searchPhrase))
            {
                float[] vector = await embeddingVectorService.GenerateVectorForSearchQueryAsync(
                    searchPhrase,
                    new()
                    {
                        Dimensions = Constants.DIMENSIONS,
                    });

                List<PaymentMethod> list = PaymentMethods
                    .OrderByDescending(i => i.Vector * vector)
                    .ToList();

                PaymentMethods.Clear();

                foreach (
                    PaymentMethod paymentMethod in list)
                    PaymentMethods.Add(paymentMethod);
            }
        }
    }
}

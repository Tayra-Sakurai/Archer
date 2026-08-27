// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Messages;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class PaymentMethodViewModel : ObservableValidator
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;
        private PaymentMethod paymentMethod;

        public PaymentMethodViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            paymentMethod = new();
            ErrorsChanged += PaymentMethodViewModel_ErrorsChanged;
        }

        private void PaymentMethodViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        public double Remainder => paymentMethod.Remainder;

        [Required]
        public string Name
        {
            get => paymentMethod.Name;
            set => SetProperty(paymentMethod.Name, value, paymentMethod, (m, v) => m.Name = v, true);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            await caesarDatabaseService.LoadRelatedEntitiesAsync(paymentMethod, e => e.Items);
            OnPropertyChanged(nameof(Remainder));
        }

        public async Task LoadExistingValueAsync(PaymentMethod paymentMethod)
        {
            this.paymentMethod = paymentMethod;

            await LoadAsync();

            OnPropertyChanged(nameof(Name));
            ValidateAllProperties();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            paymentMethod.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                paymentMethod.Name,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });

            await caesarDatabaseService.UpdateEntityAsync(paymentMethod);
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            await caesarDatabaseService.RemoveEntityAsync(paymentMethod);

            WeakReferenceMessenger.Default.Send(new PaymentMethodRemovedMessage(paymentMethod));
        }
    }
}

// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Messages;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class ItemViewModel : ObservableValidator
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;
        private Item item;

        [ObservableProperty]
        public partial ObservableCollection<SmallCategory> SmallCategories { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<PaymentMethod> PaymentMethods { get; set; }

        public ItemViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            item = new();
            SmallCategories = [];
            PaymentMethods = [];
            ErrorsChanged += ItemViewModel_ErrorsChanged;
        }

        private void ItemViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        public async Task LoadExistingValueAsync(Item item)
        {
            this.item = item;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(SmallCategory));
            OnPropertyChanged(nameof(PaymentMethod));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Time));
            OnPropertyChanged(nameof(Expense));
            OnPropertyChanged(nameof(Income));

            ValidateAllProperties();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            SmallCategories.Clear();
            PaymentMethods.Clear();

            foreach (
                SmallCategory smallCategory in
                (await caesarDatabaseService.GetEntitiesAsync<SmallCategory>())
                .OrderBy(e => e.MediumCategoryId)
                .ToList())
                SmallCategories.Add(smallCategory);

            foreach (
                PaymentMethod paymentMethod in
                (await caesarDatabaseService.GetEntitiesAsync(c => c.PaymentMethods))
                .OrderBy(e => e.Id)
                .ToList())
                PaymentMethods.Add(paymentMethod);

            OnPropertyChanged(nameof(SmallCategory));
            OnPropertyChanged(nameof(PaymentMethod));
            ValidateAllProperties();
        }

        [Required]
        public string Name
        {
            get => item.Name;
            set
            {
                if (SetProperty(item.Name, value, item, (m, v) => m.Name = v, true))
                    ValidateAllProperties();
            }
        }

        [Required]
        public DateTimeOffset Date
        {
            get => new(item.TimeTrade.Date, DateTimeOffset.Now.Offset);
            set
            {
                if (SetProperty(item.TimeTrade.Date, value.Date, item, SetDateValue, true))
                    ValidateAllProperties();
            }
        }

        private static void SetDateValue(Item model, DateTime value)
        {
            TimeSpan dateTimeOffset = model.TimeTrade.TimeOfDay;
            model.TimeTrade = new(value.Add(dateTimeOffset), DateTimeOffset.Now.Offset);
        }

        public TimeSpan Time
        {
            get => item.TimeTrade.TimeOfDay;
            set
            {
                if (SetProperty(item.TimeTrade.TimeOfDay, value, item, SetTimeValue, true))
                    ValidateAllProperties();
            }
        }

        private static void SetTimeValue(Item model, TimeSpan value)
        {
            model.TimeTrade = new(model.TimeTrade.Date.Add(value), DateTimeOffset.Now.Offset); ;
        }

        [Required]
        [Range(0, double.MaxValue)]
        public double Expense
        {
            get => item.Expense;
            set
            {
                if (SetProperty(item.Expense, value, item, (m, v) => m.Expense = v, true))
                    ValidateAllProperties();
            }
        }

        [Required, Range(0, double.MaxValue)]
        public double Income
        {
            get => item.Income;
            set
            {
                if (SetProperty(item.Income, value, item, (m, v) => m.Income = v, true))
                    ValidateAllProperties();
            }
        }

        public string? Description
        {
            get => item.Description;
            set => SetProperty(item.Description, value, item, (m, v) => m.Description = v, true);
        }

        [Required]
        public PaymentMethod? PaymentMethod
        {
            get => PaymentMethods.FirstOrDefault(i => i.Id == item.PaymentMethodId);
            set
            {
                ValidateProperty(value);

                if (value is not null)
                    SetProperty(item.PaymentMethodId, value.Id, item, (m, v) => m.PaymentMethodId = v, false);
            }
        }

        [Required]
        public SmallCategory? SmallCategory
        {
            get => SmallCategories.FirstOrDefault(i => i.Id == item.SmallCategoryId);
            set
            {
                ValidateProperty(value);

                if (value is not null)
                    SetProperty(item.SmallCategoryId, value.Id, item, (m, v) => m.SmallCategoryId = v, false);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            EmbeddingGenerationOptions options = new()
            {
                Dimensions = Constants.DIMENSIONS,
            };

            if (!string.IsNullOrWhiteSpace(item.Description))
                item.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                    item.Name,
                    item.Description,
                    options);
            else
                item.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                    item.Name,
                    options);

            await caesarDatabaseService.UpdateEntityAsync(item);
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            await caesarDatabaseService.RemoveEntityAsync(item);

            WeakReferenceMessenger.Default.Send(new ItemRemovedMessage(item));
        }
    }
}

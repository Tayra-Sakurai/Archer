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
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        public ItemsViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            Items = [];
            SmallCategories = [];
            PaymentMethods = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<Item> Items { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        public partial ObservableCollection<SmallCategory> SmallCategories { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        public partial ObservableCollection<PaymentMethod> PaymentMethods { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            Items.Clear();
            SmallCategories.Clear();
            PaymentMethods.Clear();

            List<Item> items = [];

            foreach (
                Item item in
                await caesarDatabaseService.GetEntitiesAsync(c => c.Items))
            {
                await caesarDatabaseService.LoadRelatedEntityAsync(item, i => i.SmallCategory);
                if (item.SmallCategory is not null)
                {
                    await caesarDatabaseService.LoadRelatedEntityAsync(item.SmallCategory, s => s.MediumCategory);
                    if (item.SmallCategory.MediumCategory is not null)
                        await caesarDatabaseService.LoadRelatedEntityAsync(item.SmallCategory.MediumCategory, m => m.LargeCategory);
                }

                items.Add(item);
            }

            foreach (
                Item item1 in
                items
                .OrderBy(i => i.TimeTrade))
                Items.Add(item1);

            foreach (
                SmallCategory smallCategory in
                (await caesarDatabaseService.GetEntitiesAsync(e => e.SmallCategories))
                .OrderBy(e => e.MediumCategoryId)
                .ThenBy(e => e.Name)
                .ToArray())
                SmallCategories.Add(smallCategory);

            foreach (
                PaymentMethod paymentMethod in
                (await caesarDatabaseService.GetEntitiesAsync(c => c.PaymentMethods))
                .OrderBy(e => e.Name)
                .ThenBy(e => e.Id)
                .ToArray())
                PaymentMethods.Add(paymentMethod);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAdd))]
        private async Task AddAsync()
        {
            Item item = new()
            {
                SmallCategoryId = SmallCategories.First().Id,
                PaymentMethodId = PaymentMethods.First().Id,
            };

            await caesarDatabaseService.AddEntityAsync(item);
        }

        private bool CanAdd()
        {
            return SmallCategories.Any() && PaymentMethods.Any();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync(Item? item)
        {
            if (item == null) return;

            await caesarDatabaseService.RemoveEntityAsync(item);
        }

        private static bool CanRemove(Item? item)
        {
            return item is not null;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
        private async Task SearchAsync(string searchPhrase)
        {
            if (string.IsNullOrWhiteSpace(searchPhrase))
                return;

            float[] value = await embeddingVectorService.GenerateVectorForSearchQueryAsync(
                searchPhrase,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });

            List<Item> items = Items
                .OrderByDescending(i => i.Vector * value)
                .ToList();

            Items.Clear();

            foreach (Item item in items)
                Items.Add(item);
        }

        private static bool CanSearch(string searchPhrase)
        {
            return !string.IsNullOrWhiteSpace(searchPhrase);
        }

        [RelayCommand(CanExecute = nameof(CanFilter))]
        private void Filter(SmallCategory? smallCategory)
        {
            if (smallCategory == null)
                return;

            Item[] items = Items
                .Where(i => i.SmallCategoryId == smallCategory.Id)
                .ToArray();

            Items.Clear();

            foreach (Item item in items)
                Items.Add(item);
        }

        private static bool CanFilter(SmallCategory? smallCategory)
        {
            return smallCategory is not null;
        }

        [RelayCommand(CanExecute = nameof(CanFilterByPaymentMethod))]
        private void FilterByPaymentMethod(PaymentMethod? paymentMethod)
        {
            if (paymentMethod == null)
                return;

            List<Item> items = [.. Items];

            Items.Clear();

            foreach (
                Item item in
                items.Where(i => i.PaymentMethodId == paymentMethod.Id))
                Items.Add(item);
        }

        private static bool CanFilterByPaymentMethod(PaymentMethod? paymentMethod)
        {
            return paymentMethod is not null;
        }

        [RelayCommand(CanExecute = nameof(CanOpenDetail))]
        private void Detail(Item? item)
        {
            if (item == null)
                return;

            WeakReferenceMessenger.Default.Send(new ItemInvokedMessage(item));
        }

        private static bool CanOpenDetail(Item? item)
        {
            return item is not null;
        }
    }
}

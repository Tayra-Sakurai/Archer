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
    public partial class SmallCategoriesViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        public SmallCategoriesViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            SmallCategories = [];
            this.embeddingVectorService = embeddingVectorService;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        public partial ObservableCollection<SmallCategory> SmallCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            SmallCategories.Clear();

            foreach (
                SmallCategory category in
                (await caesarDatabaseService.GetEntitiesAsync(e => e.SmallCategories))
                .OrderBy(e => e.MediumCategoryId)
                .ToList())
            {
                await caesarDatabaseService.LoadRelatedEntitiesAsync(category, c => c.Items);
                await caesarDatabaseService.LoadRelatedEntityAsync(category, c => c.MediumCategory);
                SmallCategories.Add(category);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAdd))]
        private async Task AddAsync()
        {
            ICollection<MediumCategory> mediumCategories = await caesarDatabaseService.GetEntitiesAsync(c => c.MediumCategories);

            SmallCategory smallCategory = new()
            {
                Vector = new float[Constants.DIMENSIONS],
                MediumCategoryId = mediumCategories.First().Id,
            };

            await caesarDatabaseService.AddEntityAsync(smallCategory);
            await LoadAsync();
        }

        private bool CanAdd()
        {
            return caesarDatabaseService.ExistsAnyEntity(context => context.MediumCategories);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync(SmallCategory? smallCategory)
        {
            if (smallCategory == null) return;

            await caesarDatabaseService.RemoveEntityAsync(smallCategory);
        }

        private static bool CanRemove(SmallCategory? smallCategory)
        {
            return smallCategory is not null;
        }

        [RelayCommand(CanExecute = nameof(CanRemove))]
        private static void Detail(SmallCategory? smallCategory)
        {
            if (smallCategory is null) return;

            WeakReferenceMessenger.Default.Send(new SmallCategoryInvokedMessage(smallCategory));
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
        private async Task SearchAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return;

            float[] value = await embeddingVectorService.GenerateVectorForSearchQueryAsync(searchText, new() { Dimensions = Constants.DIMENSIONS });

            List<SmallCategory> smallCategories = [.. SmallCategories];
            SmallCategories.Clear();

            foreach (
                SmallCategory smallCategory in
                smallCategories
                .OrderByDescending(e => e.Vector * value)
                .ToList())
                SmallCategories.Add(smallCategory);
        }

        private static bool CanSearch(string searchText)
        {
            return !string.IsNullOrWhiteSpace(searchText);
        }
    }
}

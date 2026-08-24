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
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class LargeCategoriesViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> databaseService;
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        public LargeCategoriesViewModel(ICaesarDatabaseService<CaesarContext> databaseService, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.databaseService = databaseService;
            this.embeddingGenerator = embeddingGenerator;
            LargeCategories = [];
            SearchPhrase = string.Empty;
        }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        public partial string SearchPhrase { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            ICollection<LargeCategory> largeCategories = await databaseService.GetEntitiesAsync(c => c.LargeCategories);

            LargeCategories.Clear();

            foreach (LargeCategory largeCategory in largeCategories.OrderBy(e => e.Name).ToList())
                LargeCategories.Add(largeCategory);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddAsync()
        {
            LargeCategory largeCategory = new()
            {
                Vector = new float[Constants.DIMENSIONS],
            };

            await databaseService.AddEntityAsync(largeCategory);
            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync(LargeCategory? largeCategory)
        {
            if (largeCategory == null)
                return;

            await databaseService.RemoveEntityAsync(largeCategory);
            await LoadAsync();
        }

        private static bool CanRemove(LargeCategory? largeCategory)
        {
            return largeCategory is not null;
        }

        [RelayCommand(CanExecute = nameof(CanInvoke))]
        private void Invoke(LargeCategory? largeCategory)
        {
            if (largeCategory == null)
                return;

            WeakReferenceMessenger.Default.Send(new LargeCategoryInvokedMessage(largeCategory));
        }

        private static bool CanInvoke(LargeCategory? largeCategory)
        {
            return largeCategory is not null;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchPhrase)) return;

            ReadOnlyMemory<float> readOnlyMemory = await embeddingGenerator.GenerateVectorAsync(
                SearchPhrase,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });
            float[] values = readOnlyMemory.ToArray();

            List<LargeCategory> largeCategories = [.. LargeCategories];
            LargeCategories.Clear();

            largeCategories = largeCategories
                .OrderBy(e => e.Vector * values)
                .ToList();

            foreach (
                LargeCategory category in largeCategories)
                LargeCategories.Add(category);
        }

        private bool CanSearch()
        {
            return !string.IsNullOrWhiteSpace(SearchPhrase);
        }
    }
}

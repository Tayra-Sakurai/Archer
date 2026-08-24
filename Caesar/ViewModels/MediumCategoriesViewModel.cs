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
    public partial class MediumCategoriesViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        public MediumCategoriesViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingGenerator = embeddingGenerator;
            MediumCategories = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<MediumCategory> MediumCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            MediumCategories.Clear();

            foreach (
                MediumCategory mediumCategory in
                (await caesarDatabaseService.GetEntitiesAsync(c => c.MediumCategories))
                .OrderBy(e => e.LargeCategoryId)
                .ThenBy(e => e.Id)
                .ToArray())
            {
                await caesarDatabaseService.LoadRelatedEntityAsync(mediumCategory, e => e.LargeCategory);
                MediumCategories.Add(mediumCategory);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddAsync()
        {
            MediumCategory mediumCategory = new()
            {
                LargeCategoryId = (await caesarDatabaseService.GetEntitiesAsync(l => l.LargeCategories)).First().Id,
                Vector = new float[Constants.DIMENSIONS],
            };

            await caesarDatabaseService.AddEntityAsync(mediumCategory);
            await LoadAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRemove))]
        private async Task RemoveAsync(MediumCategory? mediumCategory)
        {
            if (mediumCategory == null)
                return;

            await caesarDatabaseService.RemoveEntityAsync(mediumCategory);
            await LoadAsync();
        }

        private static bool CanRemove(MediumCategory? mediumCategory)
        {
            return mediumCategory is not null;
        }

        [RelayCommand(CanExecute = nameof(CanRemove))]
        private static void Detail(MediumCategory? mediumCategory)
        {
            if (mediumCategory is null) return;

            WeakReferenceMessenger.Default.Send(new MediumCategoryInvokedMessage(mediumCategory));
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSearch))]
        private async Task SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return;

            float[] value = (await embeddingGenerator.GenerateVectorAsync(
                searchTerm,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                })).ToArray();

            List<MediumCategory> mediumCategories = [.. MediumCategories];

            MediumCategories.Clear();

            foreach (
                MediumCategory mediumCategory in
                mediumCategories
                .OrderBy(e => e.Vector * value)
                .ToList())
                MediumCategories.Add(mediumCategory);
        }

        private static bool CanSearch(string searchPhrase)
        {
            return !string.IsNullOrWhiteSpace(searchPhrase);
        }
    }
}

// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class MediumCategoryViewModel : ObservableValidator
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private MediumCategory mediumCategory;
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        public MediumCategoryViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingGenerator = embeddingGenerator;
            mediumCategory = new();
            LargeCategories = [];
        }

        public async Task SetExistingValueAsync(MediumCategory value)
        {
            mediumCategory = value;
        }

        public async Task LoadAsync()
        {
            LargeCategories.Clear();

            foreach (
                LargeCategory category in
                (await caesarDatabaseService.GetEntitiesAsync(c => c.LargeCategories))
                .OrderBy(e => e.Name)
                .ToArray())
                LargeCategories.Add(category);
        }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }

        [Required]
        public LargeCategory LargeCategory
        {
            get => LargeCategories.Single(e => mediumCategory.LargeCategoryId == e.Id);
            set
            {
                if (SetProperty(mediumCategory.LargeCategoryId, value.Id, mediumCategory, (m, v) => m.LargeCategoryId = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ReadOnlyMemory<float> readOnlyVector = await embeddingGenerator.GenerateVectorAsync(
                mediumCategory.Name,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });
            mediumCategory.Vector = readOnlyVector.ToArray();

            await caesarDatabaseService.UpdateEntityAsync(mediumCategory);
        }

        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }
}

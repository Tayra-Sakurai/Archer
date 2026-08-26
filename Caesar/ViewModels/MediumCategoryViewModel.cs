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
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        public MediumCategoryViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            mediumCategory = new();
            LargeCategories = [];
            ErrorsChanged += MediumCategoryViewModel_ErrorsChanged;
        }

        private void MediumCategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        public async Task SetExistingValueAsync(MediumCategory value)
        {
            mediumCategory = value;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(LargeCategory));
            ValidateAllProperties();
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
                    ValidateAllProperties();
                }
            }
        }

        [Required]
        public string Name
        {
            get => mediumCategory.Name;
            set
            {
                if (SetProperty(mediumCategory.Name, value, mediumCategory, (m, v) => m.Name = v, true))
                    ValidateAllProperties();
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            mediumCategory.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                mediumCategory.Name,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });

            await caesarDatabaseService.UpdateEntityAsync(mediumCategory);
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            await caesarDatabaseService.RemoveEntityAsync(mediumCategory);

            WeakReferenceMessenger.Default.Send(new MediumCategoryRemovedMessage(mediumCategory));
        }
    }
}

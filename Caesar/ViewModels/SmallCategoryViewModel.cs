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
    public partial class SmallCategoryViewModel : ObservableValidator
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;
        private SmallCategory smallCategory;

        public SmallCategoryViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            smallCategory = new();
            MediumCategories = [];
            ErrorsChanged += SmallCategoryViewModel_ErrorsChanged;
        }

        private void SmallCategoryViewModel_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        public partial ObservableCollection<MediumCategory> MediumCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            MediumCategories.Clear();

            foreach (
                MediumCategory mediumCategory in
                (await caesarDatabaseService.GetEntitiesAsync<MediumCategory>())
                .OrderBy(e => e.LargeCategoryId)
                .ThenBy(e => e.Name)
                .ToArray())
                MediumCategories.Add(mediumCategory);
        }

        public async Task LoadExistingValue(SmallCategory smallCategory)
        {
            this.smallCategory = smallCategory;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(MediumCategory));
            ValidateAllProperties();
        }

        [Required]
        public MediumCategory MediumCategory
        {
            get => MediumCategories.First(c => c.Id == smallCategory.MediumCategoryId);
            set
            {
                if (SetProperty(smallCategory.MediumCategoryId, value.Id, smallCategory, (m, v) => m.MediumCategoryId = v, true))
                {
                    ValidateAllProperties();
                }
            }
        }

        [Required]
        public string Name
        {
            get => smallCategory.Name;
            set
            {
                if (SetProperty(smallCategory.Name, value, smallCategory, (m, v) => m.Name = v, true))
                    ValidateAllProperties();
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            smallCategory.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                smallCategory.Name,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });

            await caesarDatabaseService.UpdateEntityAsync(smallCategory);
        }

        private bool CanSave()
        {
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            await caesarDatabaseService.RemoveEntityAsync(smallCategory);

            WeakReferenceMessenger.Default.Send(new SmallCategoryRemovedMessage(smallCategory));
        }
    }
}

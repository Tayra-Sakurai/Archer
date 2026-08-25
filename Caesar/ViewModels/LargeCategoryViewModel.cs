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
    public partial class LargeCategoryViewModel : ObservableValidator
    {
        private LargeCategory largeCategory;
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        public LargeCategoryViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            largeCategory = new();
        }

        public async Task LoadExistingLargeCategoryAsync(LargeCategory largeCategory)
        {
            this.largeCategory = largeCategory;

            OnPropertyChanged(nameof(Name));
            SaveCommand.NotifyCanExecuteChanged();
        }

        [Required]
        public string Name
        {
            get => largeCategory.Name;
            set
            {
                if (SetProperty(largeCategory.Name, value, largeCategory, (m, v) => m.Name = v, true))
                {
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            largeCategory.Vector = await embeddingVectorService.GenerateVectorForDocumentAsync(
                Name,
                new()
                {
                    Dimensions = Constants.DIMENSIONS,
                });
            await caesarDatabaseService.UpdateEntityAsync(largeCategory);
        }

        private bool CanSave()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RemoveAsync()
        {
            await caesarDatabaseService.RemoveEntityAsync(largeCategory);
            WeakReferenceMessenger.Default.Send(new LargeCategoryRemovedMessage(largeCategory));
        }
    }
}

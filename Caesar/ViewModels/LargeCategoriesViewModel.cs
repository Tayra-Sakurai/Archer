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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class LargeCategoriesViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> databaseService;

        public LargeCategoriesViewModel(ICaesarDatabaseService<CaesarContext> databaseService)
        {
            this.databaseService = databaseService;
            LargeCategories = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }

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
            LargeCategory largeCategory = new();

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
    }
}

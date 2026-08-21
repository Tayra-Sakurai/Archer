// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public MediumCategoriesViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
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
                .OrderBy(e => e.Id)
                .ToArray())
            {
                await caesarDatabaseService.LoadRelatedEntityAsync(mediumCategory, e => e.LargeCategory);
                MediumCategories.Add(mediumCategory);
            }
        }
    }
}

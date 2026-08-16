// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Caesar.ViewModels
{
    public partial class LargeCategoriesViewModel : ObservableObject
    {
        private readonly IDbContextFactory<CaesarContext> _contextFactory;

        public LargeCategoriesViewModel(IDbContextFactory<CaesarContext> contextFactory)
        {
            _contextFactory = contextFactory;
            LargeCategories = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<LargeCategory> LargeCategories { get; set; }
    }
}

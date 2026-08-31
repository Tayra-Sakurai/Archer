// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Archer
{
    public class ConstantsArcher
    {
        public static readonly ObservableCollection<PageInfo> Infos =
            [
                new()
                {
                    Name = "LargeCategoriesViewPageName",
                    PageType = typeof(LargeCategoriesViewPage),
                    Icon = new FontIcon
                    {
                        Glyph = "\uED44",
                    }
                },
            ];
    }
}

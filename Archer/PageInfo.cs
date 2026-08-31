// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Archer
{
    public class PageInfo
    {
        public required string Name { get; set; }
        public required Type PageType { get; set; }
        public IconElement? Icon { get; set; }
        public string DisplayName
        {
            get
            {
                ResourceLoader resourceLoader = new();
                return resourceLoader.GetString(Name);
            }
        }
    }
}

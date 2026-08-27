// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer.TemplatedElements
{
    public sealed partial class RemoveActionButton : Control
    {
        public RemoveActionButton()
        {
            DefaultStyleKey = typeof(RemoveActionButton);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static DependencyProperty CommandProperty { get; set; } = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(RemoveActionButton),
            new(null));

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static DependencyProperty CommandParameterProperty { get; set; } = DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(RemoveActionButton),
            new(default));
    }
}

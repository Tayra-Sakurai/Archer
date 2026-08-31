// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer.TemplatedElements
{
    [ContentProperty(Name = nameof(AdditionalCommands))]
    [TemplatePart(Name = "CommandBar", Type = typeof(CommandBar))]
    public sealed partial class DataViewCommands : Control
    {
        private CommandBar? commandBar;

        public DataViewCommands()
        {
            DefaultStyleKey = typeof(DataViewCommands);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            commandBar = (CommandBar)GetTemplateChild("CommandBar");
            AdditionalCommands.CollectionChanged += AdditionalCommands_CollectionChanged;
        }

        private void AdditionalCommands_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplyTemplateCommands();
        }

        private void ApplyTemplateCommands()
        {
            if (commandBar == null)
                return;

            foreach (ICommandBarElement commandBarElement in AdditionalCommands)
                commandBar.PrimaryCommands.Add(commandBarElement);
        }

        private readonly static DependencyProperty AddCommandProperty = DependencyProperty.Register(
            nameof(AddCommand),
            typeof(ICommand),
            typeof(DataViewCommands),
            new(default(ICommand)));

        public ICommand LoadCommand
        {
            get => (ICommand)GetValue(LoadCommandProperty);
            set => SetValue(LoadCommandProperty, value);
        }

        private readonly static DependencyProperty LoadCommandProperty = DependencyProperty.Register(
            nameof(LoadCommand),
            typeof(ICommand),
            typeof(DataViewCommands),
            new(default));

        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }

        private readonly static DependencyProperty DetailCommandProperty = DependencyProperty.Register(
            nameof(DetailCommand),
            typeof(ICommand),
            typeof(DataViewCommands),
            new(null));

        public object? DetailCommandParameter
        {
            get => GetValue(DetailCommandParameterProperty);
            set => SetValue(DetailCommandParameterProperty, value);
        }

        private readonly static DependencyProperty DetailCommandParameterProperty = DependencyProperty.Register(
            nameof(DetailCommandParameter),
            typeof(object),
            typeof(DataViewCommands),
            new(null));

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        private readonly static DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            nameof(RemoveCommand),
            typeof(ICommand),
            typeof(DataViewCommands),
            new(null));

        public object? RemoveCommandParameter
        {
            get => GetValue(RemoveCommandParameterProperty);
            set => SetValue(RemoveCommandParameterProperty, value);
        }

        private readonly static DependencyProperty RemoveCommandParameterProperty = DependencyProperty.Register(
            nameof(RemoveCommandParameter),
            typeof(object),
            typeof(DataViewCommands),
            new(null));

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        private readonly static DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            nameof(SearchCommand),
            typeof(ICommand),
            typeof(DataViewCommands),
            new(default(ICommand)));

        public object SearchCommandParameter
        {
            get => GetValue(SearchCommandParameterProperty);
            set => SetValue(SearchCommandParameterProperty, value);
        }

        private readonly static DependencyProperty SearchCommandParameterProperty = DependencyProperty.Register(
            nameof(SearchCommandParameter),
            typeof(object),
            typeof(DataViewCommands),
            new(default));

        public ObservableCollection<ICommandBarElement> AdditionalCommands
        {
            get => (ObservableCollection<ICommandBarElement>)GetValue(AdditionalCommandsProperty);
            set => SetValue(AdditionalCommandsProperty, value);
        }

        private static readonly DependencyProperty AdditionalCommandsProperty = DependencyProperty.Register(
            nameof(AdditionalCommands),
            typeof(ObservableCollection<ICommandBarElement>),
            typeof(DataViewCommands),
            new(new ObservableCollection<ICommandBarElement>(), OnAdditionalCommandsPropertyChanged));

        private static void OnAdditionalCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataViewCommands)d).ApplyTemplateCommands();
        }
    }
}

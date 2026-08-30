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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer.TemplatedElements
{
    [TemplatePart(Name = "Box", Type = typeof(NumberBox))]
    [TemplatePart(Name = "Block", Type = typeof(TextBlock))]
    public sealed partial class ValidationNumberBox : Control
    {
        private NumberBox? box;
        private TextBlock? block;

        public ValidationNumberBox()
        {
            DefaultStyleKey = typeof(ValidationNumberBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            box = (NumberBox)GetTemplateChild("Box");
            block = (TextBlock)GetTemplateChild("Block");

            box.ValueChanged += Box_ValueChanged;
        }

        private void Box_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Value = sender.Value;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static DependencyProperty ValueProperty { get; } = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(ValidationNumberBox),
            new(default(double)));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static DependencyProperty HeaderProperty { get; } = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(ValidationTextBox),
            new(string.Empty));

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        private static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(ValidationTextBox),
            new(string.Empty));

        public INotifyDataErrorInfo DataErrorInfo
        {
            get => (INotifyDataErrorInfo)GetValue(DataErrorInfoProperty);
            set => SetValue(DataErrorInfoProperty, value);
        }

        public static DependencyProperty DataErrorInfoProperty { get; } = DependencyProperty.Register(
            nameof(DataErrorInfo),
            typeof(INotifyDataErrorInfo),
            typeof(ValidationTextBox),
            new(default, HandleDataErrorInfoPropertyChanged));

        private static void HandleDataErrorInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            INotifyDataErrorInfo? old = e.OldValue as INotifyDataErrorInfo;
            if (old != null)
                old.ErrorsChanged -= ((ValidationNumberBox)d).NewValue_ErrorsChanged;

            INotifyDataErrorInfo newValue = (INotifyDataErrorInfo)e.NewValue;
            newValue.ErrorsChanged += ((ValidationNumberBox)d).NewValue_ErrorsChanged;
            ((ValidationNumberBox)d).HandleErrors();
        }

        private void NewValue_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            HandleErrors();
        }

        private void HandleErrors()
        {
            if (
                DataErrorInfo is null ||
                box is null ||
                block is null ||
                string.IsNullOrEmpty(PropertyName))
                return;

            ValidationResult? result = DataErrorInfo.GetErrors(PropertyName).OfType<ValidationResult>().FirstOrDefault();
            if (result is null)
            {
                block.Text = string.Empty;
                if (App.Current.Resources.TryGetValue("TextBoxBorderThemeBrush", out object brush1))
                {
                    box.BorderBrush = (Brush)brush1;
                }
            }
            else
            {
                block.Text = result.ErrorMessage ?? string.Empty;
                if (App.Current.Resources.TryGetValue("SystemControlErrorTextForegroundBrush", out object brush))
                {
                    box.BorderBrush = (Brush)brush;
                }
            }
        }
    }
}

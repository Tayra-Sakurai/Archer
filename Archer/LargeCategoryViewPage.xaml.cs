// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Caesar.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging;
using Caesar.Messages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LargeCategoryViewPage : Page, IRecipient<LargeCategoryRemovedMessage>
{
    private LargeCategoryViewModel? viewModel;

    public LargeCategoryViewPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register(this);
    }

    ~LargeCategoryViewPage()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        viewModel = Ioc.Default.GetRequiredService<LargeCategoryViewModel>();
        if (e.Parameter is LargeCategory largeCategory)
        {
            await viewModel.LoadExistingLargeCategoryAsync(largeCategory);
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    public void Receive(LargeCategoryRemovedMessage message)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
        else
        {
            Frame.Navigate(typeof(LargeCategoriesViewPage));
        }
    }
}

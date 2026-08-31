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
using Microsoft.Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BasePage : Page
{
    public BasePage()
    {
        InitializeComponent();

        MainNavigation.BackRequested += MainNavigation_BackRequested;
        MainNavigation.ItemInvoked += MainNavigation_ItemInvoked;
        BaseFrame.Navigated += BaseFrame_Navigated;
    }

    private void BaseFrame_Navigated(object sender, NavigationEventArgs e)
    {
        MainNavigation.IsBackEnabled = BaseFrame.CanGoBack;
    }

    private void MainNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is PageInfo pageInfo)
        {
            ResourceLoader resourceLoader = new();
            sender.Header = resourceLoader.GetString(pageInfo.Name);
            BaseFrame.Navigate(pageInfo.PageType);
        }
    }

    private void MainNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (BaseFrame.CanGoBack)
        {
            BaseFrame.GoBack();
        }
        sender.IsBackEnabled = BaseFrame.CanGoBack;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        MainNavigation.MenuItemsSource = ConstantsArcher.Infos;
        BaseFrame.Navigate(ConstantsArcher.Infos[0].PageType);
    }
}

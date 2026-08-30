// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Contexts;
using Caesar.Services;
using Caesar.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.Storage;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Ioc.Default.ConfigureServices(GetService());
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            using (CaesarContext context = await Ioc.Default.GetRequiredService<IDbContextFactory<CaesarContext>>().CreateDbContextAsync())
            {
                await context.Database.MigrateAsync();
            }
            _window = new MainWindow();
            _window.Activate();
        }

        private static IServiceProvider GetService()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .AddEnvironmentVariables()
                .Build();

            ApiSettings? apiSettings = configuration.Get<ApiSettings>();

            if (apiSettings != null)
            {
                ServiceCollection services = new();
                services.AddEmbeddingGenerator(
                    new EmbeddingClient(
                        model: "gemini-embedding-2-preview",
                        credential: new System.ClientModel.ApiKeyCredential(apiSettings.Api_Key),
                        options: new()
                        {
                            Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/"),
                        })
                    .AsIEmbeddingGenerator());

                string source = System.IO.Path.Combine(ApplicationData.GetDefault().LocalFolder.Path, "Caesar.db");

                services.AddDbContextFactory<CaesarContext>(
                    optionsBuilder => optionsBuilder
                        .UseSqlite($"Data Source={source}"));

                services.AddSingleton<ICaesarDatabaseService<CaesarContext>, WindowsCaesarDatabaseService>();
                services.AddSingleton<IEmbeddingVectorService<string, float>, WindowsGeminiEmbeddignVectorService>();

                services.AddTransient<LargeCategoriesViewModel>();
                services.AddTransient<LargeCategoryViewModel>();
                services.AddTransient<MediumCategoriesViewModel>();
                services.AddTransient<MediumCategoryViewModel>();
                services.AddTransient<SmallCategoriesViewModel>();
                services.AddTransient<SmallCategoryViewModel>();
                services.AddTransient<ItemsViewModel>();
                services.AddTransient<ItemViewModel>();
                services.AddTransient<PaymentMethodsViewModel>();
                services.AddTransient<PaymentMethodViewModel>();

                return services.BuildServiceProvider();
            }
            else
                throw new Exception("No valid API key was given.");
        }
    }
}

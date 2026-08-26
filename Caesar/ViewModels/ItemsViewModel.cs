using Caesar.Contexts;
using Caesar.Models;
using Caesar.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;

        public ItemsViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            Items = [];
            SmallCategories = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<Item> Items { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<SmallCategory> SmallCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            Items.Clear();
            SmallCategories.Clear();

            List<Item> items = [];
            List<SmallCategory> smallCategories = [];

            foreach (
                Item item in
                await caesarDatabaseService.GetEntitiesAsync(c => c.Items))
            {
                await caesarDatabaseService.LoadRelatedEntityAsync(item, i => i.SmallCategory);
                if (item.SmallCategory is not null)
                {
                    await caesarDatabaseService.LoadRelatedEntityAsync(item.SmallCategory, s => s.MediumCategory);
                    if (item.SmallCategory.MediumCategory is not null)
                        await caesarDatabaseService.LoadRelatedEntityAsync(item.SmallCategory.MediumCategory, m => m.LargeCategory);
                }

                items.Add(item);
            }
        }
    }
}

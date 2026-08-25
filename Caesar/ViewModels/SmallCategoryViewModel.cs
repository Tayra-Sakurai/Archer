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
    public partial class SmallCategoryViewModel : ObservableValidator
    {
        private readonly ICaesarDatabaseService<CaesarContext> caesarDatabaseService;
        private readonly IEmbeddingVectorService<string, float> embeddingVectorService;
        private SmallCategory smallCategory;

        public SmallCategoryViewModel(ICaesarDatabaseService<CaesarContext> caesarDatabaseService, IEmbeddingVectorService<string, float> embeddingVectorService)
        {
            this.caesarDatabaseService = caesarDatabaseService;
            this.embeddingVectorService = embeddingVectorService;
            smallCategory = new();
            MediumCategories = [];
        }

        [ObservableProperty]
        public partial ObservableCollection<MediumCategory> MediumCategories { get; set; }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task LoadAsync()
        {
            MediumCategories.Clear();

            foreach (
                MediumCategory mediumCategory in
                await caesarDatabaseService.GetEntitiesAsync<MediumCategory>())
                MediumCategories.Add(mediumCategory);
        }
    }
}

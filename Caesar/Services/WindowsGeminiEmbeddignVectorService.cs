using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.Services
{
    public class WindowsGeminiEmbeddignVectorService : IEmbeddingVectorService<string, float>
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;

        public WindowsGeminiEmbeddignVectorService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.embeddingGenerator = embeddingGenerator;
        }

        private async Task<float[]> GetEmbeddingsAsync(string query, EmbeddingGenerationOptions? options = null)
        {
            ReadOnlyMemory<float> readOnlyMemory = await embeddingGenerator.GenerateVectorAsync(query, options);
            return readOnlyMemory.ToArray();
        }

        public async Task<float[]> GenerateVectorForSemanticSimilarlityAsync(string input, EmbeddingGenerationOptions? options = null)
        {
            string query = $"task: sentence similarity | query: {input}";
            return await GetEmbeddingsAsync(query, options);
        }

        public async Task<float[]> GenerateVectorForDocumentAsync(string input, EmbeddingGenerationOptions? options = null)
        {
            string query = $"title: none | text: {input}";
            return await GetEmbeddingsAsync(query, options);
        }

        public Task<float[]> GenerateVectorForDocumentAsync(string title, string content, EmbeddingGenerationOptions? options = null)
        {
            string query = $"title: {title} | text: {content}";
            return GetEmbeddingsAsync(query, options);
        }

        public Task<float[]> GenerateVectorForSearchQueryAsync(string input, EmbeddingGenerationOptions? options = null)
        {
            string query = $"task: search result | query: {input}";
            return GetEmbeddingsAsync(query, options);
        }

        public Task<float[]> GenerateVectorForClassificationAsync(string input, EmbeddingGenerationOptions? options = null)
        {
            string query = $"task: classification | query: {input}";
            return GetEmbeddingsAsync(query, options);
        }
    }
}

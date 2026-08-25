using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.Services
{
    /// <summary>
    /// The embedding generation interface to generate task-oriented vectors.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TOutput">The output vector element type.</typeparam>
    public interface IEmbeddingVectorService<TInput, TOutput>
    {
        /// <summary>
        /// Generates the vector to calculate the semantic similarlity.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="options">The generation option.</param>
        /// <returns>The task to return the vector.</returns>
        /// <exception cref="ArgumentNullException">A null value has been handled.</exception>
        /// <exception cref="ArgumentException">The input was empty or only contained white spaces.</exception>
        Task<TOutput[]> GenerateVectorForSemanticSimilarlityAsync(TInput input, EmbeddingGenerationOptions? options = null);

        /// <summary>
        /// Generates the vector for searched document.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="options">The generation option.</param>
        /// <returns>The task to return the vector data of <paramref name="input"/>.</returns>
        /// <exception cref="ArgumentNullException">A null value has been handled.</exception>
        /// <exception cref="ArgumentException">The input was empty or only contained white spaces.</exception>
        Task<TOutput[]> GenerateVectorForDocumentAsync(TInput input, EmbeddingGenerationOptions? options = null);

        /// <summary>
        /// <para>Generates the document vector for the titled document.</para>
        /// <para>The <paramref name="title"/> and <paramref name="content"/> must be the same type.</para>
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <param name="content">The body contents of the document.</param>
        /// <param name="options">The generation option.</param>
        /// <returns>The task to return the document.</returns>
        /// <exception cref="ArgumentNullException">One or more argument is null.</exception>
        /// <exception cref="ArgumentException">One or more arguments were empty or only contained white spaces.</exception>
        Task<TOutput[]> GenerateVectorForDocumentAsync(TInput title, TInput content, EmbeddingGenerationOptions? options = null);

        /// <summary>
        /// Generates the vector for the search query.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="options">The generation options.</param>
        /// <returns>The asynchronous task controller to return the value of the vector.</returns>
        /// <exception cref="ArgumentNullException">One or more argument is null.</exception>
        /// <exception cref="ArgumentException">One or more arguments were empty or only contained white spaces.</exception>
        Task<TOutput[]> GenerateVectorForSearchQueryAsync(TInput input, EmbeddingGenerationOptions? options = null);

        /// <summary>
        /// Generates the vector for classification task asynchronously.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="options">The generation option.</param>
        /// <returns>The asynchronous task to return the vector.</returns>
        /// <exception cref="ArgumentNullException">One or more argument is null.</exception>
        /// <exception cref="ArgumentException">One or more arguments were empty or only contained white spaces.</exception>
        Task<TOutput[]> GenerateVectorForClassificationAsync(TInput input, EmbeddingGenerationOptions? options = null);
    }
}

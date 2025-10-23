#region Copyright & License

// Copyright © 2024-2025 Yuma
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Yuma.Ddd;

/// <summary>Represents a generic repository interface for managing aggregate root entities.</summary>
/// <typeparam name="TEntity">The type of the aggregate root entity.</typeparam>
/// <typeparam name="TKey">The type of the key for the aggregate root entity, which must be a value type.</typeparam>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public abstraction.")]
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public abstraction.")]
public interface IRepository<TEntity, in TKey> : IQueryableReadOnlyRepository<TEntity, TKey>
	where TEntity : AggregateRoot<TKey>
	where TKey : struct
{
	/// <summary>Asynchronously adds an entity to the repository context, marking it for insertion.</summary>
	/// <remarks>
	/// If there are associated objects to <paramref name="entity"/>, the whole object graph will be added as well as
	/// necessary.
	/// </remarks>
	/// <param name="entity">The entity to add to the repository.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	[SuppressMessage("ReSharper", "UnusedParameter.Global")]
	Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

	/// <summary>Asynchronously removes an entity from the repository context, marking it for deletion.</summary>
	/// <param name="entity">The entity to remove from the repository.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	[SuppressMessage("ReSharper", "UnusedParameter.Global")]
	Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

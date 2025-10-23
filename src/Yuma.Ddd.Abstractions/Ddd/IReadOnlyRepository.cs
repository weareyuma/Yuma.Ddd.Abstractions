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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yuma.Ddd;

/// <summary>Provides a read-only repository abstraction for managing and querying aggregate root entities.</summary>
/// <typeparam name="TEntity">The type of the entity being managed.</typeparam>
/// <typeparam name="TKey">The type of the key for the entity.</typeparam>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public abstraction.")]
public interface IReadOnlyRepository<TEntity, in TKey>
	where TEntity : AggregateRoot<TKey>
	where TKey : struct
{
	/// <summary>Finds all entities.</summary>
	/// <param name="cancellationToken"></param>
	/// <returns>An enumeration containing all entities.</returns>
	Task<IEnumerable<TEntity?>> FindAllAsync(CancellationToken cancellationToken = default);

	/// <summary>Finds an entity by its key value.</summary>
	/// <param name="id">The key value.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>The entity or null if not found.</returns>
	Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);

	/// <summary>Finds an entity by its key value.</summary>
	/// <param name="id">The key value.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>The entity.</returns>
	/// <exception cref="EntityNotFoundException">Thrown when no matching entity is found.</exception>
	Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

	/// <summary>Returns true if any entity matching the given predicate exists, false otherwise.</summary>
	/// <param name="predicate">Predicate that must be matched by any entity.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>true if there is at least one entity matching the predicate, false otherwise.</returns>
	/// <remarks>
	/// This method must be used with some caution: it will only return accurate results if there are no pending changes that
	/// have not been flushed in the current unit of work. Indeed, only the storage layer is queried for the existence of matching
	/// entities, and not the local entity cache.
	/// </remarks>
	Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}

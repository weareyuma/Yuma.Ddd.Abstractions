#region Copyright & License

// Copyright © 2024 - 2025 Yuma
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

namespace Yuma.Ddd.Extensions;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public static class RepositoryExtensions
{
	/// <summary>
	/// Allows to invoke the <see cref="AggregateRoot{TKey}.OnAddedToRepository"/> method for a specified aggregate root
	/// entity once it has been added to the repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the aggregate root entity.</typeparam>
	/// <typeparam name="TKey">The type of the key for the aggregate root entity, which must be a value type.</typeparam>
	/// <param name="repository">The repository interface, which provides context for the operation.</param>
	/// <param name="entity">The aggregate root entity that has been added to the repository.</param>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter")]
	[SuppressMessage("ReSharper", "UnusedParameter.Global")]
	public static void DispatchOnAddedToRepository<TEntity, TKey>(this IRepository<TEntity, TKey> repository, TEntity entity)
		where TEntity : AggregateRoot<TKey>
		where TKey : struct
	{
		entity.OnAddedToRepository();
	}

	/// <summary>
	/// Allows to invoke the <see cref="AggregateRoot{TKey}.OnRemovedFromRepository"/> method for a specified aggregate root
	/// entity once it has been removed from the repository.
	/// </summary>
	/// <typeparam name="TEntity">The type of the aggregate root entity.</typeparam>
	/// <typeparam name="TKey">The type of the key for the aggregate root entity, which must be a value type.</typeparam>
	/// <param name="repository">The repository interface, providing context for the operation.</param>
	/// <param name="entity">The aggregate root entity that has been removed from the repository.</param>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter")]
	[SuppressMessage("ReSharper", "UnusedParameter.Global")]
	public static void DispatchOnRemovedFromRepository<TEntity, TKey>(this IRepository<TEntity, TKey> repository, TEntity entity)
		where TEntity : AggregateRoot<TKey>
		where TKey : struct
	{
		entity.OnRemovedFromRepository();
	}
}

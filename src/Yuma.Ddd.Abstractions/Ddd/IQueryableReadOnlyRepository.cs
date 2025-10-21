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
using System.Linq;

namespace Yuma.Ddd;

/// <summary>Represents a read-only queryable repository providing <see cref="IQueryable"/> access to aggregate root entities.</summary>
/// <typeparam name="TEntity">The type of the aggregate root entity being managed.</typeparam>
/// <typeparam name="TKey">The type of the key that uniquely identifies the aggregate root entity.</typeparam>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public abstraction.")]
public interface IQueryableReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity, TKey>
	where TEntity : AggregateRoot<TKey>
	where TKey : struct
{
	/// <summary>Creates an object that can be used to query the repository using <see cref="IQueryable"/>.</summary>
	/// <returns>An object that can be used to write LINQ queries. Supported features are LINQ-provider dependent.</returns>
	IQueryable<TEntity> CreateQuery();
}

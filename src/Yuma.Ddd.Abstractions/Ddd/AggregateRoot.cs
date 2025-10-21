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

namespace Yuma.Ddd;

/// <summary>Represents an aggregate root entity in a domain-driven design (DDD) context.</summary>
/// <typeparam name="TKey">The type of the unique identifier for the aggregate root, constrained to <c>struct</c> value types.</typeparam>
/// <remarks>
/// <para>
/// The <c>AggregateRoot{TKey}</c> class serves as a base class for aggregate root entities, which are the entry points to
/// aggregates in the domain model. All aggregate roots must derive from this class, inheriting functionality related to unique
/// identification and base entity behavior.
/// </para>
/// <para>Currently serves as a marker mostly.</para>
/// </remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public abstract class AggregateRoot<TKey> : Entity<TKey>
	where TKey : struct
{
	/// <summary>Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class with default values.</summary>
	/// <remarks>
	/// This constructor is useful for scenarios such as framework instantiation or serialization where the identifier does
	/// not need to be initially provided.
	/// </remarks>
	protected AggregateRoot() { }

	/// <summary>Initializes a new instance of the <see cref="AggregateRoot{TKey}"/> class with the specified identifier.</summary>
	/// <param name="id">The unique identifier for the aggregate root.</param>
	/// <remarks>
	/// Use this constructor to initialize an aggregate root with a specific <c>Id</c>. This is useful for scenarios where the
	/// identifier is already known and needs to be assigned at the time of creation.
	/// </remarks>
	protected AggregateRoot(TKey id) : base(id) { }

	/// <summary>Invoked when the aggregate root is added to a repository.</summary>
	/// <remarks>
	/// This method is called to notify that the aggregate root has been added or registered in a repository context. It may
	/// be overridden in derived classes to provide custom behavior that should occur upon the addition to the repository.
	/// </remarks>
	protected internal virtual void OnAddedToRepository() { }

	/// <summary>Invoked when the aggregate root is removed from the repository.</summary>
	/// <remarks>
	/// This method is called to notify that the aggregate root has been detached or removed from its associated repository.
	/// It may be overridden in derived classes to provide custom behavior that should occur upon the removal operation.
	/// </remarks>
	protected internal virtual void OnRemovedFromRepository() { }
}

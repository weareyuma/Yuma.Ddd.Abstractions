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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Yuma.Ddd;

/// <summary>Base class that defines the common properties for entities.</summary>
/// <typeparam name="TKey">The type of the key for the entity.</typeparam>
/// <seealso href="https://github.com/JonPSmith/EfCore.GenericEventRunner">EfCore.GenericEventRunner</seealso>
[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API.")]
public abstract class Entity<TKey> : Entity, IEquatable<Entity<TKey>>
	where TKey : struct
{
	#region Operators

	/// <summary>Determines whether two <see cref="Entity{TKey}"/> instances are considered equal.</summary>
	/// <param name="left">The first <see cref="Entity{TKey}"/> instance to compare.</param>
	/// <param name="right">The second <see cref="Entity{TKey}"/> instance to compare.</param>
	/// <returns><c>true</c> if the two entities are considered equal; otherwise, <c>false</c>.</returns>
	/// <remarks>This operator uses the <see cref="object.Equals(object?, object?)"/> method to determine equality.</remarks>
	public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
	{
		return Equals(left, right);
	}

	/// <summary>Determines whether two <see cref="Entity{TKey}"/> instances are not equal.</summary>
	/// <param name="left">The first <see cref="Entity{TKey}"/> instance to compare.</param>
	/// <param name="right">The second <see cref="Entity{TKey}"/> instance to compare.</param>
	/// <returns><c>true</c> if the two entities are not equal; otherwise, <c>false</c>.</returns>
	/// <remarks>This operator uses the negation of the <see cref="object.Equals(object?, object?)"/> method to determine inequality.</remarks>
	public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
	{
		return !Equals(left, right);
	}

	#endregion

	/// <summary>Initializes a new instance of the <see cref="Entity{TKey}"/> class with default values.</summary>
	/// <remarks>
	/// This constructor is typically used by frameworks and serializers when creating an entity instance without explicitly
	/// specifying an identifier.
	/// </remarks>
	protected Entity() { }

	/// <summary>Initializes a new instance of the <see cref="Entity{TKey}"/> class with the specified identifier.</summary>
	/// <param name="id">The unique identifier for the entity.</param>
	/// <remarks>
	/// Use this constructor to initialize an entity with a specific <c>Id</c>. This is useful for scenarios where the
	/// identifier is already known and needs to be assigned at the time of creation.
	/// </remarks>
	protected Entity(TKey id)
	{
		_id = id;
	}

	#region IEquatable<Entity<TKey>> Members

	/// <inheritdoc/>
	public bool Equals(Entity<TKey>? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return !Equals(Id, default(TKey)) && GetType() == other.GetType() && Equals(Id, other.Id);
	}

	#endregion

	#region Base Class Member Overrides

	/// <inheritdoc/>
	[SuppressMessage("Naming", "CA1725:Parameter names should match base declaration")]
	public override bool Equals(object? @object)
	{
		if (@object is null) return false;
		if (ReferenceEquals(this, @object)) return true;
		return @object is Entity<TKey> other && Equals(other);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		// use object's runtime memory reference for hash code
		return RuntimeHelpers.GetHashCode(this);
	}

	#endregion

	#region Base Class Member Overrides

	/// <inheritdoc/>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	protected internal override bool PrintMembers(StringBuilder stringBuilder, string format, IFormatProvider? formatProvider)
	{
		stringBuilder.Append("Id = ")
			.Append(Id);
		return true;
	}

	#endregion

	/// <summary>Gets or sets the unique identifier for the current instance of the entity.</summary>
	/// <remarks>
	/// The <c>Id</c> property is typically used to uniquely distinguish instances of the entity within a collection or data
	/// context. It is frequently assigned by the database or another data source when the entity is persisted.
	/// </remarks>
	/// <value>A value of type <c>Guid</c> (or other type, if applicable) that represents the entity's unique identifier.</value>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public virtual TKey Id
	{
		get => _id;
		protected set => _id = value;
	}

	/// <summary>Gets a value indicating whether the entity is new and has not been persisted yet.</summary>
	/// <remarks>
	/// This property determines if the entity is considered "new" by checking whether its <c>Id</c> is equal to the default
	/// value for the type <c>TKey</c>. If the <c>Id</c> has a default value, the entity is likely not yet persisted in a data store or
	/// database.
	/// </remarks>
	/// <value><c>true</c> if the entity is new (its <c>Id</c> equals the default value for <c>TKey</c>); otherwise, <c>false</c>.</value>
	[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
	public virtual bool IsNew => Equals(Id, default(TKey));

	private TKey _id;
}

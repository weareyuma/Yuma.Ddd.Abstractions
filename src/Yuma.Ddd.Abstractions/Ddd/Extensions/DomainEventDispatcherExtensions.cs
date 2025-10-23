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

namespace Yuma.Ddd.Extensions;

[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
public static class DomainEventDispatcherExtensions
{
	/// <summary>
	/// Dequeues all domain events from the specified <see cref="Entity"/> and prepares them for dispatching using the
	/// provided <see cref="IDomainEventDispatcher"/>.
	/// </summary>
	/// <param name="domainEventDispatcher">
	/// The <see cref="IDomainEventDispatcher"/> instance responsible for dispatching the domain
	/// events.
	/// </param>
	/// <param name="entity">The <see cref="Entity"/> instance from which domain events will be dequeued.</param>
	/// <returns>An array of <see cref="IDomainEvent"/> containing the dequeued domain events from the entity.</returns>
	/// <remarks>
	/// This method is used to retrieve the domain events from an entity, typically as part of a dispatching workflow. The
	/// events dequeued are returned, and it is assumed that the dispatcher will dispatch them subsequently.
	/// </remarks>
	[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter")]
	[SuppressMessage("ReSharper", "UnusedParameter.Global")]
	public static IDomainEvent[] DequeueEntityDomainEvents(this IDomainEventDispatcher domainEventDispatcher, Entity entity)
	{
		return entity.DequeueDomainEvents();
	}
}

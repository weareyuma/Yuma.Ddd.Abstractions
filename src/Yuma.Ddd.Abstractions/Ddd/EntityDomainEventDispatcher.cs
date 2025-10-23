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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Be.Stateless.Linq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Yuma.Ddd.Extensions;

namespace Yuma.Ddd;

/// <summary>
/// Defines a dispatcher that dispatches domain events for an entity by resolving and invoking their  to handlers in a
/// strongly typed manner.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
public sealed class EntityDomainEventDispatcher : IDomainEventDispatcher
{
	/// <summary>Initializes a new instance of the <see cref="EntityDomainEventDispatcher"/> class.</summary>
	/// <param name="serviceProvider">An <see cref="IServiceProvider"/> to resolve domain event handlers.</param>
	/// <exception cref="ArgumentNullException">Thrown when the <paramref name="serviceProvider"/> is <c>null</c>.</exception>
	public EntityDomainEventDispatcher(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider);
		_serviceProvider = serviceProvider;
	}

	#region IDomainEventDispatcher Members

	/// <inheritdoc/>
	public Task DispatchEntityDomainEventsAsync(Entity entity, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(entity);
		return this.DequeueEntityDomainEvents(entity)
			.ForEachAsync(domainEvent => DispatchDomainEventAsync((dynamic) domainEvent, cancellationToken));
	}

	#endregion

	/// <summary>
	/// Implements a strongly-typed double dispatch mechanism for the given domain event by invoking all handlers that handle
	/// the specified type of event.
	/// </summary>
	/// <typeparam name="T">The type of the domain event being dispatched.</typeparam>
	/// <param name="domainEvent">The domain event to dispatch.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous dispatch operation.</returns>
	/// <remarks>
	/// This method uses the dynamic type system to resolve and invoke the correct handler for the specified type of domain
	/// event. It ensures that all registered handlers for the event are executed in sequence.
	/// </remarks>
	/// <seealso href="https://en.wikipedia.org/wiki/Double_dispatch#Double_dispatch_in_C#">Double dispatch in C#</seealso>
	private Task DispatchDomainEventAsync<T>(T domainEvent, CancellationToken cancellationToken)
		where T : IDomainEvent
	{
		return _serviceProvider.GetServices<IDomainEventHandler<T>>()
			.ForEachAsync(handler => handler.HandleAsync(domainEvent, cancellationToken));
	}

	/// <summary>The <see cref="IServiceProvider"/> used to resolve domain event handlers.</summary>
	private readonly IServiceProvider _serviceProvider;
}

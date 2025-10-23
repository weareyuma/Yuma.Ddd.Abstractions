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

/// <summary>Defines a contract for handling domain events in a domain-driven design (DDD) context.</summary>
/// <typeparam name="T">The type of the domain event to handle, which must implement the <see cref="IDomainEvent"/> interface.</typeparam>
/// <remarks>
/// Implement this interface to define asynchronous logic for processing specific domain events. Each implementation
/// corresponds to a particular domain event type and specifies how the event should be handled (e.g., updating state, triggering
/// side effects, etc.).
/// </remarks>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public abstraction.")]
[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public abstraction.")]
public interface IDomainEventHandler<in T>
	where T : IDomainEvent
{
	/// <summary>Handles the given domain event asynchronously.</summary>
	/// <param name="domainEvent">The domain event instance to handle.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	/// This method defines the event-handling logic for a specific type of domain event. Use the provided
	/// <paramref name="domainEvent"/> parameter to access the event's details and take appropriate actions. The
	/// <paramref name="cancellationToken"/> parameter supports graceful operation cancellation.
	/// </remarks>
	Task HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
}

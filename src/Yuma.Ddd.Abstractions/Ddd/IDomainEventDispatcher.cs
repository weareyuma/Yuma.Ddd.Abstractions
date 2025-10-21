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
using System.Threading;
using System.Threading.Tasks;

namespace Yuma.Ddd;

/// <summary>Provides an abstraction for dispatching domain events to their respective handlers.</summary>
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public abstraction.")]
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public abstraction.")]
public interface IDomainEventDispatcher
{
	/// <summary>Dispatches all the domain events associated with the specified entity to their respective handlers asynchronously.</summary>
	/// <param name="entity">The entity whose domain events are to be dispatched.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous operation of dispatching domain events.</returns>
	Task DispatchEntityDomainEventsAsync(Entity entity, CancellationToken cancellationToken = default);
}

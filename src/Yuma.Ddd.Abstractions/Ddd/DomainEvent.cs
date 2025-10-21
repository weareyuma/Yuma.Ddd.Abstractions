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

using NodaTime;

namespace Yuma.Ddd;

/// <summary>
/// Represents the base implementation of a domain event, <see cref="IDomainEvent"/>, in a domain-driven design (DDD)
/// context.
/// </summary>
/// <remarks>
/// A domain event describes something that has occurred within the domain and is of significance to the domain model. The
/// <c>DomainEvent</c> abstract record provides a base for all domain events, including metadata such as a timestamp.
/// </remarks>
public abstract record DomainEvent : IDomainEvent
{
	#region IDomainEvent Members

	/// <summary>Gets the creation timestamp of the domain event.</summary>
	/// <value>A <see cref="ZonedDateTime"/> representing the moment this event was created.</value>
	/// <remarks>
	/// The timestamp captures when the domain event occurred. This may be used for auditing, sequencing, or other purposes
	/// where the time of the event is important.
	/// </remarks>
	public ZonedDateTime Timestamp { get; } = ClockProvider.Instance.UtcNow;

	#endregion
}

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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Be.Stateless.Extensions;

namespace Yuma.Ddd;

/// <summary>
/// This abstract class acts as a fundamental building block in domain-driven design, providing event management
/// capabilities and state representation for derived domain entities.
/// </summary>
/// <remarks>
/// <para>
/// Domain events are not dispatched to handlers immediately when raised, as doing so can lead to unpredictable side effects
/// and complicate testing. Instead, domain events are recorded, <see cref="EnqueueDomainEvent{T}"/>>, within the entity and
/// dispatched later. This approach decouples the domain model from immediate side effects, making it easier to test entities in
/// isolation without relying on a global event dispatcher.
/// </para>
/// <para>
/// When it’s time to persist changes, such as in EF Core, we hook into the <c>SaveChanges</c> method. Just before committing
/// the transaction, we dispatch the recorded domain events to their respective handlers. By separating the raising of domain
/// events from their dispatching, we make the process clearer for developers and maintain transactional consistency.
/// </para>
/// <para>
/// This design also offers flexibility. Events can be dispatched synchronously (e.g., within the same process) or
/// asynchronously (e.g. by storing events as JSON for processing in an external system). By decoupling event raising from handler
/// logic, the solution promotes simpler testing, easier maintenance, and a more robust event handling system.
/// </para>
/// <para>Additionally, this class provides support for customizable string representations of entity states.</para>
/// </remarks>
/// <seealso cref="IDomainEventDispatcher"/>
/// <seealso href="https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/">A better domain events pattern</seealso>
[DebuggerDisplay($"{{{nameof(ToString)}(\"d\")}}")]
[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public base class.")]
public abstract class Entity : IFormattable
{
	#region Nested Type: FormatStrings

	/// <summary>
	/// Contains predefined format strings and validation logic specific to the <see cref="Entity"/> class and its derived
	/// types.
	/// </summary>
	/// <remarks>
	/// The <c>FormatStrings</c> nested class provides a centralized set of format strings tailored for formatting entities'
	/// data consistently. It also ensures that only valid format strings are used through validation logic.
	/// </remarks>
	[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	protected internal static class FormatStrings
	{
		internal static string EnsureNotNullOrEmptyOrValidate(string? format)
		{
			if (format.IsNullOrEmpty())
				format = DEFAULT;
			else
				ThrowIfInvalid(format);
			return format;
		}

		public static bool IsCustom(string format)
		{
			return format is CUSTOM_SHORT or CUSTOM_LONG;
		}

		public static bool IsGeneral(string format)
		{
			return format is GENERAL_SHORT or GENERAL_LONG;
		}

		public static bool IsDebug(string format)
		{
			return format is DEBUG_SHORT or DEBUG_LONG;
		}

		public static bool IsLong(string format)
		{
			return format is GENERAL_LONG or CUSTOM_LONG or DEBUG_LONG;
		}

		public static bool IsShort(string format)
		{
			return format is GENERAL_SHORT or CUSTOM_SHORT or DEBUG_SHORT;
		}

		private static void ThrowIfInvalid(string format)
		{
			if (format is not (GENERAL_SHORT or GENERAL_LONG or CUSTOM_SHORT or CUSTOM_LONG or DEBUG_SHORT or DEBUG_LONG))
				throw new FormatException(
					string.Format(
						CultureInfo.InvariantCulture,
						"The format string '{0}' is invalid. Supported formats are '{1}', '{2}', '{3}', '{4}', '{5}', and '{6}'.",
						format,
						CUSTOM_SHORT,
						CUSTOM_LONG,
						GENERAL_SHORT,
						GENERAL_LONG,
						DEBUG_SHORT,
						DEBUG_LONG));
		}

		/// <summary>Represents a customized long format string for entities, providing detailed information about the entity.</summary>
		public const string CUSTOM_LONG = "C";

		/// <summary>Represents a customized short format string for entities, used for concise entity summaries.</summary>
		public const string CUSTOM_SHORT = "c";

		/// <summary>Represents the long debug format string, typically including detailed diagnostic information.</summary>
		public const string DEBUG_LONG = "D";

		/// <summary>Represents the short debug format string, used for concise debugging-related information.</summary>
		public const string DEBUG_SHORT = "d";

		/// <summary>
		/// Represents the default format string for entities, pointing to <see cref="GENERAL_SHORT"/>. This is used when no
		/// specific format string is provided.
		/// </summary>
		public const string DEFAULT = GENERAL_SHORT;

		/// <summary>Represents a detailed general format string for entities, including more exhaustive information.</summary>
		public const string GENERAL_LONG = "G";

		/// <summary>Represents a short general format string for entities, typically used for concise representations.</summary>
		public const string GENERAL_SHORT = "g";
	}

	#endregion

	#region IFormattable Members

	/// <inheritdoc/>
	public virtual string ToString(string? format, IFormatProvider? formatProvider = null)
	{
		format = FormatStrings.EnsureNotNullOrEmptyOrValidate(format);
		// ensure consistent output for debug
		if (FormatStrings.IsDebug(format)) formatProvider = CultureInfo.InvariantCulture;

		var stringBuilder = new StringBuilder();
		// @formatter:wrap_chained_method_calls wrap_if_long
		if (FormatStrings.IsLong(format)) stringBuilder.Append(formatProvider, $"{GetType().Name} ");
		// @formatter:wrap_chained_method_calls restore
		stringBuilder.Append("{ ");
		if (PrintMembers(stringBuilder, format, formatProvider)) stringBuilder.Append(value: ' ');
		stringBuilder.Append(value: '}');
		return stringBuilder.ToString();
	}

	#endregion

	#region Base Class Member Overrides

	/// <inheritdoc/>
	[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider")]
	public override string ToString()
	{
		return ToString(string.Empty);
	}

	#endregion

	/// <summary>Indicates whether the entity has any domain events queued for dispatching.</summary>
	[SuppressMessage("Performance", "CA1860:Avoid using Enumerable.Any() extension method")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public bool HasQueuedDomainEvents => _events?.Any() ?? false;

	/// <summary>Represents a queue of domain events that have occurred on the entity but are pending dispatch.</summary>
	/// <remarks>The domain events in this queue have not been dispatched yet.</remarks>
	protected internal virtual IEnumerable<IDomainEvent> Events => _events ?? Enumerable.Empty<IDomainEvent>();

	/// <summary>Queues one or more domain events within the entity for future dispatch.</summary>
	/// <typeparam name="T">The type of the domain events being queued, which must implement <see cref="IDomainEvent"/>.</typeparam>
	/// <param name="domainEvents">
	/// The domain events to be queued. These events are added to the entity's internal event queue for
	/// dispatch at a later time.
	/// </param>
	/// <remarks>
	/// This method delays the dispatch of events and their handler invocation. Instead, it enqueues the events for deferred
	/// processing, typically during a persistence operation, such as in a <c>DbContext</c>.
	/// </remarks>
	[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
	protected internal virtual void EnqueueDomainEvent<T>(params T[] domainEvents)
		where T : IDomainEvent
	{
		if (domainEvents is null || domainEvents.Length == 0) return;
		var queue = _events ??= new Queue<IDomainEvent>(capacity: 1);
		foreach (var domainEvent in domainEvents) queue.Enqueue(domainEvent);
	}

	/// <summary>Dequeues all domain events from the dispatch queue, returning them and clearing the queue.</summary>
	/// <returns>An array of domain events that were in the queue. If the queue is empty, returns an empty array.</returns>
	/// <remarks>
	/// This method retrieves and removes all domain events from the internal queue, typically after they have been prepared
	/// for dispatch or further processing.
	/// </remarks>
	protected internal virtual IDomainEvent[] DequeueDomainEvents()
	{
		var events = Events.ToArray();
		_events?.Clear();
		return events;
	}

	/// <summary>
	/// Generates a string representation of the current instance's fields and properties by appending their names and values
	/// to the specified <see cref="System.Text.StringBuilder"/>.
	/// </summary>
	/// <param name="stringBuilder">
	/// The <see cref="System.Text.StringBuilder"/> to which the members will be appended. Must not be
	/// <c>null</c>.
	/// </param>
	/// <param name="format">
	/// A string that defines the format to be applied to the members. It specifies what information is included
	/// and how the fields and properties of the object are represented as a string. The value should correspond to one of the
	/// predefined format strings, such as <c>GENERAL_LONG</c> or <c>ENTITY_SHORT</c>, provided by <see cref="Entity.FormatStrings"/>.
	/// </param>
	/// <param name="formatProvider">
	/// The <see cref="IFormatProvider"/> that supplies culture-specific formatting information. If
	/// <c>null</c>, the formatting uses the default culture information.
	/// </param>
	/// <returns>
	/// <c>true</c> if members were successfully appended to the <paramref name="stringBuilder"/>; otherwise, <c>false</c>,
	/// typically indicating no members are present.
	/// </returns>
	/// <remarks>
	/// The <c>PrintMembers</c> method is commonly overridden to customize the string representation of an object's data for
	/// debugging or logging purposes.
	/// </remarks>
	/// <example>
	/// Example of overriding <c>PrintMembers</c>: <code>
	/// protected override bool PrintMembers(StringBuilder builder)
	/// {
	///     builder.Append("Id = ").Append(Id);
	///     builder.Append(", Name = ").Append(Name);
	///     return true;
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="Entity.FormatStrings"/>
	[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public API.")]
	protected internal abstract bool PrintMembers(StringBuilder stringBuilder, string format, IFormatProvider? formatProvider);

	private Queue<IDomainEvent>? _events;
}

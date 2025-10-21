#region Copyright & License

// Copyright © 2024 - 2025 Aprico Consultants
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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using AutoFixture.AutoMoq;
using Moq;
using Yuma.AutoFixture.Xunit2;

namespace Yuma.Ddd;

public abstract class EntityFixture
{
	#region Nested Type: DequeueDomainEvents

	public class DequeueDomainEvents : EntityFixture
	{
		[Theory]
		[AutoData<AutoMoqCustomization>]
		public void ReturnsArrayOfQueuedDomainEventsAndClearsQueue(Entity sut, IDomainEvent event1, IDomainEvent event2)
		{
			sut.EnqueueDomainEvent(event1, event2);
			sut.Events.Should()
				.HaveCount(expected: 2);
			sut.DequeueDomainEvents()
				.Should()
				.BeEquivalentTo([event1, event2]);
			sut.Events.Should()
				.BeEmpty();
		}

		[Theory]
		[AutoData<AutoMoqCustomization>]
		public void ReturnsEmptyArrayIfNoQueuedDomainEvents(Entity sut)
		{
			sut.DequeueDomainEvents()
				.Should()
				.BeEmpty();
		}
	}

	#endregion

	#region Nested Type: EnqueueDomainEvent

	public class EnqueueDomainEvent : EntityFixture
	{
		[Theory]
		[AutoData<AutoMoqCustomization>]
		public void CanEnqueueMultipleEvents(Entity sut, IDomainEvent event1, IDomainEvent event2)
		{
			sut.Events.Should()
				.HaveCount(expected: 0);
			Invoking(() => sut.EnqueueDomainEvent(event1, event2))
				.Should()
				.NotThrow();
			sut.Events.Should()
				.HaveCount(expected: 2);
		}

		[Theory]
		[AutoData<AutoMoqCustomization>]
		public void CanEnqueueOneEvent(Entity sut, IDomainEvent @event)
		{
			sut.Events.Should()
				.HaveCount(expected: 0);
			Invoking(() => sut.EnqueueDomainEvent(@event))
				.Should()
				.NotThrow();
			sut.Events.Should()
				.HaveCount(expected: 1);
		}

		[Theory]
		[AutoData<AutoMoqCustomization>]
		public void DoesNotThrowWhenEnqueueingEmptyCollection(Entity sut)
		{
			Invoking(() => sut.EnqueueDomainEvent<IDomainEvent>())
				.Should()
				.NotThrow();
			sut.Events.Should()
				.HaveCount(expected: 0);
		}

		[Theory]
		[AutoData<AutoMoqCustomization>]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void DoesNotThrowWhenEnqueueingNullEvent(Entity sut)
		{
			Invoking(() => sut.EnqueueDomainEvent<IDomainEvent>(null!))
				.Should()
				.NotThrow();
			sut.Events.Should()
				.HaveCount(expected: 0);
		}
	}

	#endregion

	#region Nested Type: FormatStringsFixture

	public class FormatStringsFixture : EntityFixture
	{
		[Theory]
		[InlineData("")]
		[InlineData(Entity.FormatStrings.DEFAULT)]
		[InlineData(null!)]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void DefaultsToGeneralShortIfNullOrEmpty(string? format)
		{
			Entity.FormatStrings.EnsureNotNullOrEmptyOrValidate(format)
				.Should()
				.Be(Entity.FormatStrings.GENERAL_SHORT);
		}

		[Theory]
		[InlineData(Entity.FormatStrings.CUSTOM_LONG)]
		[InlineData(Entity.FormatStrings.CUSTOM_SHORT)]
		[InlineData(Entity.FormatStrings.DEBUG_LONG)]
		[InlineData(Entity.FormatStrings.DEBUG_SHORT)]
		[InlineData(Entity.FormatStrings.GENERAL_LONG)]
		[InlineData(Entity.FormatStrings.GENERAL_SHORT)]
		public void EnsureNotNullOrEmptyOrValidateReturnsFormatIfValid(string format)
		{
			Entity.FormatStrings.EnsureNotNullOrEmptyOrValidate(format)
				.Should()
				.Be(format);
		}

		[Theory]
		[InlineData("x")]
		[InlineData("z")]
		public void EnsureNotNullOrEmptyOrValidateThrowsIfInvalid(string format)
		{
			Invoking((Action)(() => Entity.FormatStrings.EnsureNotNullOrEmptyOrValidate(format)))
				.Should()
				.Throw<FormatException>();
		}
	}

	#endregion

	#region Nested Type: HashCode

	public class HashCode : EntityFixture
	{
		[Fact]
		public void GetHashCodeReturnsBaseObjectHasCodeIfEntityIsNew()
		{
			var sut = new DummyEntity();
			var hashCode = RuntimeHelpers.GetHashCode(sut);

			sut.IsNew.Should()
				.BeTrue();
			sut.GetHashCode()
				.Should()
				.Be(hashCode);
			// hash code is stable
			sut.GetHashCode()
				.Should()
				.Be(hashCode);
		}

		[Fact]
		public void GetHashCodeReturnsBaseObjectHasCodeIfEntityIsNotNew()
		{
			var id = Guid.NewGuid();
			var sut = new DummyEntity(id);
			var hashCode = RuntimeHelpers.GetHashCode(sut);

			sut.IsNew.Should()
				.BeFalse();
			sut.GetHashCode()
				.Should()
				.Be(hashCode);
			// hash code is stable
			sut.GetHashCode()
				.Should()
				.Be(hashCode);
		}

		private sealed class DummyEntity : Entity<Guid>
		{
			public DummyEntity() { }

			public DummyEntity(Guid guid) : base(guid) { }
		}
	}

	#endregion

	#region Nested Type: ToStringFormatting

	public class ToStringFormatting : EntityFixture
	{
		[Fact]
		public void FormattableDelegatesToPrintMembersWithFormatProvider()
		{
			var sut = new Mock<Entity> {
				CallBase = true
			};

			_ = sut.Object.ToString("", CultureInfo.CurrentCulture);

			sut.Verify(static m => m.PrintMembers(It.IsAny<StringBuilder>(), Entity.FormatStrings.GENERAL_SHORT, CultureInfo.CurrentCulture), Times.Once());
		}

		[Fact]
		public void FormattableDelegatesToPrintMembersWithNoFormatProvider()
		{
			var sut = new Mock<Entity> {
				CallBase = true
			};

			_ = sut.Object.ToString("");

			sut.Verify(static m => m.PrintMembers(It.IsAny<StringBuilder>(), Entity.FormatStrings.GENERAL_SHORT, null), Times.Once());
		}

		[Theory]
		[InlineData(Entity.FormatStrings.DEBUG_LONG)]
		[InlineData(Entity.FormatStrings.DEBUG_SHORT)]
		public void FormattableForcesFormatProviderToInvariantCultureForDebugFormats(string format)
		{
			var sut = new Mock<Entity> {
				CallBase = true
			};

			_ = sut.Object.ToString(format);

			sut.Verify(m => m.PrintMembers(It.IsAny<StringBuilder>(), format, CultureInfo.InvariantCulture), Times.Once());
		}

		[Fact]
		public void ToStringDelegatesToFormattableWithEmptyFormatAndNullFormatProvider()
		{
			var sut = new Mock<Entity> {
				CallBase = true
			};

			_ = sut.Object.ToString();

			sut.Verify(static m => m.ToString(string.Empty, null), Times.Once());
		}
	}

	#endregion
}

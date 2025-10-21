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

namespace Yuma.Ddd;

public abstract class EntityNotFoundExceptionFixture
{
	#region Nested Type: Throw

	[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
	public class Throw : EntityNotFoundExceptionFixture
	{
		[Fact]
		public void ThrowsMessageWithGeneratedMemberMessage()
		{
			DummyEntity? entity = null;
			Invoking(() => EntityNotFoundException.Throw(entity, static () => "Number: 123456"))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity { Number: 123456 }' not found.");
		}

		[Fact]
		public void ThrowsMessageWithMemberMessage()
		{
			DummyEntity? entity = null;
			Invoking(() => EntityNotFoundException.Throw(entity, "Number: 123456"))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity { Number: 123456 }' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsMessageWithNullGeneratedMemberMessage()
		{
			DummyEntity? entity = null;
			Invoking(() => EntityNotFoundException.Throw(entity, static () => null!))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsMessageWithNullMemberMessageFactory()
		{
			DummyEntity? entity = null;
			Invoking(() => EntityNotFoundException.Throw(entity, ((Func<string?>?) null)!))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}

		[Fact]
		public void ThrowsMessageWithoutMemberMessage()
		{
			DummyEntity? entity = null;
			Invoking(() => EntityNotFoundException.Throw(entity))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}
	}

	#endregion

	#region Nested Type: ThrowIfNull

	public class ThrowIfNull : EntityNotFoundExceptionFixture
	{
		[Fact]
		public void DoesNotThrowWhenNotNull()
		{
			DummyEntity entity = new();
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity))
				.Should()
				.NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsMessageWithGeneratedMemberMessage()
		{
			DummyEntity entity = null!;
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity, static () => "Number: 123456"))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity { Number: 123456 }' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsMessageWithNullGeneratedMemberMessage()
		{
			DummyEntity entity = null!;
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity, static () => null!))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsMessageWithNullMemberMessageFactory()
		{
			DummyEntity entity = null!;
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity, ((Func<string?>?) null)!))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsWhenNullWithMemberMessage()
		{
			DummyEntity entity = null!;
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity, "Number: 123456"))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity { Number: 123456 }' not found.");
		}

		[Fact]
		[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
		public void ThrowsWhenNullWithoutMemberMessage()
		{
			DummyEntity entity = null!;
			Invoking(() => EntityNotFoundException.ThrowIfNull(entity))
				.Should()
				.Throw<EntityNotFoundException>()
				.WithMessage("Entity 'DummyEntity' not found.");
		}
	}

	#endregion

	#region Nested Type: DummyEntity

	private sealed class DummyEntity : Entity<Guid>;

	#endregion
}

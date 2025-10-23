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
using AutoFixture.Xunit2;

namespace Yuma.Ddd.Extensions;

public class EntityExtensionsFixture
{
	[Fact]
	public void DoesNotThrowWhenNotNull()
	{
		DummyEntity entity = new();
		Invoking((Action) (() => entity.UnlessEntityIsNotFound()))
			.Should()
			.NotThrow();
	}

	[Fact]
	public void ReturnsEntityWhenNotNull()
	{
		DummyEntity entity = new();
		entity.UnlessEntityIsNotFound()
			.Should()
			.BeSameAs(entity);
	}

	[Theory]
	[AutoData]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void ThrowsWhenNullWithGeneratedMemberMessage(Guid id)
	{
		DummyEntity entity = null!;
		Invoking((Action) (() => entity.UnlessEntityIsNotFound(() => $"Id: {id}")))
			.Should()
			.Throw<EntityNotFoundException>()
			.WithMessage($"Entity 'DummyEntity {{ Id: {id} }}' not found.");
	}

	[Theory]
	[AutoData]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void ThrowsWhenNullWithMemberMessage(Guid id)
	{
		DummyEntity entity = null!;
		Invoking((Action) (() => entity.UnlessEntityIsNotFound($"Id: {id}")))
			.Should()
			.Throw<EntityNotFoundException>()
			.WithMessage($"Entity 'DummyEntity {{ Id: {id} }}' not found.");
	}

	[Fact]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public void ThrowsWhenNullWithoutMemberMessage()
	{
		DummyEntity entity = null!;
		Invoking((Action) (() => entity.UnlessEntityIsNotFound()))
			.Should()
			.Throw<EntityNotFoundException>()
			.WithMessage("Entity 'DummyEntity' not found.");
	}

	private sealed class DummyEntity : Entity<Guid>;
}

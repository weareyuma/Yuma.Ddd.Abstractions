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
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Yuma.AutoFixture.Xunit2;

namespace Yuma.Ddd;

public class EntityDomainEventDispatcherFixture
{
	[Theory]
	[AutoData<AutoMoqCustomization>]
	public async Task DispatchesToStronglyTypeHandler(Entity entity, OneEvent oneEvent1, OneEvent oneEvent2, TwoEvent twoEvent)
	{
		entity.EnqueueDomainEvent<IDomainEvent>(oneEvent1, twoEvent);
		var handlerMock = new Mock<HandlerSpy>();
		var services = new ServiceCollection();
		services.AddScoped<IDomainEventHandler<OneEvent>>(_ => handlerMock.Object);
		services.AddScoped<IDomainEventHandler<TwoEvent>>(_ => handlerMock.Object);

		var sut = new EntityDomainEventDispatcher(services.BuildServiceProvider());
		await sut.DispatchEntityDomainEventsAsync(entity);

		handlerMock.Verify(m => m.HandleAsync(oneEvent1, It.IsAny<CancellationToken>()), Times.Once);
		handlerMock.Verify(m => m.HandleAsync(oneEvent2, It.IsAny<CancellationToken>()), Times.Never);
		handlerMock.Verify(m => m.HandleAsync(twoEvent, It.IsAny<CancellationToken>()), Times.Once);
		handlerMock.VerifyNoOtherCalls();
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	public void DoesNothingWhenNoQueuedDomainEvents(Entity entity, IServiceProvider serviceProvider)
	{
		var sut = new EntityDomainEventDispatcher(serviceProvider);

		Invoking(() => sut.DispatchEntityDomainEventsAsync(entity))
			.Should()
			.NotThrowAsync();
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	public void DoesNothingWhenNoRegisteredHandlers(Entity entity, IServiceProvider serviceProvider, OneEvent oneEvent)
	{
		entity.EnqueueDomainEvent(oneEvent);

		var sut = new EntityDomainEventDispatcher(serviceProvider);

		Invoking(() => sut.DispatchEntityDomainEventsAsync(entity))
			.Should()
			.NotThrowAsync();
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	public async Task DoesNothingWhenNoRegisteredMatchingHandlers(Entity entity, OneEvent oneEvent)
	{
		var handlerMock = new Mock<HandlerSpy>();
		var services = new ServiceCollection();
		services.AddScoped<IDomainEventHandler<TwoEvent>>(_ => handlerMock.Object);
		entity.EnqueueDomainEvent(oneEvent);

		var sut = new EntityDomainEventDispatcher(services.BuildServiceProvider());
		await sut.DispatchEntityDomainEventsAsync(entity);

		handlerMock.Verify(static m => m.HandleAsync(It.IsAny<OneEvent>(), It.IsAny<CancellationToken>()), Times.Never);
		handlerMock.Verify(static m => m.HandleAsync(It.IsAny<TwoEvent>(), It.IsAny<CancellationToken>()), Times.Never);
		handlerMock.VerifyNoOtherCalls();
	}

	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public class HandlerSpy : IDomainEventHandler<OneEvent>, IDomainEventHandler<TwoEvent>
	{
		#region IDomainEventHandler<OneEvent> Members

		public virtual Task HandleAsync(OneEvent domainEvent, CancellationToken cancellationToken = default)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IDomainEventHandler<TwoEvent> Members

		public virtual Task HandleAsync(TwoEvent domainEvent, CancellationToken cancellationToken = default)
		{
			throw new NotSupportedException();
		}

		#endregion
	}

	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public record OneEvent : DomainEvent
	{
		public Guid Id { get; init; }
	}

	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public record TwoEvent : DomainEvent
	{
		public Guid Id { get; init; }
	}
}

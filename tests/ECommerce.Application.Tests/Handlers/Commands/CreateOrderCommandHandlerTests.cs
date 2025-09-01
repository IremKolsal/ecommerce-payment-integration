using AutoFixture.Xunit2;
using ECommerce.Application.Abstractions;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Application.Commands.CreateOrder;
using ECommerce.Application.Tests.Support;
using ECommerce.Domain.Entities;
using Moq;

namespace ECommerce.Application.Tests.Handlers.Commands;

public class CreateOrderCommandHandlerTests
{
    [Theory, InlineAutoMoqData(10, "pre-001")]
    public async Task Handle_creates_order_and_returns_result(
        int amount,
        string externalOrderId,
        [Frozen] Mock<IBalanceClient> balance,
        [Frozen] Mock<IOrderRepository> repo,
        CreateOrderCommandHandler sut)
    {
        // Arrange
        balance.Setup(b => b.PreorderAsync(amount, externalOrderId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PreorderInfo(externalOrderId, amount, "blocked"));

        repo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);          // AddAsync => Task

        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);                      // SaveChangesAsync => Task<int>

        // Act
        var result = await sut.Handle(new CreateOrderCommand(amount, externalOrderId), CancellationToken.None);

        // Assert
        Assert.Equal(externalOrderId, result.OrderId);
        Assert.Equal("blocked", result.Status);
        Assert.Equal(amount, result.TotalAmount);

        repo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

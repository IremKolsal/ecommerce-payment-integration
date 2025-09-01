using AutoFixture.Xunit2;
using ECommerce.Application.Abstractions;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Application.Commands.CompleteOrder;
using ECommerce.Application.Tests.Support;
using ECommerce.Domain.Entities;
using Moq;

namespace ECommerce.Application.Tests.Handlers.Commands;

public class CompleteOrderCommandHandlerTests
{
    [Theory, InlineAutoMoqData("pre-002")]
    public async Task Handle_completes_blocked_order(
        string externalOrderId,
        [Frozen] Mock<IOrderRepository> repo,
        [Frozen] Mock<IBalanceClient> balance,
        CompleteOrderCommandHandler sut)
    {
        // Arrange
        var order = new Order { ExternalOrderId = externalOrderId, Status = "blocked", TotalAmount = 10 };

        repo.Setup(r => r.GetByExternalIdAsync(externalOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        balance.Setup(b => b.CompleteAsync(externalOrderId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new CompletionInfo(externalOrderId, "completed", DateTime.UtcNow));

        // Act
        var res = await sut.Handle(new CompleteOrderCommand(externalOrderId), CancellationToken.None);

        // Assert
        Assert.Equal("completed", order.Status);
        Assert.Equal(externalOrderId, res.OrderId);
        Assert.Equal("completed", res.Status);

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory, InlineAutoMoqData("pre-003")]
    public async Task Handle_throws_if_not_blocked(
        string externalOrderId,
        [Frozen] Mock<IOrderRepository> repo,
        CompleteOrderCommandHandler sut)
    {
        // Arrange
        repo.Setup(r => r.GetByExternalIdAsync(externalOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order { ExternalOrderId = externalOrderId, Status = "created" });

        // Act
        var act = async () => await sut.Handle(new CompleteOrderCommand(externalOrderId), CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}

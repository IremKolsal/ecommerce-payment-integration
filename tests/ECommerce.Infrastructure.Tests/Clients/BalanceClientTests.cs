using AutoMapper;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Infrastructure.Balance;
using ECommerce.Infrastructure.Common.Errors;
using Moq;
using System.Net;

namespace ECommerce.Infrastructure.Tests.Clients;

public class BalanceClientTests
{
    [Fact]
    public async Task GetProductsAsync_returns_mapped_list()
    {
        // Arrange
        var payload = new
        {
            success = true,
            message = "ok",
            data = new[]
            {
                    new { id = "p1", name = "n", description = "d", price = 10m, currency = "USD", category = "c", stock = 1 }
                }
        };
        var http = HttpStub.FromJson(payload);

        var mapper = new Mock<IMapper>(MockBehavior.Strict);
        mapper.Setup(m => m.Map<List<ProductInfo>>(It.IsAny<object>()))
              .Returns(new List<ProductInfo> {
                      new ProductInfo("p1","n","d",10,"USD","c",1)
              });

        var sut = new BalanceClient(http, mapper.Object);

        // Act
        var list = await sut.GetProductsAsync(CancellationToken.None);

        // Assert
        Assert.Single(list);
        Assert.Equal("p1", list[0].Id);
        mapper.Verify(m => m.Map<List<ProductInfo>>(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task PreorderAsync_returns_info_on_success()
    {
        // Arrange
        var payload = new
        {
            success = true,
            message = "ok",
            data = new
            {
                preOrder = new
                {
                    orderId = "pre-1",
                    amount = 15m,
                    timestamp = DateTime.UtcNow,
                    status = "blocked"
                },
                updatedBalance = new
                {
                    userId = "u",
                    totalBalance = 0m,
                    availableBalance = 0m,
                    blockedBalance = 15m,
                    currency = "USD",
                    lastUpdated = DateTime.UtcNow
                }
            }
        };
        var http = HttpStub.FromJson(payload);

        var mapper = new Mock<IMapper>(MockBehavior.Strict);
        mapper.Setup(m => m.Map<PreorderInfo>(It.IsAny<object>()))
              .Returns(new PreorderInfo("pre-1", 15, "blocked"));

        var sut = new BalanceClient(http, mapper.Object);

        // Act
        var info = await sut.PreorderAsync(15, "pre-1", CancellationToken.None);

        // Assert
        Assert.Equal("pre-1", info.OrderId);
        Assert.Equal(15, info.Amount);
        Assert.Equal("blocked", info.Status);
        mapper.Verify(m => m.Map<PreorderInfo>(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_returns_info_on_success()
    {
        // Arrange
        var payload = new
        {
            success = true,
            message = "ok",
            data = new
            {
                order = new
                {
                    orderId = "pre-2",
                    amount = 20m,
                    timestamp = DateTime.UtcNow,
                    status = "completed",
                    completedAt = DateTime.UtcNow
                },
                updatedBalance = new
                {
                    userId = "u",
                    totalBalance = 0m,
                    availableBalance = 0m,
                    blockedBalance = 0m,
                    currency = "USD",
                    lastUpdated = DateTime.UtcNow
                }
            }
        };
        var http = HttpStub.FromJson(payload);

        var mapper = new Mock<IMapper>(MockBehavior.Strict);
        mapper.Setup(m => m.Map<CompletionInfo>(It.IsAny<object>()))
              .Returns(new CompletionInfo("pre-2", "completed", DateTime.UtcNow));

        var sut = new BalanceClient(http, mapper.Object);

        // Act
        var info = await sut.CompleteAsync("pre-2", CancellationToken.None);

        // Assert
        Assert.Equal("pre-2", info.ExternalOrderId);
        Assert.Equal("completed", info.Status);
        Assert.NotNull(info.CompletedAt);
        mapper.Verify(m => m.Map<CompletionInfo>(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetProductsAsync_throws_on_upstream_error()
    {
        // Arrange
        var payload = new { success = false, message = "boom", data = (object?)null };
        var http = HttpStub.FromJson(payload);
        var mapper = new Mock<IMapper>(MockBehavior.Loose);

        var sut = new BalanceClient(http, mapper.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UpstreamServiceException>(() =>
            sut.GetProductsAsync(CancellationToken.None));
    }

    [Fact]
    public async Task PreorderAsync_throws_on_http_400()
    {
        // Arrange
        var http = HttpStub.FromJson(new { any = "payload" }, HttpStatusCode.BadRequest);
        var mapper = new Mock<IMapper>(MockBehavior.Loose);
        var sut = new BalanceClient(http, mapper.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            sut.PreorderAsync(10, "x", CancellationToken.None));
    }
}

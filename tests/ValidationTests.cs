using CampusEvents.Backend;
using Moq;
using Xunit;

namespace CampusEvents.Tests;

public class EmailDomainValidatorTests
{
    private readonly EmailDomainValidator _validator = new();

    [Theory]
    [InlineData("juan@univ.edu.ph")]
    [InlineData("JUAN.DELA@UNIV.EDU.PH")]
    [InlineData("  maria@univ.edu.ph  ")]
    public void IsValid_ReturnsTrue_ForUniversityEmails(string email)
    {
        Assert.True(_validator.IsValid(email));
    }

    [Theory]
    [InlineData("juan@gmail.com")]
    [InlineData("juan@univ.edu.ph.evil.com")]
    [InlineData("juan@@univ.edu.ph")]
    [InlineData("not-an-email")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_ReturnsFalse_ForInvalidEmails(string? email)
    {
        Assert.False(_validator.IsValid(email));
    }
}

public class SeatAvailabilityServiceTests
{
    [Fact]
    public void HasSeat_ReturnsTrue_WhenSeatsRemain()
    {
        var repo = new Mock<IEventRepository>();
        repo.Setup(r => r.GetMaxSeats(1)).Returns(10);
        repo.Setup(r => r.GetConfirmedCount(1)).Returns(9);
        var service = new SeatAvailabilityService(repo.Object);

        Assert.True(service.HasSeat(1));
    }

    [Fact]
    public void HasSeat_ReturnsFalse_WhenEventIsFull()
    {
        var repo = new Mock<IEventRepository>();
        repo.Setup(r => r.GetMaxSeats(2)).Returns(10);
        repo.Setup(r => r.GetConfirmedCount(2)).Returns(10);
        var service = new SeatAvailabilityService(repo.Object);

        Assert.False(service.HasSeat(2));
    }

    [Fact]
    public void HasSeat_QueriesRepositoryOncePerValue()
    {
        var repo = new Mock<IEventRepository>();
        repo.Setup(r => r.GetMaxSeats(3)).Returns(5);
        repo.Setup(r => r.GetConfirmedCount(3)).Returns(0);
        var service = new SeatAvailabilityService(repo.Object);

        service.HasSeat(3);

        repo.Verify(r => r.GetMaxSeats(3), Times.Once);
        repo.Verify(r => r.GetConfirmedCount(3), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void HasSeat_Throws_ForInvalidEventId(int eventId)
    {
        var repo = new Mock<IEventRepository>(MockBehavior.Strict);
        var service = new SeatAvailabilityService(repo.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.HasSeat(eventId));
        repo.VerifyNoOtherCalls();
    }
}

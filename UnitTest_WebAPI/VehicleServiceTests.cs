using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleRegistration.Core.ServiceContracts;
using VehicleRegistration.Infrastructure.DataBaseModels;
using Xunit;

namespace VehicleRegistration.Tests
{
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleService> _mockVehicleService;

        public VehicleServiceTests()
        {
            _mockVehicleService = new Mock<IVehicleService>();
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ReturnsVehicle_WhenVehicleExists()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var expectedVehicle = new VehicleModel { VehicleId = vehicleId };

            _mockVehicleService.Setup(service => service.GetVehicleByIdAsync(vehicleId))
                               .ReturnsAsync(expectedVehicle);

            // Act
            var result = await _mockVehicleService.Object.GetVehicleByIdAsync(vehicleId);

            // Assert
            Assert.Equal(expectedVehicle, result);
            _mockVehicleService.Verify(service => service.GetVehicleByIdAsync(vehicleId), Times.Once);
        }

        [Fact]
        public async Task GetAllVehicles_ReturnsListOfVehicles_ForGivenUserId()
        {
            // Arrange
            var userId = "1";
            var expectedVehicles = new List<VehicleModel>
            {
                new VehicleModel { VehicleId = Guid.NewGuid(), UserId = 1 },
                new VehicleModel { VehicleId = Guid.NewGuid(), UserId = 1 }
            };

            _mockVehicleService.Setup(service => service.GetAllVehicles(userId))
                               .ReturnsAsync(expectedVehicles);

            // Act
            var result = await _mockVehicleService.Object.GetAllVehicles(userId);

            // Assert
            Assert.Equal(expectedVehicles.Count, result.Count());
            _mockVehicleService.Verify(service => service.GetAllVehicles(userId), Times.Once);
        }

        [Fact]
        public async Task AddVehicle_ReturnsAddedVehicle()
        {
            // Arrange
            var newVehicle = new VehicleModel { VehicleId = Guid.NewGuid() };

            _mockVehicleService.Setup(service => service.AddVehicle(newVehicle))
                               .ReturnsAsync(newVehicle);

            // Act
            var result = await _mockVehicleService.Object.AddVehicle(newVehicle);

            // Assert
            Assert.Equal(newVehicle, result);
            _mockVehicleService.Verify(service => service.AddVehicle(newVehicle), Times.Once);
        }

        [Fact]
        public async Task EditVehicle_ReturnsUpdatedVehicle()
        {
            // Arrange
            var vehicle = new VehicleModel { VehicleId = Guid.NewGuid() };

            _mockVehicleService.Setup(service => service.EditVehicle(vehicle))
                               .ReturnsAsync(vehicle);

            // Act
            var result = await _mockVehicleService.Object.EditVehicle(vehicle);

            // Assert
            Assert.Equal(vehicle, result);
            _mockVehicleService.Verify(service => service.EditVehicle(vehicle), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicle_ReturnsDeletedVehicle_WhenVehicleExists()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var expectedVehicle = new VehicleModel { VehicleId = vehicleId };

            _mockVehicleService.Setup(service => service.DeleteVehicle(vehicleId))
                               .ReturnsAsync(expectedVehicle);

            // Act
            var result = await _mockVehicleService.Object.DeleteVehicle(vehicleId);

            // Assert
            Assert.Equal(expectedVehicle, result);
            _mockVehicleService.Verify(service => service.DeleteVehicle(vehicleId), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicle_ReturnsNull_WhenVehicleDoesNotExist()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _mockVehicleService.Setup(service => service.DeleteVehicle(vehicleId))
                               .ReturnsAsync((VehicleModel)null);

            // Act
            var result = await _mockVehicleService.Object.DeleteVehicle(vehicleId);

            // Assert
            Assert.Null(result);
            _mockVehicleService.Verify(service => service.DeleteVehicle(vehicleId), Times.Once);
        }
    }
}

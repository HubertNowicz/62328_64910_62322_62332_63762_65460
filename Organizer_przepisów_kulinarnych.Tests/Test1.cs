
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Organizer_przepisów_kulinarnych.BLL.Services;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Organizer_przepisów_kulinarnych.Tests
{
    [TestClass]
    public sealed class AdminServiceTests
    {
        [TestMethod]
        public async Task DeleteIngredientAsync_WhenIngredientExists_ReturnsSuccess()
        {
            // Arrange
            var ingredientRepoMock = new Mock<IIngredientRepository>();
            var mapperMock = new Mock<IMapper>();

            ingredientRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Ingredient { Id = 1, Name = "Cukier" });

            ingredientRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Ingredient>()))
                .Returns(Task.CompletedTask);

            ingredientRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new AdminService(ingredientRepoMock.Object, mapperMock.Object);

            // Act
            var result = await service.DeleteIngredientAsync(1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Message);
        }
    }
}

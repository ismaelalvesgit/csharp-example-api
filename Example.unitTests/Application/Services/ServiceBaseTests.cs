using Example.Domain.Entitys;
using Example.Domain.Interfaces.Services;
using Example.unitTests.Fixtures;
using Example.unitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Example.unitTests.Application.Services
{
    public class ServiceBaseTests : IClassFixture<WebInfrastructureFixture>
    {
        private readonly IServiceBase<Category> _serviceBase;
        public ServiceBaseTests(WebInfrastructureFixture infrastructureFixture)
        {
            var scope = infrastructureFixture.Services.CreateScope();

            _serviceBase = scope.ServiceProvider.GetRequiredService<IServiceBase<Category>>();
        }

        [Fact]
        public async Task Remove_ShoudSucess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            await _serviceBase.InsertAsync(category);

            // Act
            var exception = await Record.ExceptionAsync(() => _serviceBase.DeleteAsync(category));

            // Assert
            Assert.Null(exception);
        }
    }
}

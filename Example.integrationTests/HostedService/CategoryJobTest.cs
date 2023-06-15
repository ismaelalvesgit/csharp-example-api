using Example.BackgroundTasks.HostedServices;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;
using Example.integrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Example.integrationTests.HostedService
{
    public class CategoryJobTest : IClassFixture<BackgroundServiceProviderFixture>
    {
        private readonly CategoryJob? _job;
        private readonly ICategoryService _service;
        public CategoryJobTest(BackgroundServiceProviderFixture fixture) {
            _job = fixture.ServiceProvider.GetRequiredService<IHostedService>() as CategoryJob;
            _service = fixture.ServiceProvider.GetRequiredService<ICategoryService>();
        }

        [Fact]
        public async Task ExecuteAsync_Sucess()
        {
            // Act
            var imageUrl = "https://ismael.alves.com";
            var date = DateTime.UtcNow;
            await _job.ExecuteAsync(CancellationToken.None);
            var categorys = await _service.FindAllAsync(new QueryData() { FilterBy = new[] { $"ImageUrl eq {imageUrl}", $"CreatedAt gt {date}" } });
            var itens = categorys.Items;

            // Assert
            Assert.Equal(imageUrl, itens.First().ImageUrl);
            Assert.NotEmpty(itens);
        }
    }
}

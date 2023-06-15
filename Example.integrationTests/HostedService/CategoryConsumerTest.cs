using Example.Application.Dto;
using Example.BackgroundTasks.HostedServices;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;
using Example.integrationTests.Fixtures;
using Example.integrationTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Example.integrationTests.HostedService
{
    public class CategoryConsumerTest : IClassFixture<BackgroundServiceProviderFixture>
    {
        private readonly CategoryConsumer? _consumer;
        private readonly ICategoryService _service;
        private readonly IProducerService _producer;
        private readonly IConfiguration _config;

        public CategoryConsumerTest(BackgroundServiceProviderFixture fixture) {
            _consumer = fixture.ServiceProvider.GetServices<IHostedService>().OfType<CategoryConsumer>().Single();
            _service = fixture.ServiceProvider.GetRequiredService<ICategoryService>();
            _config = fixture.ServiceProvider.GetRequiredService<IConfiguration>();
            _producer = fixture.ServiceProvider.GetRequiredService<IProducerService>();
        }

        [Fact(Skip = "Verificar performace")]
        public async Task ExecuteAsync_Sucess()
        {
            // Act
            var topic = "Queuing.Example.Category";
            var category = CategoryGenerator.GenerateValidCategorysDto(1).First();
            await TaskHelpers.DropQueueAsync(_config, new string[] { topic });
            var producerData = new ProducerData<CategoryDto> {
                Topic = topic,
                Data = category
            };
            await _producer.ProduceAsync(producerData);
            await TaskHelpers.CancelAfter(_consumer, TimeSpan.FromSeconds(5));

            var categorys = await _service.FindAllAsync(new QueryData() { FilterBy = new[] {$"Name eq {category.Name}"}});
            var itens = categorys.Items;

            // Assert
            Assert.Equal(category.ImageUrl, itens.First().ImageUrl);
            Assert.Equal(category.Name, itens.First().Name);
            Assert.NotEmpty(itens);
        }

        [Fact(Skip = "Verificar performace")]
        public async Task ExecuteAsync_Data_Not_Valid()
        {
            // Act
            var topic = "Queuing.Example.Category";
            var category = CategoryGenerator.GenerateValidCategorysDto(1).First();
            await TaskHelpers.DropQueueAsync(_config, new string[] { topic });
            category.ImageUrl = null;
            var producerData = new ProducerData<CategoryDto>
            {
                Topic = topic,
                Data = category
            };
            await _producer.ProduceAsync(producerData);
            await TaskHelpers.CancelAfter(_consumer, TimeSpan.FromSeconds(5));


            var categorys = await _service.FindAllAsync(new QueryData() { FilterBy = new[] { $"Name eq {category.Name}" }});
            var itens = categorys.Items;

            // Assert
            Assert.Empty(itens);
        }
    }
}

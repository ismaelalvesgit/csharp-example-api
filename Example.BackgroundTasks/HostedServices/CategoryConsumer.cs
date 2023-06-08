using AutoMapper;
using Example.Application.Dto;
using Example.Application.Validations;
using Example.BackgroundTasks.Interfaces;
using Example.BackgroundTasks.Services;
using Example.Domain.Entitys;
using Example.Domain.Interfaces.Services;

namespace Example.BackgroundTasks.HostedServices
{
    public class CategoryConsumer : ConsumerBaseService
    {
        private readonly ICategoryService _categoryService;
        readonly protected IMapper _mapper;

        public CategoryConsumer(
            ICategoryService categoryService,
            IMapper mapper,
            IConsumerConfig<CategoryConsumer> config,
            IConfiguration configuration,
            ILogger<CategoryConsumer> logger,
            IProducerService producerService,
            IHostEnvironment environment
            ) : base(config.Topic, configuration, environment, logger, producerService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public override async Task ExecuteAsync(string data)
        {
            var category = Deserialize<CategoryDto, CategoryValidation>(data);
            await CreateCategory(category);
        }

        private async Task CreateCategory(CategoryDto? categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryService.InsertAsync(category);
        }
    }
}

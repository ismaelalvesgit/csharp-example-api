using Example.Data;
using Example.Domain.Entitys;
using Example.integrationTests.Fixtures;
using Example.integrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Example.integrationTest.Application.Controllers
{
    public class CategoryControllerTests : IClassFixture<WebInfrastructureFixture>
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _client;
        private readonly string _path = "/api/v1/category";

        public CategoryControllerTests(WebInfrastructureFixture infrastructureFixture)
        {
            var scope = infrastructureFixture.Services.CreateScope();
            _client = infrastructureFixture.CreateClient();
            _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task FindById_ShouldThrowNotFoundException()
        {
            // Act
            var response = await _client.GetAsync($"{_path}/123");
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Category not found", responseString);
        }

        [Fact]
        public async Task FindById_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var categoryData = await _context.Category.FirstOrDefaultAsync(x => x.Name == category.Name).ConfigureAwait(false);

            // Act
            var response = await _client.GetFromJsonAsync<Category>($"{_path}/{categoryData?.Id}");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(categoryData?.Name, response.Name);
            Assert.Equal(categoryData?.ImageUrl, response.ImageUrl);
        }

        [Fact]
        public async Task FindAll_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(5);
            await _context.Category.AddRangeAsync(category).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var categoryName = category.First().Name;
            var queryString = HandlerHelper.ToQueryString(new { FilterBy = $"Name eq {categoryName}", OrderBy = "Name", OrderByDescending = true });

            // Act
            var response = await _client.GetAsync($"{_path}{queryString}");
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(categoryName);
            Assert.Contains(categoryName, responseString);
        }

        [Fact]
        public async Task FindAll_ShouldThrowBadRequestException()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(5);
            await _context.Category.AddRangeAsync(category).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var queryString = HandlerHelper.ToQueryString(new { FilterBy = $"Name" });

            // Act
            var response = await _client.GetAsync($"{_path}{queryString}");
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains("Filter not valid...", responseString);
        }

        [Fact]
        public async Task Producer_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorysDto(1).First();

            // Act
            var response = await _client.PostAsJsonAsync($"{_path}/async", category);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Producer_ShouldThrowBadRequestException()
        {

            // Act
            var response = await _client.PostAsJsonAsync($"{_path}/async", new { Data = "Not Data Required" });
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains("Name is not empty", responseString);
            Assert.Contains("ImageUrl is not empty", responseString);
        }

        [Fact]
        public async Task Create_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorysDto(1).First();

            // Act
            var response = await _client.PostAsJsonAsync($"{_path}", category);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Create_ShouldThrowBadRequestException()
        {

            // Act
            var response = await _client.PostAsJsonAsync($"{_path}", new { Data = "Not Data Required" });
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains("Name is not empty", responseString);
            Assert.Contains("ImageUrl is not empty", responseString);
        }

        [Fact]
        public async Task Put_ShouldThrowBadRequestException()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var categoryData = await _context.Category.FirstOrDefaultAsync(x => x.Name == category.Name).ConfigureAwait(false);

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/{categoryData?.Id}", new { Data = "Not Data Required" });
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains("Name is not empty", responseString);
            Assert.Contains("ImageUrl is not empty", responseString);
        }

        [Fact]
        public async Task Put_ShouldThrowNotFoundException()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorysDto(1).First();

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/1234", category);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains("NotFound", responseString);
        }

        [Fact]
        public async Task Put_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var categoryDto = CategoryGenerator.GenerateValidCategorysDto(1).First();
            _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var categoryData = await _context.Category.FirstOrDefaultAsync(x => x.Name == category.Name).ConfigureAwait(false);

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/{categoryData?.Id}", categoryDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Del_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var categoryData = await _context.Category.FirstOrDefaultAsync(x => x.Name == category.Name).ConfigureAwait(false);

            // Act
            var response = await _client.DeleteAsync($"{_path}/{categoryData?.Id}");
            var categoryDel = await _context.Category.FirstOrDefaultAsync(x => x.Id == categoryData.Id).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.NotNull(response);
            Assert.Null(categoryDel);
        }
    }
}

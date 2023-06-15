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
    public class ProductControllerTests : IClassFixture<WebInfrastructureFixture>
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _client;
        private readonly string _path = "/api/v1/product";

        public ProductControllerTests(WebInfrastructureFixture infrastructureFixture)
        {
            var scope = infrastructureFixture.Services.CreateScope();
            _client = infrastructureFixture.CreateClient();
            _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task FindById_ShouldThrowNotFoundException()
        {
            // Act
            var response = await _client.GetAsync($"{_path}/100000000");
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Product not found", responseString);
        }

        [Fact]
        public async Task FindById_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(1, entity.Entity.Id).First();
            _context.Product.Add(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productData = await _context.Product.FirstOrDefaultAsync(x => x.Name == product.Name).ConfigureAwait(false);

            // Act
            var response = await _client.GetFromJsonAsync<Category>($"{_path}/{productData?.Id}");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(productData?.Name, response.Name);
            Assert.Equal(productData?.ImageUrl, response.ImageUrl);
        }

        [Theory]
        [InlineData("eq")]
        [InlineData("ne")]
        [InlineData("gt")]
        [InlineData("ge")]
        [InlineData("lt")]
        [InlineData("le")]
        [InlineData("notExist")]
        public async Task FindAll_ShouldSuccess(string condition)
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var products = ProductGenerator.GenerateValidProducts(5, entity.Entity.Id);
            await _context.Product.AddRangeAsync(products).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productPrice = products.First().Price;
            var queryString = HandlerHelper.ToQueryString(new { FilterBy = $"Price {condition} {productPrice}" });

            // Act
            var response = await _client.GetAsync($"{_path}{queryString}");
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.Contains(productPrice.ToString(), responseString);
        }

        [Fact]
        public async Task FindAll_ShouldThrowBadRequestException()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(5, entity.Entity.Id);
            await _context.Product.AddRangeAsync(product).ConfigureAwait(false);
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
        public async Task Create_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(1, entity.Entity.Id).First();

            // Act
            var response = await _client.PostAsJsonAsync($"{_path}", product);

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
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(1, entity.Entity.Id).First();
            _context.Product.Add(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productData = await _context.Product.FirstOrDefaultAsync(x => x.Name == product.Name).ConfigureAwait(false);

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/{productData?.Id}", new { Data = "Not Data Required" });
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
            var product = ProductGenerator.GenerateValidProductsDto(1, 1).First();

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/1234", product);
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
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(1, entity.Entity.Id).First();
            var productDto = ProductGenerator.GenerateValidProductsDto(1, entity.Entity.Id).First();
            _context.Product.Add(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productData = await _context.Product.FirstOrDefaultAsync(x => x.Name == product.Name).ConfigureAwait(false);

            // Act
            var response = await _client.PutAsJsonAsync($"{_path}/{productData?.Id}", productDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Del_ShouldSuccess()
        {
            // Arrange
            var category = CategoryGenerator.GenerateValidCategorys(1).First();
            var entity = _context.Category.Add(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var product = ProductGenerator.GenerateValidProducts(1, entity.Entity.Id).First();
            _context.Product.Add(product);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productData = await _context.Product.FirstOrDefaultAsync(x => x.Name == product.Name).ConfigureAwait(false);

            // Act
            var response = await _client.DeleteAsync($"{_path}/{productData?.Id}");
            var productDel = await _context.Product.FirstOrDefaultAsync(x => x.Id == productData.Id).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
            Assert.NotNull(response);
            Assert.Null(productDel);
        }
    }
}

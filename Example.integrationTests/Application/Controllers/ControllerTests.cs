using Example.integrationTests.Fixtures;
using System.Net;
using Xunit;

namespace Example.integrationTest.Application.Controllers
{
    public class ControllerTests : IClassFixture<WebInfrastructureFixture>
    {
        private readonly HttpClient _client;
        private readonly string _pathHealthcheck = "/api/v1/healthcheck";

        public ControllerTests(WebInfrastructureFixture infrastructureFixture)
        {
            _client = infrastructureFixture.CreateClient();
        }

        [Fact]
        public async Task Healthcheck_ShouldSuccess() 
        {
            // Act
            var response = await _client.GetAsync($"{_pathHealthcheck}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }  
        
        [Fact]
        public async Task Document_ShouldSuccess() 
        {
            // Act
            var response = await _client.GetAsync($"/swagger/index.html");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

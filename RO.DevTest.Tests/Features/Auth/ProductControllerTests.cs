using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RO.DevTest.WebApi;
using Xunit;

namespace RO.DevTest.Tests.Integration.Products
{
    public class ProductControllerTests : IClassFixture<WebApplicationFactory<WebApiEntryPoint>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<WebApiEntryPoint> _factory;

        public ProductControllerTests(WebApplicationFactory<WebApiEntryPoint> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Configurações adicionais para testes
                });
            });
            
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetProducts_Unauthorized_WhenNoToken()
        {
            // Act
            var response = await _client.GetAsync("/api/Product");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private class ProductDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RazorPagesProject.Tests.IntegrationTests
{
    #region snippet1
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Arragro.ObjectHistory.WebExample.Startup>>
    {
        private readonly WebApplicationFactory<Arragro.ObjectHistory.WebExample.Startup> _factory;

        public BasicTests(WebApplicationFactory<Arragro.ObjectHistory.WebExample.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/home")]
        [InlineData("/session")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        #endregion
    }
}
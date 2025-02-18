using System.Net;  
using Microsoft.AspNetCore.Mvc.Testing;  
using NUnit.Framework;  
using Reqnroll;  
using Uxcheckmate_Main; 

namespace BDD_Tests.StepDefinitions  
{
    [Binding] 
    public class ErrorPageSteps
    {
        private readonly WebApplicationFactory<Program> _factory; 
        private HttpClient _client; 
        private HttpResponseMessage _response;  

        public ErrorPageSteps()
        {
            _factory = new WebApplicationFactory<Program>();  // Start a test instance of the app
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Prevent automatic redirects to can test redirection manually
            });
        }

        [Given(@"David requests a non-existent page ""(.*)""")]
        public async Task GivenDavidRequestsANonExistentPage(string url)
        {
            _response = await _client.GetAsync(url);  // Send a GET request to the specified URL
        }

        [Then(@"he should be redirected to ""(.*)""")]
        public void ThenHeShouldBeRedirectedTo(string expectedUrl)
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));  // Assert that the response is a redirect (302 Found)
            Assert.That(_response.Headers.Location?.ToString(), Is.EqualTo(expectedUrl));  // Verify the redirection target
        }
    }
}

using System.Net;
using Moq;
using Moq.Protected;

namespace RickAndMortyApp.Test.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient GetMockHttpClient(string jsonResponse)
        {
            var callCount = 0;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount <= 1
                    ? new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(jsonResponse),
                    } : new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{}"),
                    };
                });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://rickandmortyapi.com/api/")
            };
        }
    }
}

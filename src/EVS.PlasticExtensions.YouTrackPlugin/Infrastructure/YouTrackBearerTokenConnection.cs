using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YouTrackSharp;
using YouTrackSharp.Generated;

namespace EVS.PlasticExtensions.YouTrackPlugin.Infrastructure
{
  internal sealed class YouTrackBearerTokenConnection : Connection
  {
    private readonly string _bearerToken;
    private readonly object _sync = new object();
    private HttpClient _httpClient;
    private YouTrackClient _youTrackClient;

    public YouTrackBearerTokenConnection(string serverUrl, string bearerToken)
      : base(serverUrl)
    {
      _bearerToken = bearerToken;
    }

    public override Task<YouTrackClient> GetAuthenticatedApiClient()
    {
      lock (_sync)
      {
        if (_youTrackClient == null)
        {
          var handler = new HttpClientHandler
          {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
          };

          _httpClient = new HttpClient(handler) { BaseAddress = ServerUri };
          _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _bearerToken);
          _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
          _youTrackClient = new YouTrackClient(_httpClient);
        }

        return Task.FromResult(_youTrackClient);
      }
    }

    public override async Task<HttpClient> GetAuthenticatedRawClient()
    {
      await GetAuthenticatedApiClient();
      return _httpClient;
    }
  }
}

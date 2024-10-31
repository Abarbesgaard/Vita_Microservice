using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpForwarder();

        var app = builder.Build();

        var httpClient = new HttpMessageInvoker(
            new SocketsHttpHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false,
                EnableMultipleHttp2Connections = true,
                ActivityHeadersPropagator = new ReverseProxyPropagator(
                    DistributedContextPropagator.Current
                ),
                ConnectTimeout = TimeSpan.FromSeconds(15),
            }
        );
        var transformer = HttpTransformer.Default;
        var requestConfig = new ForwarderRequestConfig
        {
            ActivityTimeout = TimeSpan.FromSeconds(100),
        };

        app.UseRouting();

        app.Map(
              "/api/video/{**catch-all}",
              async (HttpContext httpContext, IHttpForwarder forwarder) =>
              {
                  var error = await forwarder.SendAsync(
                      httpContext,
                      "https://localhost:10000/",
                      httpClient,
                      requestConfig,
                      transformer
                  );
                  // Check if the operation was successful
                  if (error != ForwarderError.None)
                  {
                      var errorFeature = httpContext.GetForwarderErrorFeature();
                      var exception = errorFeature.Exception;
                  }
              }
          );

        WebApplication
            .CreateBuilder(args)
            .Services.AddReverseProxy()
            .LoadFromConfig(
                WebApplication.CreateBuilder(args).Configuration.GetSection("ReverseProxy")
            );
        app.MapReverseProxy();

        app.Run();
    }
}

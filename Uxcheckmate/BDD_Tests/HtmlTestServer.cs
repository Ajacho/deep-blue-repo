using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

public class HtmlTestServer
{
    private IHost _host;
    private int _port;
    public HtmlTestServer() : this(5002)
    {

    }
    public HtmlTestServer(int port)
    {
        _port = port;
    }
    public void StartServer(string contentRoot)
    {
        StopServer(); // If existing server, stoping it
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseWebRoot(contentRoot)
                    .UseUrls($"http://localhost:{_port}")
                    .Configure(app => app.UseStaticFiles());
            })
            .Build();
        _host.Start();
    }
    public void ChangeContentRoot(string newContentRoot)
    {
        StartServer(newContentRoot); // Changing for new root
    }
    public void StopServer()
    {
        if (_host != null){
            _host.StopAsync().Wait();
            _host.Dispose();
            _host = null;
        }
    }
}

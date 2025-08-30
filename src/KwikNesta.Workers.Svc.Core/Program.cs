using KwikNesta.Workers.Svc.Core.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.
    RegisterServices(builder.Configuration, builder.Environment.ApplicationName);
var host = builder.Build();
host.Run();

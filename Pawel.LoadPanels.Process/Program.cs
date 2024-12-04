using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Globalization;
using BowlingSys.Services.UserService;
using BowlingSys.Handlers.Handlers;
using BowlingSys.DBConnect;
using Pawel.LoadPanels.Contracts.ViewDtos;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5007);  
        options.ListenAnyIP(5008, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    });
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5006); 
    });
}


builder.Services.AddScoped<MyMessageHandler>();
builder.Services.AddScoped<DBConnect>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DBConnect(connectionString);
});

builder.Services.AddScoped<ViewService>(provider =>
{
    var dbConnect = provider.GetRequiredService<DBConnect>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new ViewService(dbConnect);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin() 
            .AllowAnyMethod()  
            .AllowAnyHeader()); 
});

var endpointConfiguration = new EndpointConfiguration("NServiceBusHandlers");
string instanceId = Environment.MachineName;
endpointConfiguration.MakeInstanceUniquelyAddressable(instanceId);

endpointConfiguration.EnableCallbacks();



var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Auto,
    Converters =
    {
        new IsoDateTimeConverter
        {
            DateTimeStyles = DateTimeStyles.RoundtripKind
        }
    }
};
var serialization = endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
serialization.Settings(settings);


var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("/app/.learningtransport");
var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();


var routing = transport.Routing();
routing.RouteToEndpoint(typeof(ViewDto), "NServiceBusHandlers");
var scanner = endpointConfiguration.AssemblyScanner().ScanFileSystemAssemblies = true;
builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

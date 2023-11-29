using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {   
                    policy.WithOrigins("https://nvd.nist.gov",
                        "https://services.nvd.nist.gov/",
                        "https://services.nvd.nist.gov/rest/json/cves/2.0").AllowAnyHeader().AllowAnyMethod();
                }
        );
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

#if DEBUG
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
IConfigurationProvider secretProvider = config.Providers.First();
secretProvider.TryGet("RedisConn", out var ConnString);
#else
builder.Services.AddDataProtection()
                .SetApplicationName("fow-customer-portal")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/dpkeys/"));

           string ConnString = Environment.GetEnvironmentVariable("RedisConn");
#endif
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = ConnString;
    options.InstanceName = "SimsFrontend_";
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


using DotNetEnv;
using webapp.FileSorting.cs.lib;

namespace Webapp.sorting.cs;

class Program
{
    public static void Main(string[] args)
    {
        //  Loading the Environment Variables
        Env.Load();
        
        FileReader filereader = new FileReader();
        filereader.ReadFile("/home/krigjo25/RiderProjects/webapp-FileSorting-cs/DatabaseIntegration/Jeg er uorganisert.txt");
        
        Console.WriteLine($"Server={Environment.GetEnvironmentVariable("MSQL_server")};Database={Environment.GetEnvironmentVariable("MSQL_db")};User Id={Environment.GetEnvironmentVariable("MSQL_user")};Password={Environment.GetEnvironmentVariable("MSQL_passcode")};");
    }
    
}

/*
using webapp_FileSorting_cs.Components;
   
   var builder = WebApplication.CreateBuilder(args);
   
   // Add services to the container.
   builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();
   
   var app = builder.Build();
   
   // Configure the HTTP request pipeline.
   if (!app.Environment.IsDevelopment())
   {
       app.UseExceptionHandler("/Error", createScopeForErrors: true);
       // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
       app.UseHsts();
   }
   
   app.UseHttpsRedirection();
   
   
   app.UseAntiforgery();
   
   app.MapStaticAssets();
   app.MapRazorComponents<App>()
       .AddInteractiveServerRenderMode();
   
   app.Run();
 */
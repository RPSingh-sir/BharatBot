using ChatBot.Data;
using ChatBot.Services;
using ChatBot.Repository;
using Microsoft.EntityFrameworkCore;
using ChatBot.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthentication("ManualAuth")
    .AddCookie("ManualAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatKnowledgeRepository, ChatKnowledgeRepository>();
builder.Services.AddHttpClient<IAiService, OpenAiService>();
builder.Services.AddScoped<IAiService, OpenAiService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DbConnector  >();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 🔴 ORDER MATTERS
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 🔥 THIS WAS MISSING
app.MapControllers();   // enables [Route], [HttpPost], [HttpGet]

// MVC route (UI)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

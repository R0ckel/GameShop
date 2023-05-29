using GameShopAPI.Extensions;
using GameShopAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// add domain services
builder
    .ConfigureServices()
    .ConfigureContext()
    .ConfigureAuth()
    .ConfigureSwagger();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
    builder.WithOrigins(app.Configuration.GetSection("Clients:ReactClient").Value ?? "http://localhost:3000")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials());
app.UseCookiePolicy();

//app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>(); 
app.UseAuthentication();
app.UseAuthorization();

app.UseSeedData();

app.MapControllers();

app.Run();

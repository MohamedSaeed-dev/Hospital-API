using HospitalAPI.Middlewares;
using HospitalAPI.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddJsonConverters();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorizationServices();
builder.Services.AddMiddlewareServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital System APIv1");
    });
}

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/auth"), appBuilder =>
{
    appBuilder.UseMiddleware<VerifyTokenMiddleware>();
});

app.UseCors
    (x => x 
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );

app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler(_ => { });

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
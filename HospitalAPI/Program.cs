using HospitalAPI.Features.JsonConverter;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Repositories;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(
    builder.Configuration.GetConnectionString("SQLConnection")
));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString(DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd"))
    });
    c.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "datetime",
        Example = new OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"))
    });
});

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

builder.Services.AddScoped<MyDbContext>();

builder.Services.AddTransient<IDoctorService, DoctorRepository>();
builder.Services.AddTransient<IDepartmentService, DepartmentRepository>();
builder.Services.AddTransient<IPatientService, PatientRepository>();
builder.Services.AddTransient<IAppointmentService, AppointmentRepository>();
builder.Services.AddTransient<IBillingService, BillingRepository>();
builder.Services.AddTransient<IPrescriptionService, PrescriptionRepository>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseCors();

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();


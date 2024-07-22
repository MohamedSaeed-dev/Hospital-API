using HospitalAPI.Features.JsonConverter;
using HospitalAPI.Features.Mail.Repository;
using HospitalAPI.Features.Mail.Service;
using HospitalAPI.Features.Mail;
using HospitalAPI.Features.Redis.Repository;
using HospitalAPI.Features.Redis.Service;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Repositories;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;
using HospitalAPI.Features.Cookies.IServices;
using HospitalAPI.Features.Cookies.Repository;
using HospitalAPI.Models.ViewModels.ResponseStatus;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Features.Utils.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(
    builder.Configuration.GetConnectionString("SQLConnection")
));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key Authentication"
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();

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
builder.Services.AddTransient<IUserService, UserRepository>();
builder.Services.AddTransient<IAuthService, AuthRepository>();
builder.Services.AddTransient<IMailService, MailRepository>();
builder.Services.AddTransient<IRedisService, RedisRepository>();
builder.Services.AddTransient<ICookieService, CookieRepository>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IUtilitiesService, UtilitiesRepository>();
builder.Services.AddTransient<IResponseStatus, ResponseStatusRepository>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddResponseCaching();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes((builder.Configuration["Jwt:KeyAccessToken"]!)))
        };
    });

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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
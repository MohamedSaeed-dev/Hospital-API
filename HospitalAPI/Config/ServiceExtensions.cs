using HospitalAPI.Features.JsonConverter;
using HospitalAPI.Features.Mail.Repository;
using HospitalAPI.Features.Mail.Service;
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
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Features.Utils.Repository;
using HospitalAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using HospitalAPI.Features.GlobalException;
using HospitalAPI.Features.Mail;

namespace HospitalAPI.Config
{
    public static class ServiceExtensions
    {
        public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MyDbContext>(x => x.UseSqlServer(
                configuration.GetConnectionString("SQLConnection")
            ));
            services.AddScoped<MyDbContext>();
        }

        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IResponseStatus, ResponseStatusRepository>();

            services.AddTransient<IDoctorService, DoctorRepository>();
            services.AddTransient<IDepartmentService, DepartmentRepository>();
            services.AddTransient<IPatientService, PatientRepository>();
            services.AddTransient<IAppointmentService, AppointmentRepository>();
            services.AddTransient<IBillingService, BillingRepository>();
            services.AddTransient<IPrescriptionService, PrescriptionRepository>();
            services.AddTransient<IUserService, UserRepository>();

            services.AddTransient<IAuthService, AuthRepository>();
            services.AddTransient<IMailService, MailRepository>();
            services.AddTransient<IRedisService, RedisRepository>();
            services.AddTransient<ICookieService, CookieRepository>();
            services.AddTransient<IUtilitiesService, UtilitiesRepository>();
            services.AddTransient<ITokenService, TokenRepository>();
            services.AddTransient<HttpClient>();

            services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!));

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            services.AddAutoMapper(typeof(Program).Assembly);
        }

        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hospital System API", Version = "v1" });

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
        }

        public static void AddJsonConverters(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    });
        }

        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes((configuration["Jwt:KeyAccessToken"]!)))
                    };
                })
                .AddGoogle(x =>
                {
                    var config = configuration.GetSection("Auth:Google");
                    x.ClientId = config["ClientId"]!;
                    x.ClientSecret = config["ClientSecret"]!;
                }).AddCookie();
        }

        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization();
        }

        public static void AddMiddlewareServices(this IServiceCollection services)
        {
            services.AddTransient<VerifyTokenMiddleware>();
            services.AddExceptionHandler<AppExceptionHandler>();
            services.AddProblemDetails();
        }
    }
}

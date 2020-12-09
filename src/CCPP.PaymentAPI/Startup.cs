using Coldairarrow.Api;
using Coldairarrow.Util;
using EFCore.Sharding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using System.Linq;

namespace CCPP.PaymentAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFxServices();
            services.AddAutoMapper();
            services.AddEFCoreSharding(config =>
            {
                var dbOptions = _configuration.GetSection("Database:BaseDb").Get<DatabaseOptions>();

                config.UseDatabase(dbOptions.ConnectionString, dbOptions.DatabaseType);
            });
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidFilterAttribute>();
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.GetType().GetProperties().ForEach(aProperty =>
                {
                    var value = aProperty.GetValue(JsonExtention.DefaultJsonSetting);
                    aProperty.SetValue(options.SerializerSettings, value);
                });
            });
            services.AddHttpContextAccessor();

            //swagger
            services.AddOpenApiDocument(settings =>
            {
                settings.Title = "���ܻ���֧��ϵͳAPI";
                settings.AllowReferencesWithProperties = true;
                settings.AddSecurity("������֤Token", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Scheme = "bearer",
                    Description = "Authorization:Bearer {your JWT token}<br/><b>��Ȩ��ַ:/Base_Manage/Home/SubmitLogin</b>",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http
                });
            });
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //����
            app.UseCors(x =>
            {
                x.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .DisallowCredentials();
            })
                .UseMiddleware<RequestBodyMiddleware>()
                .UseMiddleware<RequestLogMiddleware>()
                .UseDeveloperExceptionPage()
                .UseStaticFiles(new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/octet-stream"
                })
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();
                })
                .UseOpenApi()//����swagger����api�ĵ���Ĭ��·���ĵ� /swagger/v1/swagger.json��
                .UseSwaggerUi3()//����Swagger UI������ܵ���(Ĭ��·��: /swagger).
                ;
        }
    }
}

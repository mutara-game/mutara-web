using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Mutara.Web.Services;

namespace Mutara.Web
{
    public class Startup
    {
        private readonly ConfigClient configClient;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configClient = new ConfigClient();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // configuration (consul)
            services.AddSingleton(configClient);

            // AWS
            var regionEndpoint = RegionEndpoint.USWest2;
            var awsCredentials = new AnonymousAWSCredentials();
            services.AddSingleton<AmazonCognitoIdentityProviderClient, AmazonCognitoIdentityProviderClient>(
                serviceProvider => new AmazonCognitoIdentityProviderClient(awsCredentials, regionEndpoint));

            // swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mutara-web",
                    Version = "v1"
                });
                // use JWT
                OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };
                c.AddSecurityDefinition("Bearer", securityDefinition);
                // Make sure swagger UI requires a Bearer token specified
                OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement()
                {
                    {securityScheme, new string[] { }},
                };
                c.AddSecurityRequirement(securityRequirements);
            });
            
            // authorization of JWT via Cognito
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>{
                    options.Audience = configClient.GetValue("secrets.yaml", "cognito/clientId").GetAwaiter().GetResult();
                    options.Authority = configClient.GetValue("secrets.yaml", "cognito/authorityUrl").GetAwaiter()
                        .GetResult();
                });
            
            // services
            services.AddSingleton<CognitoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mutara-web v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

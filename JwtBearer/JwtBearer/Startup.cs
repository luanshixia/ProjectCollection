using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JwtBearer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "http://localhost:17447";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("long_enough_secret")),
                        ValidIssuer = "issuer",
                        ValidAudience = "http://localhost:17447",
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async (context) =>
                        {
                            // Add the access_token as a claim, as we may actually need it
                            var accessToken = context.SecurityToken as JwtSecurityToken;
                            if (accessToken != null)
                            {

                            }
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseAuthentication();
        }
    }
}

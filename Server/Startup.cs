using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quibble.Server.Data;
using Quibble.Server.Data.Models;
using Quibble.Server.Hub;
using Quibble.Server.Services.EmailSender;
using Quibble.Shared.Models;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(opts =>
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { MediaTypeNames.Application.Octet }));

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                    options.InvalidModelStateResponseFactory = context =>
                        new BadRequestObjectResult(
                            context.ModelState
                                .Where(kv => kv.Value is not null)
                                .SelectMany(kv => kv.Value!.Errors)
                                .Select(modelError => modelError.ErrorMessage)
                                .ToList()
                        ));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
                {
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IEmailSender, DebugToLogEmailSender>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "_User";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Events.OnRedirectToLogin = context =>
                {
                    // Login should be handled by the api
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddSignalR();
            services.AddAutoMapper(config =>
            {
                config.CreateMap<DbQuiz, QuizDto>();
                config.CreateMap<DbParticipant, ParticipantDto>()
                    .ForMember(dto => dto.UserName,
                        options => options.MapFrom(dbParticipant => dbParticipant.User.UserName));
                config.CreateMap<DbRound, RoundDto>();
                config.CreateMap<DbQuestion, QuestionDto>();
                config.CreateMap<DbSubmittedAnswer, SubmittedAnswerDto>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<QuibbleHub>("Api/Quibble/{QuizId:guid}");

                // Will prevent calls to APIs which don't exist to return a 404 rather than fall through to index.html.
                endpoints.Map("Api/{**CatchAll}", context =>
                {
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}

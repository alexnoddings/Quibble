using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Quibble.Common.Dtos;
using Quibble.Server.Core;
using Quibble.Server.Core.Models;
using Quibble.Server.Services.EmailSender;
using Quibble.Sync.SignalR.Server;
using System.Net.Mime;

namespace Quibble.Server;

public class Startup
{
	private IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
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

		var dbConnectionString = Configuration.GetConnectionString("DefaultConnection");
		if (string.IsNullOrWhiteSpace(dbConnectionString))
			throw new InvalidOperationException("Invalid connection string DefaultConnection: cannot be null or whitespace.");

		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(dbConnectionString, sqlOptions =>
			{
				sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
			});
		});

		services.AddIdentity<DbUser, DbRole>()
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

		services.AddServerSync().AddSignalr();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseResponseCompression();

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
			endpoints.MapSignalrSync();

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

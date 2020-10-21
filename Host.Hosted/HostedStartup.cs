using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Hosted.Areas.Identity;
using Quibble.Host.Hosted.Data;
using Quibble.Host.Hosted.Platform;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.Host.Hosted.Platform.Services;
using Quibble.UI.Core.Events;
using Quibble.UI.Core.Services;
using Quibble.UI.Core.Services.Data;

namespace Quibble.Host.Hosted
{
    public class HostedStartup : CommonStartup<QuibbleServerSideDbContext>
    {
        public HostedStartup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigurePlatformServices(IServiceCollection services)
        {
            services.AddServerSideBlazor();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<DbQuibbleUser>>();

            services.AddScoped<ILoginHandler, HostedLoginHandler>();
            services.AddScoped<IAppMetadata, HostedAppMetadata>();

            services.AddScoped<IQuizService, HostedQuizService>();
            services.AddScoped<IQuizEvents, StaticQuizEvents>();
            services.AddScoped<IQuizEventsInvoker, StaticQuizEvents>();

            services.AddScoped<IRoundService, HostedRoundService>();
            services.AddScoped<IRoundEvents, StaticRoundEvents>();
            services.AddScoped<IRoundEventsInvoker, StaticRoundEvents>();

            services.AddScoped<IQuestionService, HostedQuestionService>();
            services.AddScoped<IQuestionEvents, StaticQuestionEvents>();
            services.AddScoped<IQuestionEventsInvoker, StaticQuestionEvents>();

            services.AddScoped<IParticipantService, HostedParticipantService>();
            services.AddScoped<IParticipantEvents, StaticParticipantEvents>();
            services.AddScoped<IParticipantEventsInvoker, StaticParticipantEvents>();

            services.AddScoped<IAnswerService, HostedAnswerService>();
            services.AddScoped<IAnswerEvents, StaticAnswerEvents>();
            services.AddScoped<IAnswerEventsInvoker, StaticAnswerEvents>();
        }

        protected override void ConfigurePlatformApplication(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}

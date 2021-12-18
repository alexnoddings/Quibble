using Microsoft.Extensions.DependencyInjection;
using Quibble.Common.Dtos;
using Quibble.Server.Core.Domain;
using Quibble.Server.Core.Models;

namespace Quibble.Server.Core;

public static class ServerSyncBuilderExtensions
{
	public static ServerSyncBuilder AddServerSync(this IServiceCollection services)
	{
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

		services.AddTransient<QuizLogic>();
		services.AddTransient<RoundLogic>();
		services.AddTransient<QuestionLogic>();
		services.AddTransient<SubmittedAnswerLogic>();

		return new ServerSyncBuilder(services);
	}
}

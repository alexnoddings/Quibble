using AutoMapper;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Server.Core;

namespace Quibble.Server.Core.Domain;

public class BaseLogic
{
	protected ILogger<BaseLogic> Logger { get; }
	protected IMapper Mapper { get; }
	protected AppDbContext DbContext { get; }

	public BaseLogic(ILogger<BaseLogic> logger, IMapper mapper, AppDbContext dbContext)
	{
		Logger = logger;
		Mapper = mapper;
		DbContext = dbContext;
	}

	protected static ApiResponse Success() => ApiResponse.FromSuccess();
	protected static ApiResponse<TValue> Success<TValue>(TValue value) => ApiResponse.FromSuccess(value);
	protected static ApiResponse Failure(ApiError error) => ApiResponse.FromError(error);
	protected static ApiResponse<TValue> Failure<TValue>(ApiError error) => ApiResponse.FromError<TValue>(error);
}

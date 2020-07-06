using System;
using System.Threading.Tasks;
using Grpc.Core;
using Quibble.Client.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Extensions.Grpc
{
    public static class QuizServiceClientExtensions
    {
        private static readonly EmptyMessage EmptyMessage = new EmptyMessage();

        public static Task<GrpcReply<QuizInfo>> CreateAsync(this QuizService.QuizServiceClient client, string title)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (title == null) throw new ArgumentNullException(nameof(title));

            var request = new CreateQuizRequest { Title = title };
            return RunAsync(async () => await client.CreateAsync(request));
        }

        public static Task<GrpcReply<QuizInfo>> GetAsync(this QuizService.QuizServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new GetQuizRequest { Id = id };
            return RunAsync(async () => await client.GetAsync(request));
        }

        public static Task<GrpcReply<GetOwnedQuizzesReply>> GetOwnedAsync(this QuizService.QuizServiceClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return RunAsync(async () => await client.GetOwnedAsync(EmptyMessage));
        }

        public static Task<GrpcReply> UpdateTitleAsync(this QuizService.QuizServiceClient client, string id, string newTitle)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (newTitle == null) throw new ArgumentNullException(nameof(newTitle));

            var request = new UpdateQuizTitleRequest {Id = id, NewTitle = newTitle};
            return RunAsync(async () => await client.UpdateTitleAsync(request));
        }

        public static async Task<GrpcReply> RunAsync(Func<Task<EmptyMessage>> exec) => 
            await RunAsync<EmptyMessage>(exec).ConfigureAwait(false);

        public static async Task<GrpcReply<TReply>> RunAsync<TReply>(Func<Task<TReply>> exec)
            where TReply : class
        {
            if (exec == null) throw new ArgumentNullException(nameof(exec));

            TReply? reply = null;
            var statusCode = StatusCode.OK;
            string statusDetail = "OK";

            try
            {
                reply = await exec().ConfigureAwait(false);
            }
            // e.InnerException is not set correctly, so specific cases need to check the status code and detail
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal && e.Status.Detail.Contains("AccessTokenNotAvailableException", StringComparison.OrdinalIgnoreCase))
            {
                // AccessTokenNotAvailableException is thrown when the user is not logged in
                statusCode = StatusCode.Unauthenticated;
                statusDetail = "You must be logged in to access this";
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal && e.Status.Detail.Contains("NetworkError when attempting to fetch resource", StringComparison.OrdinalIgnoreCase))
            {
                // JSException for NetworkError is thrown when the use cannot connect to the server
                statusCode = StatusCode.Unauthenticated;
                statusDetail = "You must be logged in to access this";
            }
            catch (RpcException e)
            {
                statusCode = e.Status.StatusCode;
                statusDetail = e.Status.Detail;
            }

            // Grpc messages all support empty constructors
            reply ??= Activator.CreateInstance<TReply>();

            return new GrpcReply<TReply>(statusCode, statusDetail, reply);
        }
    }
}
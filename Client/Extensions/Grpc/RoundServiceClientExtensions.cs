using System;
using System.Threading.Tasks;
using Grpc.Core;
using Quibble.Client.Grpc;
using Quibble.Common.Protos;

namespace Quibble.Client.Extensions.Grpc
{
    /// <summary>
    /// Extensions methods for executing remote calls from a <see cref="RoundService.RoundServiceClient"/>.
    /// </summary>
    public static class RoundServiceClientExtensions
    {
        /// <summary>
        /// Creates a round.
        /// </summary>
        /// <param name="client">The <see cref="RoundService.RoundServiceClient"/>.</param>
        /// <param name="title">The title of the round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous creation operation. The task result <see cref="GrpcReply"/> represents the created round's <see cref="RoundInfo"/>.</returns>
        /// <seealso cref="RoundService.RoundServiceClient.CreateAsync(CreateRoundRequest,CallOptions)"/>
        public static Task<GrpcReply<RoundInfo>> CreateAsync(this RoundService.RoundServiceClient client, string title)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (title == null) throw new ArgumentNullException(nameof(title));

            var request = new CreateRoundRequest { Title = title };
            return GrpcClientExtensionHelpers.RunAsync(async () => await client.CreateAsync(request));
        }

        /// <summary>
        /// Gets a <see cref="RoundInfo"/>.
        /// </summary>
        /// <param name="client">The <see cref="RoundService.RoundServiceClient"/>.</param>
        /// <param name="id">The identifier for the round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found round's <see cref="RoundInfo"/>.</returns>
        public static Task<GrpcReply<RoundInfo>> GetInfoAsync(this RoundService.RoundServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new GetEntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(async () => await client.GetInfoAsync(request));
        }

        /// <summary>
        /// Gets a <see cref="RoundFull"/>.
        /// </summary>
        /// <param name="client">The <see cref="RoundService.RoundServiceClient"/>.</param>
        /// <param name="id">The identifier for the round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous get operation. The task result <see cref="GrpcReply"/> represents the found round's <see cref="RoundFull"/>.</returns>
        public static Task<GrpcReply<RoundFull>> GetFullAsync(this RoundService.RoundServiceClient client, string id)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new GetEntityRequest { Id = id };
            return GrpcClientExtensionHelpers.RunAsync(async () => await client.GetFullAsync(request));
        }

        /// <summary>
        /// Updates the title for a round.
        /// </summary>
        /// <param name="client">The <see cref="RoundService.RoundServiceClient"/>.</param>
        /// <param name="id">The identifier for the round.</param>
        /// <param name="newTitle">The new title for the round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result <see cref="GrpcReply"/> represents the updated round's <see cref="RoundInfo"/>.</returns>
        public static Task<GrpcReply> UpdateTitleAsync(this RoundService.RoundServiceClient client, string id, string newTitle)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (newTitle == null) throw new ArgumentNullException(nameof(newTitle));

            var request = new UpdateRoundTitleRequest { Id = id, NewTitle = newTitle };
            return GrpcClientExtensionHelpers.RunAsync(async () => await client.UpdateTitleAsync(request));
        }

        /// <summary>
        /// Updates the state for a round.
        /// </summary>
        /// <param name="client">The <see cref="RoundService.RoundServiceClient"/>.</param>
        /// <param name="id">The identifier for the round.</param>
        /// <param name="newState">The new state for the round.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous update operation. The task result <see cref="GrpcReply"/> represents the updated round's <see cref="RoundInfo"/>.</returns>
        public static Task<GrpcReply> UpdateStateAsync(this RoundService.RoundServiceClient client, string id, RoundState newState)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (id == null) throw new ArgumentNullException(nameof(id));

            var request = new UpdateRoundStateRequest { Id = id, NewState = newState };
            return GrpcClientExtensionHelpers.RunAsync(async () => await client.UpdateStateAsync(request));
        }
    }
}

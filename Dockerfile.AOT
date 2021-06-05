FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0.0-preview.5 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/nightly/sdk:6.0.100-preview.5 AS build
WORKDIR /src

# Emscripten SDK install
RUN echo "## Start building" \
    && echo "## Update and install packages" \
    && apt-get -qq -y update \
    && apt-get -qq install -y --no-install-recommends \
        binutils \
        build-essential \
        ca-certificates \
        file \
        git \
        python3 \
        python3-pip \
    && echo "## Done"

RUN git clone https://github.com/emscripten-core/emsdk.git

RUN echo "## Install Emscripten" \
    && cd emsdk \
    && ./emsdk install latest \
    && echo "## Done"

RUN cd emsdk \
    && echo "## Generate standard configuration" \
    && ./emsdk activate latest \
    && chmod 777 ./upstream/emscripten \
    && chmod -R 777 ./upstream/emscripten/cache \
    && echo "int main() { return 0; }" > hello.c \
    && ./upstream/emscripten/emcc -c hello.c \
    && cat ./upstream/emscripten/cache/sanity.txt \
    && echo "## Done"

# Cleanup Emscripten installation and strip some symbols
RUN echo "## Aggressive optimization: Remove debug symbols" \
    && cd emsdk && . ./emsdk_env.sh \
    # Remove debugging symbols from embedded node (extra 7MB)
    && strip -s `which node` \
    # Tests consume ~80MB disc space
    && rm -fr ./upstream/emscripten/tests \
    # Fastcomp is not supported
    && rm -fr ./upstream/fastcomp \
    # strip out symbols from clang (~extra 50MB disc space)
    && find ./upstream/bin -type f -exec strip -s {} + || true \
    && echo "## Done"

# App build/publish
RUN cd ../
COPY ["Directory.Build.props", ""]
COPY ["DockerNuGet.config", "NuGet.config"]
COPY ["Server/Server.csproj", "Server/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Client/Client.csproj", "Client/"]
COPY ["Client.Synchronisation/Client.Sync.csproj", "Client.Synchronisation/"]
RUN dotnet workload install microsoft-net-sdk-blazorwebassembly-aot --skip-manifest-update
RUN dotnet restore "Server/Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
# /p:PublishTrimmed=true  isn't supported with Blazor hosted model
RUN dotnet publish "Server.csproj" -c Release -o /app/publish /p:RunAOTCompilation=true /p:DebuggerSupport=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuibbleServer.dll"]

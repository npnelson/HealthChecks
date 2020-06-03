#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:3.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["test/NetToolBox.HealthChecks.Functions.TestHarness/NetToolBox.HealthChecks.Functions.TestHarness.csproj", "test/NetToolBox.HealthChecks.Functions.TestHarness/"]
COPY ["src/NetToolBox.HealthChecks.ServiceBus/NetToolBox.HealthChecks.ServiceBus.csproj", "src/NetToolBox.HealthChecks.ServiceBus/"]
COPY ["src/NetToolBox.HealthChecks.Functions/NetToolBox.HealthChecks.Functions.csproj", "src/NetToolBox.HealthChecks.Functions/"]
COPY ["src/NetToolBox.HealthChecks.Core/NetToolBox.HealthChecks.Core.csproj", "src/NetToolBox.HealthChecks.Core/"]
COPY ["src/NetToolBox.HealthChecks.BlobStorage/NetToolBox.HealthChecks.BlobStorage.csproj", "src/NetToolBox.HealthChecks.BlobStorage/"]
COPY ["src/NetToolBox.HealthChecks.AzureFunctionTimer/NetToolBox.HealthChecks.AzureFunctionTimer.csproj", "src/NetToolBox.HealthChecks.AzureFunctionTimer/"]
RUN dotnet restore "test/NetToolBox.HealthChecks.Functions.TestHarness/NetToolBox.HealthChecks.Functions.TestHarness.csproj"
COPY . .
WORKDIR "/src/test/NetToolBox.HealthChecks.Functions.TestHarness"
RUN dotnet build "NetToolBox.HealthChecks.Functions.TestHarness.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetToolBox.HealthChecks.Functions.TestHarness.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
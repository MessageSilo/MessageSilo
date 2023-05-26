#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 11111
EXPOSE 30000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MessageSilo.Silo/MessageSilo.Silo.csproj", "src/MessageSilo.Silo/"]
COPY ["src/MessageSilo.Features/MessageSilo.Features.csproj", "src/MessageSilo.Features/"]
COPY ["src/MessageSilo.Shared/MessageSilo.Shared.csproj", "src/MessageSilo.Shared/"]
RUN dotnet restore "src/MessageSilo.Silo/MessageSilo.Silo.csproj"
COPY . .
WORKDIR "/src/src/MessageSilo.Silo"
RUN dotnet build "MessageSilo.Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageSilo.Silo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageSilo.Silo.dll"]
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MessageSilo/MessageSilo.csproj", "src/MessageSilo/"]
COPY ["src/MessageSilo.Application/MessageSilo.Application.csproj", "src/MessageSilo.Application/"]
COPY ["src/MessageSilo.Infrastructure/MessageSilo.Infrastructure.csproj", "src/MessageSilo.Infrastructure/"]
COPY ["src/MessageSilo.Domain/MessageSilo.Domain.csproj", "src/MessageSilo.Domain/"]
RUN dotnet restore "src/MessageSilo/MessageSilo.csproj"
COPY . .
WORKDIR "/src/src/MessageSilo"
RUN dotnet build "MessageSilo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageSilo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageSilo.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MessageSilo.API/MessageSilo.API.csproj", "src/MessageSilo.API/"]
#COPY ["src/MessageSilo.BlazorApp/MessageSilo.BlazorApp.csproj", "src/MessageSilo.BlazorApp/"]
COPY ["src/MessageSilo.Features/MessageSilo.Features.csproj", "src/MessageSilo.Features/"]
COPY ["src/MessageSilo.Shared/MessageSilo.Shared.csproj", "src/MessageSilo.Shared/"]
RUN dotnet restore "src/MessageSilo.API/MessageSilo.API.csproj"
COPY . .
WORKDIR "/src/src/MessageSilo.API"
RUN dotnet build "MessageSilo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageSilo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageSilo.API.dll"]
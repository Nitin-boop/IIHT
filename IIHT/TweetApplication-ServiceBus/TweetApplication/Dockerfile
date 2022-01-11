#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TweetApplication.csproj", "./"]
RUN dotnet restore "./TweetApplication.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TweetApplication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TweetApplication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TweetApplication.dll"]
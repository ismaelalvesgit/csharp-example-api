#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Example.API/Example.API.csproj", "Example.API/"]
COPY ["Example.Application/Example.Application.csproj", "Example.Application/"]
COPY ["Example.Data/Example.Data.csproj", "Example.Data/"]
COPY ["Example.Domain/Example.Domain.csproj", "Example.Domain/"]
RUN dotnet restore "Example.API/Example.API.csproj"
COPY . .
WORKDIR "/src/Example.API"
RUN dotnet build "Example.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Example.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.API.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Gem.Spaas.Service/Gem.Spaas.Service.csproj", "Gem.Spaas.Service/"]
RUN dotnet restore "Gem.Spaas.Service/Gem.Spaas.Service.csproj"
COPY . .
WORKDIR "/src/Gem.Spaas.Service"
RUN dotnet build "Gem.Spaas.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gem.Spaas.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gem.Spaas.Service.dll"]
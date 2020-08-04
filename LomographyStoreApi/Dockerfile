FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["LomographyStoreApi.csproj", "./"]
RUN dotnet restore "./LomographyStoreApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LomographyStoreApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LomographyStoreApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LomographyStoreApi.dll"]

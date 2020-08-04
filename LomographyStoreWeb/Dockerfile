FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app


# copy csproj and restore as distinct layers
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["LomographyStoreWeb.csproj", "./"]
RUN dotnet restore "./LomographyStoreWeb.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LomographyStoreWeb.csproj" -c Release -o /app/build

# copy and publish app and libraries
FROM build AS publish
RUN dotnet publish "LomographyStoreWeb.csproj" -c Release -o /app/publish

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LomographyStoreWeb.dll"]

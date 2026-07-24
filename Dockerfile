FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["FlowerShop.API.csproj", "./"]
RUN dotnet restore "FlowerShop.API.csproj"
COPY . .
RUN dotnet publish "FlowerShop.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowerShop.API.dll"]

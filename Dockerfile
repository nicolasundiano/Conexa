FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Conexa.sln", "./"]
COPY ["src/Conexa.Api/Conexa.Api.csproj", "src/Conexa.Api/"]
COPY ["src/Conexa.Application/Conexa.Application.csproj", "src/Conexa.Application/"]
COPY ["src/Conexa.Domain/Conexa.Domain.csproj", "src/Conexa.Domain/"]
COPY ["src/Conexa.Infrastructure/Conexa.Infrastructure.csproj", "src/Conexa.Infrastructure/"]
RUN dotnet restore "src/Conexa.Api/Conexa.Api.csproj"

COPY . .
RUN dotnet publish "src/Conexa.Api/Conexa.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Conexa.Api.dll"]

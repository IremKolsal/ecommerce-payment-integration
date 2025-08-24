# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projeyi kopyala
COPY ["src/ECommerce.Api/ECommerce.Api.csproj", "src/ECommerce.Api/"]
COPY ["src/ECommerce.Application/ECommerce.Application.csproj", "src/ECommerce.Application/"]
COPY ["src/ECommerce.Domain/ECommerce.Domain.csproj", "src/ECommerce.Domain/"]
COPY ["src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "src/ECommerce.Infrastructure/"]

# Restore baðýmlýlýklarý
RUN dotnet restore "src/ECommerce.Api/ECommerce.Api.csproj"

# Tüm solution'ý kopyala ve build et
COPY . .
WORKDIR "/src/src/ECommerce.Api"
RUN dotnet publish "ECommerce.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Container'ýn dinleyeceði port
EXPOSE 8080

# Uygulama baþlatma
ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]

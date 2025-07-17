# ---- STAGE 1: Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copia as definicoes de solucao e projetos
COPY PollsApp.sln ./
COPY PollsApp.Api/PollsApp.Api.csproj PollsApp.Api/
COPY PollsApp.Application/PollsApp.Application.csproj PollsApp.Application/
COPY PollsApp.Infrastructure/PollsApp.Infrastructure.csproj PollsApp.Infrastructure/
COPY PollsApp.Domain/PollsApp.Domain.csproj PollsApp.Domain/

# 2) Restaura dependencias (somente da API, ele puxa referencias transitivas)
RUN dotnet restore PollsApp.Api/PollsApp.Api.csproj

# 3) Copia todo o codigo e publica
COPY . .
WORKDIR /src/PollsApp.Api
RUN dotnet publish -c Release -o /app/publish

# ---- STAGE 2: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 1) Copia os binarios publicados
COPY --from=build /app/publish .

# 2) Expoe a porta padrao
ENV ASPNETCORE_URLS=http://+:5232
EXPOSE 5232

# 3) Ambiente = Production
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PollsApp.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Backend/Backend/Backend.csproj ./Backend/Backend/Backend.csproj
RUN dotnet restore "Backend/Backend/Backend.csproj"
COPY Backend/ ./Backend/
RUN dotnet publish "Backend/Backend/Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]

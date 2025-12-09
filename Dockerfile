FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/*.csproj ./src/
RUN dotnet restore src/BarkKomodoAlerter.csproj

COPY src/. ./src/
RUN dotnet publish src/BarkKomodoAlerter.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "BarkKomodoAlerter.dll"]

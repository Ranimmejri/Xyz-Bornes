FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore Xyz.Bornes.Simulateur/Xyz.Bornes.Simulateur.csproj

RUN dotnet publish Xyz.Bornes.Simulateur/Xyz.Bornes.Simulateur.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final

WORKDIR /app

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Xyz.Bornes.Simulateur.dll"]
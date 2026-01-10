# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder
WORKDIR /app

COPY docker/nuget.config ./nuget.config
COPY . .

RUN dotnet restore ./mediflow.sln --configfile ./nuget.config
RUN dotnet publish ./src/API/Mediflow/Mediflow.csproj -c Release -r linux-musl-x64 --self-contained false -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

RUN apk add --no-cache icu-libs krb5-libs libgcc libstdc++ libssl3 zlib ca-certificates && update-ca-certificates

COPY --from=builder /app/publish .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Mediflow.dll"]
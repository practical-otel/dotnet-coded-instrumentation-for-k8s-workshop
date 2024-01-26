FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY src .
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled
WORKDIR /app
COPY --from=build /app .
# 👇 set to use the non-root USER here
ENTRYPOINT ["./src"]
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env

WORKDIR /src
COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.9-alpine3.12
WORKDIR /src
COPY --from=build-env /src/out .
ENTRYPOINT [ "dotnet", "AtheerBackend.dll" ]




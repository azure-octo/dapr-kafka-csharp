# ------------------------------------------------------------
# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.
# ------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY ./producer ./producer
COPY ./models ./models

WORKDIR /app/producer
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY --from=build-env /app/producer/out .
ENTRYPOINT ["dotnet", "producer.dll"]

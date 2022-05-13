# ------------------------------------------------------------
# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.
# ------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY ./consumer ./consumer
COPY ./models ./models

WORKDIR /app/consumer
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY --from=build-env /app/consumer/out .
ENTRYPOINT ["dotnet", "consumer.dll"]

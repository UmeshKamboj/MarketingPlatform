#!/bin/bash

# Script to generate swagger.json from the API project
# This runs the API temporarily and exports the Swagger specification

echo "Generating Swagger JSON documentation..."

cd src/MarketingPlatform.API

# Build the API
dotnet build --no-incremental

# Install swagger CLI tool if not already installed
dotnet tool install -g Swashbuckle.AspNetCore.Cli || true

# Generate swagger.json using the CLI tool
dotnet swagger tofile --output ../../swagger.json bin/Debug/net8.0/MarketingPlatform.API.dll v1

echo "Swagger JSON generated successfully at swagger.json"

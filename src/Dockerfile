# Stage 1: base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
RUN mkdir -p /app/wwwroot/profile-pictures && chmod -R 777 /app/wwwroot

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Copy each csproj to subfolders that match
COPY ["src/WebApi/WebApi.csproj", "WebApi/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "SharedKernel/"]

# Restore, referencing the correct path
RUN dotnet restore "WebApi/WebApi.csproj"

# Copy the entire solution
COPY . .

# Build
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/WebApi
RUN dotnet publish "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: run EF migrations
FROM build AS ef-migration
WORKDIR "/src/Infrastructure"
CMD dotnet ef database update --startup-project ../WebApi

# Stage 5: final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Tạm chuyển sang root để tạo thư mục và copy file
USER root
RUN mkdir -p /Infrastructure/Persistence/Seed
COPY --from=build /src/Infrastructure/Persistence/Seed/course.json /Infrastructure/Persistence/Seed/course.json

# Chuyển lại về user không phải root (nếu cần)
USER $APP_UID

ENTRYPOINT ["dotnet", "WebApi.dll"]


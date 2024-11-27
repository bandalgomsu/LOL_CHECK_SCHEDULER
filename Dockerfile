# 1. 런타임 이미지 설정
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV TZ=Asia/Seoul

# 2. 빌드 이미지 설정
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2-1. 프로젝트 파일 복사 및 복원
COPY ["lol-check-scheduler.csproj", "./"]
RUN dotnet restore "lol-check-scheduler.csproj"

# 2-2. 전체 소스 복사 및 빌드
COPY . .
RUN dotnet publish "lol-check-scheduler.csproj" -c Release -o /app/publish

# 3. 런타임 이미지에 복사
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "lol-check-scheduler.dll"]
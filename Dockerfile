# Bước 1: Sử dụng hình ảnh .NET SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Bước 2: Sao chép tệp .csproj và phục hồi các gói
COPY *.csproj .
RUN dotnet restore

# Bước 3: Sao chép tất cả mã nguồn và xây dựng ứng dụng
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Bước 4: Sử dụng hình ảnh .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Websitecanhan.dll", "--urls", "http://*:8080"]

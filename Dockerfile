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

# Cài đặt các thư viện cần thiết cho AWS CLI (optional)
RUN apt-get update && \
    apt-get install -y curl unzip && \
    curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" && \
    unzip awscliv2.zip && \
    ./aws/install && \
    rm -rf awscliv2.zip aws && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Bước 5: Chỉ định lệnh để chạy ứng dụng
ENTRYPOINT ["dotnet", "Websitecanhan.dll"]
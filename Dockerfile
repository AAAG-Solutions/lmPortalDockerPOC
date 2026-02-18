# escape=`
# Build stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Install NuGet
RUN Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile C:\nuget.exe

# Install MSBuild
RUN setx /M PATH $($Env:PATH + ';C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin')

WORKDIR /app

# Copy project files
COPY *.csproj .
COPY *.sln .
COPY packages.config .

RUN Invoke-WebRequest `
        -Uri 'https://dist.nuget.org/win-x86-commandline/v6.8.0/nuget.exe' `
        -OutFile 'C:\nuget.exe'
 
RUN C:\nuget.exe restore packages.config `
        -PackagesDirectory packages `
        -Verbosity detailed

COPY . .

# Build and publish
RUN msbuild LiquidMotorsWholesale.csproj `
    /p:Configuration=Release `
    /p:DeployOnBuild=true `
    /p:WebPublishMethod=FileSystem `
    /p:publishUrl=C:\publish `
    /p:DeleteExistingFiles=True

# Runtime stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022

WORKDIR /inetpub/wwwroot

# Copy published files from build stage
COPY --from=build C:\publish ./

# Expose port
EXPOSE 80


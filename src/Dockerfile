FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build-env
WORKDIR /publish
COPY *.csproj nuget.config ./
RUN dotnet restore --configfile nuget.config
COPY . .
RUN dotnet publish -c Release --output ./out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
WORKDIR /app
COPY --from=build-env /publish/out .
ENV ASPNETCORE_ENVIRONMENT prod
ENV ASPNETCORE_URLS "http://0.0.0.0:5000"
EXPOSE 5000
ENTRYPOINT ["dotnet", "madpdf.dll"]

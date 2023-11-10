FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY /build .
ENTRYPOINT ["dotnet", "WebApi.dll"]
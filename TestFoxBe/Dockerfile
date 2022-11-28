FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestFoxBe/TestFoxBe.csproj", "TestFoxBe/"]
RUN dotnet restore "TestFoxBe/TestFoxBe.csproj"
COPY . .
WORKDIR "/src/TestFoxBe"
RUN dotnet build "TestFoxBe.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestFoxBe.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TestFoxBe.dll"]

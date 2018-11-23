FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Rexxar.Identity/Rexxar.Identity.csproj", "Rexxar.Identity/"]
RUN dotnet restore "Rexxar.Identity/Rexxar.Identity.csproj"
COPY . .
WORKDIR "/src/Rexxar.Identity"
RUN dotnet build "Rexxar.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Rexxar.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Rexxar.Identity.dll"]
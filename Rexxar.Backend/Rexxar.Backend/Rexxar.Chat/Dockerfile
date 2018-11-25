FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Rexxar.Chat/Rexxar.Chat.csproj", "Rexxar.Chat/"]
RUN dotnet restore "Rexxar.Chat/Rexxar.Chat.csproj"
COPY . .
WORKDIR "/src/Rexxar.Chat"
RUN dotnet build "Rexxar.Chat.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Rexxar.Chat.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Rexxar.Chat.dll"]
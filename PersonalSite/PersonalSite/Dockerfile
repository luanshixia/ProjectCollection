FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["PersonalSite/PersonalSite.csproj", "PersonalSite/"]
RUN dotnet restore "PersonalSite/PersonalSite.csproj"
COPY . .
WORKDIR "/src/PersonalSite"
RUN dotnet build "PersonalSite.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PersonalSite.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PersonalSite.dll"]
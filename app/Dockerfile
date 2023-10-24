FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /App
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
COPY . ./
RUN dotnet restore "NoteAppAPI.csproj"
RUN dotnet publish -c Release -o out

FROM base AS final
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT [ "dotnet", "NoteAppAPI.dll"]
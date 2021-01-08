# set base image as the dotnet 2.1 SDK.
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env

# set the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD
# instructions that follows the WORKDIR instruction.
WORKDIR /app

# our current working directory within the container is /app
# we now copy all the files (from local machine) to /app (in the container).
COPY . ./

# update the ubuntu packages and install curl
RUN apt-get update -yq && apt-get upgrade -yq

# download node
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -

# install nodejs
RUN apt-get install -y nodejs

# install dart sass
RUN npm install -g sass

# install gulp-cli globally
RUN npm install gulp-cli -g

# we run npm for the gulp minification and sass compilation
RUN npm install --prefix ./YeGods.Web/

# run unit tests within the solution.
RUN dotnet test YeGods.sln

# again, on the container (we are in /app folder)
# we now publish the project into a folder called 'out'.
RUN dotnet publish YeGods.Web/YeGods.Web.csproj -c Release -o out

# set base image as the dotnet 2.1 runtime.
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime

# telling the application what port to run on.
ENV ASPNETCORE_URLS=http://*:5005

# set the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD
# instructions that follows the WORKDIR instruction.
WORKDIR /app

# copy the contents of /app/out in the `build-env` and paste it in the
# `/app` directory of the new runtime container.
COPY --from=build-env /app/YeGods.Web/out .

# set the entry point into the application.
ENTRYPOINT ["dotnet", "YeGods.Web.dll"]
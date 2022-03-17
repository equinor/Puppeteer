FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base


# See https://crbug.com/795759
RUN apt-get update && apt-get install -yq libgconf-2-4
RUN apt-get update && apt-get install -y curl
# Install Nodejs
RUN curl -sL https://deb.nodesource.com/setup | bash -
RUN apt-get install -y nodejs


RUN mkdir -p /usr/share/man/man1mkdir -p /usr/share/man/man1
# Install latest chrome dev package and fonts to support major charsets (Chinese, Japanese, Arabic, Hebrew, Thai and a few others)
# Note: this installs the necessary libs to make the bundled version of Chromium that Puppeteer
# installs, work.
ARG CHROME_VERSION="99.0.4844.51-1"
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget --no-verbose -O /tmp/chrome.deb http://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_${CHROME_VERSION}_amd64.deb \
    && apt-get update \
    && apt-get install -y /tmp/chrome.deb --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf \
    && rm /tmp/chrome.deb

# It's a good idea to use dumb-init to help prevent zombie chrome processes.
ADD https://github.com/Yelp/dumb-init/releases/download/v1.2.5/dumb-init_1.2.5_amd64.deb /usr/local/bin/dumb-init
RUN chmod +x /usr/local/bin/dumb-init
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY . .


RUN dotnet restore --configfile nuget.config

RUN dotnet build -c Release -o /app


FROM build AS publish
RUN dotnet publish -c Release -o /app
WORKDIR /app


FROM base AS final
WORKDIR /app
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
COPY --from=publish /app .
RUN npm install
ENTRYPOINT ["dotnet", "madpdf.dll"]
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

RUN apt-get update && apt-get install -y apt-transport-https
# See https://crbug.com/795759
RUN apt-get update && apt-get install -yq libgconf-2-4

# Install Nodejs
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs

#pdf2png support
#https://www.npmjs.com/package/pdf-image
RUN apt-get install -y imagemagick ghostscript poppler-utils

# Install latest chrome dev package and fonts to support major charsets (Chinese, Japanese, Arabic, Hebrew, Thai and a few others)
# Note: this installs the necessary libs to make the bundled version of Chromium that Puppeteer
# installs, work.
RUN apt-get update && apt-get install -y wget gnupg2 pdftk --no-install-recommends \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-unstable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst ttf-freefont \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get purge --auto-remove -y curl \
    && rm -rf /src/*.deb




# It's a good idea to use dumb-init to help prevent zombie chrome processes.
ADD https://github.com/Yelp/dumb-init/releases/download/v1.2.0/dumb-init_1.2.0_amd64 /usr/local/bin/dumb-init
RUN chmod +x /usr/local/bin/dumb-init
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /src
COPY . .


RUN dotnet restore --configfile nuget.config

RUN dotnet build -c Release -o /app


FROM build AS publish
RUN dotnet publish -c Release -o /app
WORKDIR /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN npm install
ENTRYPOINT ["dotnet", "madpdf.dll"]

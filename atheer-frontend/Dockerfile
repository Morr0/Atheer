FROM node:12-alpine

ADD *.json ./
RUN npm install
RUN npm -g install serve

ADD . .

ENTRYPOINT ["serve", "./dist", "-l", "8080", "-s"]

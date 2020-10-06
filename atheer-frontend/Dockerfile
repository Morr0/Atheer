FROM node:12-alpine

ADD *.json ./

RUN npm install
RUN npm -g install serve

ADD . .

RUN npm run build

CMD [ "serve", "./dist", "-l", "8080" ]

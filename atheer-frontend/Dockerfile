FROM node:12-alpine

ADD *.json ./

RUN npm install
RUN npm -g install serve
RUN apk add bash

ADD . .

ADD start.sh ./
RUN chmod +x ./start.sh

ENV VUE_APP_API="https://localhost:5001/"

CMD [ "./start.sh" ]

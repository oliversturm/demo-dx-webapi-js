FROM node:lts

RUN apt-get update
RUN apt-get install -y wait-for-it
RUN npm i -g pnpm

WORKDIR /src
COPY . ./
RUN chmod +x /src/start.sh

CMD ["wait-for-it", "-t", "0", "webapi:5273", "--", "/bin/sh", "/src/start.sh"]


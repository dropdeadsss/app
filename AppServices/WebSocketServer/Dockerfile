FROM node:latest
WORKDIR /app
COPY package-lock.json .
COPY package.json .
RUN npm install
COPY server.js .
EXPOSE 40000
CMD ["node", "server.js"]
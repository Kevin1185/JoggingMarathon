FROM node:22 AS builder

WORKDIR /app

# Copy BOTH package.json AND yarn.lock before install
COPY package.json yarn.lock .yarnrc.yml ./

RUN corepack enable && corepack prepare yarn@4.8.1 --activate
RUN yarn install --immutable

COPY . .

RUN yarn build

# Stage 2: Serve the built app using Nginx
FROM nginx:alpine

# Copy the built React app from the builder stage to Nginx's web server directory
COPY --from=builder /app/dist /usr/share/nginx/html/

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 54545 (Note: This is only for documentation purposes)
EXPOSE 50545

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
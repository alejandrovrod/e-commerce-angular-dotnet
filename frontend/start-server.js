#!/usr/bin/env node

const httpServer = require('http-server');
const path = require('path');

// Get port from environment variable or use default
const port = process.env.PORT || 8080;

console.log(`Starting server on port ${port}`);

// Create and start the server
const server = httpServer.createServer({
  root: path.join(__dirname, 'dist', 'ecommerce-frontend'),
  cache: -1,
  cors: true
});

server.listen(port, '0.0.0.0', () => {
  console.log(`Server running at http://0.0.0.0:${port}/`);
});

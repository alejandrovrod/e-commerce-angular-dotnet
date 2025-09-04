const fs = require('fs');
const path = require('path');

console.log('=== BUILD-ENV.JS DEBUG ===');
console.log('Current working directory:', process.cwd());
console.log('__dirname:', __dirname);

// Leer el archivo de environment
const envPath = path.join(__dirname, 'src', 'environments', 'environment.prod.ts');
console.log('Environment file path:', envPath);

let content = fs.readFileSync(envPath, 'utf8');
console.log('Original content:', content);

// Obtener la URL del API Gateway desde la variable de entorno
const apiGatewayUrl = process.env.API_GATEWAY_URL || process.env.VITE_API_URL || 'https://tu-api-gateway.up.railway.app';
console.log('API Gateway URL from env:', apiGatewayUrl);
console.log('All environment variables:', Object.keys(process.env).filter(key => key.includes('API') || key.includes('GATEWAY')));

// Reemplazar el placeholder con la URL real
const newContent = content.replace(
  '{{PLACEHOLDER_API_URL}}',
  `${apiGatewayUrl}/api`
);

console.log('New content after replacement:', newContent);

// Escribir el archivo actualizado
fs.writeFileSync(envPath, newContent);

console.log(`Environment updated with API Gateway URL: ${apiGatewayUrl}`);
console.log('=== END BUILD-ENV.JS DEBUG ===');

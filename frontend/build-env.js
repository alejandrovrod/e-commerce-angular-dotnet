const fs = require('fs');
const path = require('path');

// Leer el archivo de environment
const envPath = path.join(__dirname, 'src', 'environments', 'environment.prod.ts');
let content = fs.readFileSync(envPath, 'utf8');

// Obtener la URL del API Gateway desde la variable de entorno
const apiGatewayUrl = process.env.API_GATEWAY_URL || process.env.VITE_API_URL || 'https://tu-api-gateway.up.railway.app';

// Reemplazar el placeholder con la URL real
content = content.replace(
  '{{PLACEHOLDER_API_URL}}',
  `${apiGatewayUrl}/api`
);

// Escribir el archivo actualizado
fs.writeFileSync(envPath, content);

console.log(`Environment updated with API Gateway URL: ${apiGatewayUrl}`);

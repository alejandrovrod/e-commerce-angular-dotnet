export const environment = {
  production: true,
  apiUrl: process.env['API_GATEWAY_URL'] || process.env['VITE_API_URL'] || 'https://tu-api-gateway.up.railway.app',
  stripePublishableKey: 'pk_test_your_stripe_key_here',
  features: {
    enableAnalytics: false,
    enablePushNotifications: true,
    enableGeolocation: true,
    enableOfflineMode: false
  },
  cache: {
    enabled: true,
    version: 1,
    ttl: 300000 // 5 minutes
  },
  logging: {
    level: 'info',
    enableConsoleLogging: true,
    enableRemoteLogging: false
  }
};
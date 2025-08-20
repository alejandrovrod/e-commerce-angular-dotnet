export const environment = {
  production: true,
  apiUrl: process.env['API_GATEWAY_URL'] || 'https://api-gateway-production.railway.app/api',
  stripePublishableKey: process.env['STRIPE_PUBLIC_KEY'] || 'pk_live_your_stripe_key_here',
  features: {
    enableAnalytics: true,
    enablePushNotifications: true,
    enableGeolocation: true,
    enableOfflineMode: true
  },
  cache: {
    enabled: true,
    version: 1,
    ttl: 600000 // 10 minutes
  },
  logging: {
    level: 'error',
    enableConsoleLogging: false,
    enableRemoteLogging: true
  },
  railway: {
    environment: 'production',
    region: 'us-west1'
  }
};

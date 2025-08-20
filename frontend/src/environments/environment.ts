export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
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
    level: 'debug',
    enableConsoleLogging: true,
    enableRemoteLogging: false
  }
};

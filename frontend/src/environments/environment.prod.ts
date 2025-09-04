export const environment = {
  production: true,
  apiUrl: '${API_GATEWAY_URL}/api',
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
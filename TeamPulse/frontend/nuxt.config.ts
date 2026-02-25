export default defineNuxtConfig({
  devtools: { enabled: true },
   css: ['~/assets/css/main.css'],
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:7250'
    }
  }
})
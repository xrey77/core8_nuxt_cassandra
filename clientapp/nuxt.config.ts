// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: ['@nuxt/eslint','nuxt-file-storage',],
  nitro: {
    prerender: {
      crawlLinks: true,
      failOnError: false, 
    },
  },    
  css: [
    'bootstrap/dist/css/bootstrap.min.css',
    '@/assets/css/main.css',
  ],  
  app: {
    head: {
      script: [
        { src: 'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js', tagPosition: 'bodyClose' }
      ]
    }
  }  
})
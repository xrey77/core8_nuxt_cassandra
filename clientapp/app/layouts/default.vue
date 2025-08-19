<template>
    <div>
        <Header/>
        <slot />
        <Footer/>
    </div>
  </template>

<script setup>
import Header from './Header.vue';
import Footer from './Footer.vue';

// head () {
//     return {
//       title: 'Test Page - My awesome project',
//       link: [
//         { rel: 'stylesheet', href: '/bootstrap/bootstrap.min.css' }
//       ],
//       script: [
//         { src: "/bootstrap/bootstrap.bundle.min.js", type: "text/javascript"}
//       ]

//     }
//   }

useHead({
  title: 'World Taekwondo',
})

</script>
  

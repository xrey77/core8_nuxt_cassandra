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

useHead({
  title: 'World Taekwondo',
})

</script>
  

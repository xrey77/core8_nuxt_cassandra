<template>
<div class="container-fluid">
    <nav class="navbar navbar-expand-lg bg-white">
        <div class="container-fluid">
          <NuxtLink class="navbar-brand" to="/"><img class="logo" src="/assets/images/logo.png" alt=""></NuxtLink>
  <!-- <button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions" aria-label="Close"></button> -->

          <button class="navbar-toggler" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions" aria-label="Close">
            <!-- <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions" aria-expanded="false" aria-label="Toggle navigation"> -->

            <span class="navbar-toggler-icon"></span>
          </button>
          <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav me-auto mb-2 mb-lg-0">
              <li class="nav-item">
                <NuxtLink class="nav-link active" aria-current="page" to="/aboutus" no-prefetch>About Us</NuxtLink>
              </li>
              <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                  Products
                </a>
                <ul class="dropdown-menu">
                  <li><NuxtLink class="dropdown-item" href="/products/list">Product List</NuxtLink></li>
                  <li><NuxtLink class="dropdown-item" href="/products/catalog">Products Catalog</NuxtLink></li>
                  <li><hr class="dropdown-divider"></li>
                  <li><NuxtLink class="dropdown-item" href="/products/search">Product Search</NuxtLink></li>
                </ul>
              </li>
              <li class="nav-item">
                <NuxtLink class="nav-link" to="/contactus" aria-disabled="true">Contact Us</NuxtLink>
              </li>
            </ul>

            <div v-if="data.username.length !== 0">
              <ul class="navbar-nav mr-auto">
                <li class="nav-item dropdown">
                  <a class="nav-link dropdown-toggle text-dark" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <img class="user" :src="data.userpic" alt=""/>
                    {{ data.username }}
                  </a>
                  <ul class="dropdown-menu">
                    <li><a @click="logout" class="dropdown-item" href="/#">LogOut</a></li>
                    <li><NuxtLink class="dropdown-item" to="/userprofile">Profile</NuxtLink></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><NuxtLink class="dropdown-item" to="/#">Messenger</NuxtLink></li>
                  </ul>
                </li>
              </ul>
            </div>
            <ul v-else class="navbar-nav mr-auto">
                <li class="nav-item">
                  <NuxtLink class="nav-link active text-dark" to="/#" data-bs-toggle="modal" data-bs-target="#staticLogin">Login</NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink class="nav-link active text-dark" to="/#" data-bs-toggle="modal" data-bs-target="#staticRegister">Register</NuxtLink>
                </li>        
            </ul>            
          </div>
        </div>
      </nav>
<!-- start-DRAWER MENU       -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasWithBothOptions" aria-labelledby="offcanvasWithBothOptionsLabel">
  <div class="offcanvas-header bg-primary">
    <h5 class="offcanvas-title text-white" id="offcanvasWithBothOptionsLabel">Drawer Menu</h5>
  <button type="button" class="btn-close btn-close-white" data-bs-dismiss="offcanvas" aria-label="Close"></button>
  </div>
  <div class="offcanvas-body">

    <ul class="nav flex-column">
      <li class="nav-item" data-bs-dismiss="offcanvas">
        <NuxtLink class="nav-link active" data-bs-dismiss="offcanvas" to="/aboutus">About Us</NuxtLink>
      </li>
      <li><hr/></li>   

      <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
          Products
        </a>
        <ul class="dropdown-menu">
          <li><NuxtLink class="dropdown-item" href="/products/list">Product List</NuxtLink></li>
          <li><NuxtLink class="dropdown-item" href="/products/catalog">Products Catalog</NuxtLink></li>
          <li><hr class="dropdown-divider"></li>
          <li><NuxtLink class="dropdown-item" href="/products/search">Product Search</NuxtLink></li>
        </ul>
      </li>

      <li><hr/></li>   

      <li class="nav-item" data-bs-dismiss="offcanvas">
        <NuxtLink class="nav-link active" to="/contactus">Contact Us</NuxtLink>  
      </li>
      <li><hr/></li>

      <div v-if="data.username">
        <ul class="nav">

        <li class="nav-item dropdown">
          <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
            <img class="user" :src="data.userpic" />&nbsp;{{data.username}}
          </a>
          <ul class="dropdown-menu">
            <li data-bs-dismiss="offcanvas"><NuxtLink @click="logout" class="dropdown-item" to="#">Logout</NuxtLink></li>
            <li data-bs-dismiss="offcanvas"><NuxtLink class="dropdown-item" to="/userprofile">Profile</NuxtLink></li>
            <li><hr class="dropdown-divider"></li>
            <li data-bs-dismiss="offcanvas"><NuxtLink class="dropdown-item" to="/#">Messenger</NuxtLink></li>
          </ul>
        </li>
      </ul>          
      <li><hr/></li>

      </div>

      <div v-if="!data.username">
        <ul class="nav flex-column">

        <li data-bs-dismiss="offcanvas" class="nav-item">
          <a class="nav-link active" aria-current="page" href="#" data-bs-toggle="modal" data-bs-target="#staticLogin">Login</a>
        </li>
        <li><hr/></li>

        <li data-bs-dismiss="offcanvas" class="nav-item">
          <a class="nav-link active" aria-current="page" href="#" data-bs-toggle="modal" data-bs-target="#staticRegister">Register</a>
        </li>     
        <li><hr/></li>
               
        </ul>
      </div>

    </ul>

  </div>
<!-- end-DRAWER MENU -->
</div>


<Login/>
<Register/>
</div>
</template>


<script setup lang="ts">
  import Login from '../auth/login.vue';
  import Register from '../auth/register.vue';
  import {reactive, onMounted} from 'vue'
  import { reloadNuxtApp } from '#app';

    const data = reactive({
        username: '',
        userpic: ''
      });

      onMounted(() => {
        const usrname = sessionStorage.getItem('USERNAME');
        if (usrname) {
          data.username = usrname;
        } else {
          data.username = '';
        }
        const usrpic = sessionStorage.getItem('USERPIC');
        if (usrpic) {
          data.userpic = usrpic;
        } else {
          data.userpic = '';
        }
      });

      function logout(e) {
          e.preventDefault();
          sessionStorage.removeItem('USERNAME');
          sessionStorage.removeItem('USERPIC');
          sessionStorage.removeItem('USERID');
          sessionStorage.removeItem('TOKEN');
          window.setTimeout(() => {
            reloadNuxtApp({path: '/'});
          },1000);
      }

</script>

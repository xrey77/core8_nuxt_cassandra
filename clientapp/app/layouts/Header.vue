<template>
<div class="container-fluid">
    <nav class="navbar navbar-expand-lg bg-white">
        <div class="container-fluid">
          <NuxtLink class="navbar-brand" to="/"><img class="logo" src="/assets/images/logo.png" alt=""></NuxtLink>

          <!-- <button class="btn btn-primary" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions">Enable both scrolling & backdrop</button> -->

          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions" aria-expanded="false" aria-label="Toggle navigation">          
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
                  <li><a class="dropdown-item" href="#">Product List</a></li>
                  <li><a class="dropdown-item" href="#">Products Catalog</a></li>
                  <li><hr class="dropdown-divider"></li>
                  <li><a class="dropdown-item" href="#">Product Search</a></li>
                </ul>
              </li>
              <li class="nav-item">
                <NuxtLink class="nav-link" to="/contactus" aria-disabled="true">Contact Us</NuxtLink>
              </li>
            </ul>

            <ul class="navbar-nav mr-auto">
                <li class="nav-item">
                    <a href="#" class="nav-link" aria-disabled="true" data-bs-toggle="modal" data-bs-target="#staticLogin">Login</a>
                  </li>
                  <li class="nav-item">
                    <a href="#" class="nav-link" aria-disabled="true" data-bs-toggle="modal" data-bs-target="#staticRegister">Register</a>
                  </li>
    
            </ul>
          </div>
        </div>
      </nav>
<!-- DRAWER MENU       -->

<div class="offcanvas offcanvas-start" data-bs-scroll="true" tabindex="-1" id="offcanvasWithBothOptions" aria-labelledby="offcanvasWithBothOptionsLabel">
  <div class="offcanvas-header">
    <h5 class="offcanvas-title" id="offcanvasWithBothOptionsLabel">Backdrop with scrolling</h5>
    <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
  </div>
  <div class="offcanvas-body">
    <p>Try scrolling the rest of the page to see this option in action.</p>
  </div>
</div>
<Login/>
<Register/>
</div>
</template>

<script setup>
import Login from '../auth/login.vue';
import Register from '../auth/register.vue';
</script>
<template>
<div>
  <Mfa/>
<div class="modal fade" id="staticLogin" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticLoginLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered modal-sm">
    <div class="modal-content">
      <div class="modal-header bg-primary">
        <h1 class="modal-title fs-5 text-light" id="staticLoginLabel">User Login</h1>
        <button @click="closeLogin" type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <form @submit.prevent="submitLogin" autocomplete="off">
        <div class="mb-3">
        <input type="text" v-model="formData.username" required class="form-control" id="txtUsername" :disabled="formData.isDisabled" placeholder="enter Username"/>
      </div>        
      <div class="mb-3">
        <input type="password" v-model="formData.password" required class="form-control mt-1" id="txtPassword" :disabled="formData.isDisabled" placeholder="enter Password"/>
      </div>        
        <button type="submit" class="btn btn-primary" :disabled="formData.isDisabled">login</button>
        <button id="mfaModal" type="button" class="btn btn-warning d-none" data-bs-toggle="modal" data-bs-target="#staticMFA">MFA</button>
      </form>        
      </div>
      <div class="modal-footer">
        <div class="w-100 text-center text-danger">{{formData.loginMessage}}</div>
      </div>
    </div>
  </div>
</div>
</div>

</template>

<script setup lang="ts">
import $ from 'jquery';
import {reactive} from 'vue'
import { reloadNuxtApp } from '#app';
import Mfa from './Mfa.vue';
import axios from 'axios';

const api = axios.create({
    baseURL: "https://localhost:7100",
    headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
})

const formData = reactive({
    username: '',
    password: '',
    loginMessage: '',
    isDisabled: false
});

function closeLogin() {
    formData.username = '';
    formData.password = '';
    formData.loginMessage = '';
    $("#loginReset").click();
}

async function submitLogin(e: any) {
    e.preventDefault()
    formData.isDisabled=true;
    formData.loginMessage = 'Please wait...';
    const data =JSON.stringify({ username: formData.username, password: formData.password });
    api.post("/signin", data)
    .then((res) => {
            formData.loginMessage = res.data.message;
            if (res.data.qrcodeurl !== null) {
                sessionStorage.setItem('USERID', res.data.id);
                sessionStorage.setItem('TOKEN', res.data.token);
                sessionStorage.setItem('ROLE', res.data.roles);
                sessionStorage.setItem('USERPIC', res.data.profilepic);
                formData.username = '';
                formData.password = '';
                formData.loginMessage = '';
                $("#loginReset").click();
                $("#mfaModal").click();
            } else {
                sessionStorage.setItem('USERID', res.data.id);
                sessionStorage.setItem('USERNAME', res.data.username);
                sessionStorage.setItem('TOKEN', res.data.token);
                sessionStorage.setItem('ROLE', res.data.roles);
                sessionStorage.setItem('USERPIC', res.data.profilepic);
                $("#loginReset").click();
                window.setTimeout(function() {
                    formData.loginMessage = '';
                    formData.username = '';
                    formData.password = '';
                    reloadNuxtApp({path: '/'})
                }, 3000);
            }
            return;
        }, (error: any) => {
            formData.loginMessage = error.response.data.message;
            window.setTimeout(() => {
                formData.loginMessage = '';
                formData.password = '';
                formData.username = '';
                formData.isDisabled=false;
            }, 3000);
            return;
    });
}
</script>
<template>
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
        <input type="text" v-model="formData.username" required class="form-control" id="txtUsername" placeholder="enter Username"/>
      </div>        
      <div class="mb-3">
        <input type="password" v-model="formData.password" required class="form-control mt-1" id="txtPassword" placeholder="enter Password"/>
      </div>        
        <button type="submit" class="btn btn-primary">login</button>
      </form>        
      </div>
      <div class="modal-footer">
        <div id="loginMsg" class="w-100 text-center msg">{{formData.loginMessage}}</div>
      </div>
    </div>
  </div>
</div>
</template>

<script setup lang="ts">
import nuxtStorage from 'nuxt-storage';
import $ from 'jquery';
import {reactive, onMounted} from 'vue'
import axios from 'axios';

const api = axios.create({
    baseURL: "https://localhost:7100",
    headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
})

export interface User {
    id: number,
    token: string,
    roles: string,
    profilepic: string
    statuscode: number,
    message: string
}

const formData = reactive({
    username: '',
    password: '',
    loginMessage: ''
});

// onMounted(() => {
//     function isDisabled() {
//             return !this.activate;
//         }
// })

function closeLogin() {
    formData.username = '';
    formData.password = '';
    $("#loginReset").click();
}

function checkIfImageExists(url, callback) {
    const img = new Image();
    img.src = url;

    if (img.complete) {
    callback(true);
    } else {
    img.onload = () => {
        callback(true);
    };

    img.onerror = () => {
        callback(false);
    };
    }
}

async function submitLogin(e) {
    e.preventDefault()
    // formData.isDisabled=true;
    formData.loginMessage = 'Please wait...';
    const data =JSON.stringify({ username: formData.username, password: formData.password });
    api.post("/signin", data)
    .then((res) => {
        if (res.data.statuscode == 200) {
            formData.loginMessage = res.data.message;
            if (data.qrcodeurl !== null) {
                sessionStorage.setItem('USERID', res.data.id, 8, 'h');
                sessionStorage.setItem('TOKEN', res.data.token, 8, 'h');
                sessionStorage.setItem('ROLE', res.data.roles, 8, 'h');
                sessionStorage.setItem('USERPIC', res.data.profilepic, 8, 'h');
                formData.username = '';
                formData.password = '';
                formData.loginMessage = '';
                $("#loginReset").click();
                $("#mfaModal").click();
            } else {
                sessionStorage.setItem('USERID', res.data.id, 8, 'h');
                sessionStorage.setItem('USERNAME', res.data.username, 8, 'h');
                sessionStorage.setItem('TOKEN', res.data.token, 8, 'h');
                sessionStorage.setItem('ROLE', res.data.roles, 8, 'h');
                sessionStorage.setItem('USERPIC', res.data.profilepic, 8, 'h');
                formData.username = '';
                formData.password = '';
                // formData.loginMessage = '';
                $("#loginReset").click();
                window.setTimeout(function() {
                    window.location.reload();
                }, 3000);
            }
            return;
        } else {
            $('input').prop('disabled', false);
            formData.loginMessage = data.message;
            window.setTimeout(() => {
                formData.loginMessage = '';
                formData.password = '';
            }, 3000);
            return;
        }
        }, (error) => {
            $('input').prop('disabled', false);
            formData.loginMessage = error.message;
            window.setTimeout(() => {
                formData.loginMessage = '';
            }, 3000);
            return;
    });
}
</script>
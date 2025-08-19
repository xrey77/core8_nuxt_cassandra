<template>
    <div class="modal fade" id="staticMFA" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticMFALabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
            <div class="modal-header bg-warning">
                <h1 class="modal-title fs-5 text-white" id="staticMFALabel">2-Factor Authenticator</h1>
                <button @click="closeMFA" type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body" autocomplete="off">
                <form @submit.prevent="submitOTP">
                    <div class="mb-3">
                        <input type="text" v-model="mfaForm.otpcode" :disabled="mfaForm.isDisabled" required class="form-control" placeholder="enter 6 digits OTP Code"/>
                    </div>
                    <button type="submit" :disabled="mfaForm.isDisabled" class="btn btn-warning text-white">submit</button>
                    <button id="mfaReset" type="reset" class="hide-btn d-none">reset</button>
                </form>
            </div>
            <div class="modal-footer">
                <div class="w-100 text-center text-danger">{{ mfaForm.Message}}</div>
            </div>
            </div>
        </div>
        </div>
</template>

<script setup lang="ts">
    import {reactive} from 'vue'
    import { reloadNuxtApp } from '#app';    
    import $ from 'jquery';
    import axios from 'axios';

const api = axios.create({
    baseURL: "https://localhost:7100",
    headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
})

    const mfaForm = reactive({
        otpcode: '',
        Message: '',
        isDisabled: false
    });

    function clearSession() {
        sessionStorage.removeItem('USERID');
        sessionStorage.removeItem('USERNAME');
        sessionStorage.removeItem('TOKEN');
        sessionStorage.removeItem('ROLE');
        sessionStorage.removeItem('USERPIC');
    }
    function closeMFA(e: any) {
        e.preventDefault();
        mfaForm.otpcode = '';
        mfaForm.Message = '';
        $("#mfaReset").click();
        clearSession();
    }

    async function submitOTP(e: any) {
        e.preventDefault();
        mfaForm.isDisabled=true;
        mfaForm.Message = 'please wait..';
        const userId = sessionStorage.getItem('USERID');
        const data =JSON.stringify({id: userId, otp: mfaForm.otpcode});
            api.post("/api/validateotp", data)
            .then((res) => {
                mfaForm.Message = res.data.message;
                  window.setTimeout(() => {
                    mfaForm.Message = '';
                    sessionStorage.setItem('USERNAME', res.data.username);   
                    reloadNuxtApp({path: '/'});
                  }, 3000);
            }, (error: any) => {
                mfaForm.Message = error.response.data.message;
                    window.setTimeout(() => {
                        mfaForm.isDisabled=false;
                        mfaForm.Message = '';
                        mfaForm.otpcode = '';
                  }, 3000);
            });        
    }
</script>

<style scoped>
  .msg {
    font-size: 12px;
    color: red;
  }
</style>


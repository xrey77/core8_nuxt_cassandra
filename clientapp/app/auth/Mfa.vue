<template>
    <div class="modal fade" id="staticMFA" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticMFALabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
            <div class="modal-header bg-primary">
                <h1 class="modal-title fs-5 text-white" id="staticMFALabel">2-Factor Authenticator</h1>
                <button @click="closeMFA" type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body" autocomplete="off">
                <form @submit.prevent="submitOTP">
                    <div class="mb-3">
                        <input type="number" v-model="mfa.otpcode" :disabled="mfa.isDisabled" required class="form-control" placeholder="enter 6 digits OTP Code"/>
                    </div>
                    <button type="submit" :disabled="mfa.isDisabled" class="btn btn-primary">submit</button>
                    <button id="mfaReset" type="reset" class="hide-btn">reset</button>
                </form>
            </div>
            <div class="modal-footer">
                <div class="w-100 text-center msg">{{ mfa.Message}}</div>
            </div>
            </div>
        </div>
        </div>
</template>

<script setup lang="ts">
    import {reactive, onMounted} from 'vue'
    import nuxtStorage from 'nuxt-storage';
    import $ from 'jquery';

    const mfa = reactive({
        otpcode: '',
        Message: '',
        isDisabled: false
    });

    function closeMFA(e: any) {
        e.preventDefault();
        mfa.otpcode = '';
        mfa.Message = '';
        nuxtStorage.localStorage.clear();
        $("#mfaReset").click();
    }

    async function submitOTP(e: any) {
        e.preventDefault();
        let userId = nuxtStorage.localStorage.getData('USERID');
        mfa.isDisabled=true;
        mfa.Message = 'please wait..';
        const data = await $fetch('/api/auth/validatetoken', {
                headers: {
                    'Content-Type': 'application/json'
                },
                method: 'POST',
                body: {
                    id: userId,
                    otpcode: mfa.otpcode
                }

            }).catch( (error: any) => {
                mfa.Message(error.message);
                window.setTimeout(() => {
                    mfa.isDisabled=false;
                    mfa.Message = "";
                },3000);
                return;
            });
            if (data.statuscode === 200) {
                    mfa.Message = data.message;
                    nuxtStorage.localStorage.setData('USERNAME', data.username, 4, 'h');
                    window.setTimeout(() => {
                        reloadNuxtApp({path: "/"});
                    },3000);

            } else {
                mfa.Message = data.message;
            }
            window.setTimeout(() => {
                mfa.Message = "";
                mfa.isDisabled=false;
            },3000);
    }
</script>

<style scoped>
  .msg {
    font-size: 12px;
    color: red;
  }
</style>

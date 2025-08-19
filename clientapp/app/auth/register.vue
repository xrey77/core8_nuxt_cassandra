<template>
  <div class="modal fade" id="staticRegister" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticRegisterLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header bg-success">
          <h1 class="modal-title fs-5 text-light" id="staticRegisterLabel">Register Account</h1>
          <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
          <form id="registrationForm" @submit.prevent="submitRegistration" autocomplete="off">
          <div class="mb-3">
              <div class="row">
                <div class="col">
                  <input type="text" v-model="regForm.firstname" required class="form-control" id="txtFirstname" placeholder="enter Firstname">
                </div>
                <div class="col">
                  <input type="text" v-model="regForm.lastname" required class="form-control" id="txtLastname" placeholder="enter Lastname">
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <input type="email" v-model="regForm.email" required class="form-control mt-1" id="txtEmail" placeholder="enter Email Address">
                </div>
                <div class="col">
                  <input type="text" v-model="regForm.mobile" required class="form-control mt-1" id="txtMobile" placeholder="enter Mobile No.">
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <input type="text" v-model="regForm.username" required class="form-control mt-2" id="txtUsername" placeholder="enter Username">
                </div>
                <div class="col">
                  <input type="password" v-model="regForm.password" required class="form-control mt-1" id="txtPassword" placeholder="enter Password">
                </div>
              </div>
              <button type="submit" class="btn btn-success mt-3 mx-2">register</button>
              <button type="reset" class="btn btn-success mt-3">reset</button>
            </div>        
      
          </form>
        </div>
        <div class="modal-footer">
          <div id="regMsg" class="w-100 text-center msg">{{regForm.registerMsg}}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import $ from 'jquery';
import {reactive} from 'vue'
import axios from 'axios';

const api = axios.create({
    baseURL: "https://localhost:7100",
    headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
})

    const regForm = reactive({
                lastname: '',
                firstname: '',
                email: '',
                mobile: '',
                username: '',
                password: '',
                registerMsg: ''                
            });

        function closeRegistration() {
          regForm.lastname = '';
          regForm.firstname = '';
          regForm.email = '';
          regForm.mobile = '';
          regForm.username = '';
          regForm.password = '';
          regForm.registerMsg = '';
          $("#regReset").click();
        }

        async function submitRegistration(e: any) {
            e.preventDefault();
            regForm.registerMsg = "please wait..";
            const data =JSON.stringify({ 
              lastname: regForm.lastname, firstname: regForm.firstname,
              email: regForm.email, mobile: regForm.mobile,
              username: regForm.username, password: regForm.password });
            api.post("/signup", data)
            .then((res) => {
                  regForm.registerMsg = res.data.message;
                  window.setTimeout(() => {
                    regForm.registerMsg = '';
                  }, 3000);
            }, (error: any) => {
                    regForm.registerMsg = error.response.data.message;
                    window.setTimeout(() => {
                    regForm.registerMsg = '';
                  }, 3000);
            });
        }
</script>
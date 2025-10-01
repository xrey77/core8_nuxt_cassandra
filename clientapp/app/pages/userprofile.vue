<template>
<div class="container">
    <div class="card mt-3 mb-4">
        <div class="card-header">
            <h4>USER PROFILE</h4>
        </div>

        <div class="card-body">
        
            <form enctype="multipart/form-data">
            <div class="row">
                <div class="col">
                       <div class="mb-3">
                            <input type="text" v-model="userData.firstname" required class="form-control" placeholder="enter First Name">
                       </div>
                       <div class="mb-3">
                            <input type="text" v-model="userData.lastname" required class="form-control" placeholder="enter Last Name">
                       </div>
                       <div class="mb-3">
                            <input type="email" v-model="userData.email" class="form-control" :disabled="true">
                       </div>
                       <div class="mb-3">
                            <input type="text" v-model="userData.mobile" required class="form-control" placeholder="enter Mobile No.">
                       </div>                    
                </div>
                <div class="col">

                    <div class="mb-3 text-left">
                        <img class="userpic" :src="userData.profilepic" alt="profilepic" />
                    </div>
                    <div class="mb-3 text-left">
                        <input @change="changeProfilepic" class="form-control form-control-sm" id="userpic" type="file" accept=".png, .jpg, .jpeg, .gif">
                    </div>

                </div>
            </div>

            <div class="row">

                <div class="col">
                    <div class="form-check">
                        <input @change="changePassword" class="form-check-input" type="checkbox" value="" id="changepwd">
                        <label class="form-check-label" for="changepwd">
                          Change Password
                        </label>
                    </div>
                    <div id="cpwd">
                          <div class="mb-3">
                            <input type="password" required class="form-control" v-model="userData.password" placeholder="enter new Password" autocomplete="off">
                          </div>
                          <div class="mb-3">
                            <input type="password" required class="form-control" v-model="userData.confpassword" placeholder="confirm new Password" autocomplete="off">
                          </div>
                          <button @click="changePwd" type="button" class="btn btn-primary">change password</button>
                    </div>
                    <div id="mfa1">
                        <div v-if="userData.qrcodeurl">
                            <img class="qrcode1" :src="userData.qrcodeurl" alt="qrcodeurl"/>
                        </div>
                        <div v-else>
                            <img class="qrcode2" src="/images/qrcode.png" alt="QRCODE" />
                        </div>
                    </div>
                </div>

                <div class="col">
                    <div class="form-check">
                        <input @change="onetimePassword" class="form-check-input" type="checkbox" value="" id="twofactor">
                        <label class="form-check-label" for="twofactor">
                             2-Factor Authenticator
                        </label>
                    </div>
                    <div id="mfa2">
                        <p class='text-danger'><strong>Requirements</strong></p>
                        <p>You need to install <strong>Google or Microsoft Authenticator</strong> in your Mobile Phone, once installed, click Enable Button below, and <strong>SCAN QR CODE</strong>, next time you login, another dialog window will appear, then enter the <strong>OTP CODE</strong> from your Mobile Phone in order for you to login.</p>
                        <div class="row">
                            <div class="col btn-1">
                                <button @click="enableMFA" type="button" class="btn btn-primary">Enable</button>
                            </div>
                            <div class="col col-3 btn-2">
                                <button @click="disableMFA" type="button" class="btn btn-secondary">Disable</button>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
                <div v-if="!userData.showSave">
                    <button @click="submitProfileForm" id="save" type="submit" class="btn btn-primary">save</button>
                </div>            
            </form>
        </div>
        <div class="card-footer text-danger fsize-12">
            {{userData.profileMsg}}
        </div>
    </div>

</div>
</template>

<script setup lang="ts">
    import $ from 'jquery';
    import { reloadNuxtApp } from '#app';
    import {reactive, ref, onMounted} from 'vue'
    import axios from 'axios';

    const api = axios.create({
    baseURL: "https://localhost:7100",
    headers: {'Accept': 'application/json',
                'Content-Type': 'application/json'}
    })

    const userData = reactive({
        firstname: '',
        lastname: '',
        email: '',
        mobile: '',
        password: '',
        confpassword: '',
        profileMsg: 'loading, please wait..',
        qrcodeurl: '',
        profilepic: '',
        userId: '',
        token: '',
        showSave: false
    });

    const selectedFile = ref<File | null>(null);

    async function fetchData(idno: any) {
        await api.get(`/api/getbyid/${idno}`, { headers: {
            Authorization: `Bearer ${userData.token}`
        }} )
            .then((res: any) => {
                userData.firstname = res.data.user.firstName;
                userData.lastname = res.data.user.lastName;
                userData.email = res.data.user.email;
                userData.mobile = res.data.user.mobile;
                userData.profilepic = res.data.user.profilepic;
                userData.qrcodeurl = res.data.user.qrcodeurl;
                userData.profileMsg = res.data.user.message;
                    // $("#userpic").attr('src',user.profilepic);
              }, (error: any) => {
                    userData.profileMsg = error.response.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = "";
                    },3000);
            });                
    }

    onMounted(() => {
        const _usrid = sessionStorage.getItem('USERID');
        if (_usrid !== null) {
            userData.userId = _usrid
        } else {
            userData.userId = '';
        }
        const _token = sessionStorage.getItem("TOKEN");
        if (_token !== null) {
            userData.token = _token
        } else {
            userData.token = '';
        }

        $("#cpwd").hide();
        $("#mfa1").hide();
        $("#mfa2").hide();
        fetchData(userData.userId);
    });

    async function submitProfileForm(e: any) {
        e.preventDefault();
        userData.profileMsg = 'please wait...';
        const data =JSON.stringify({ lastname: userData.lastname, 
                firstname: userData.firstname, mobile: userData.mobile });

            await api.patch(`/api/updateprofile/${userData.userId}`, data, { headers: {
            Authorization: `Bearer ${userData.token}`
            }} )
            .then((res: any) => {
                    userData.profileMsg = res.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
              }, (error: any) => {
                    userData.profileMsg = error.response.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
            }); 
    }

    function changePassword() {
     if ($('#changepwd').is(":checked")) {
        $("#cpwd").show();
        $("#mfa1").hide();
        $("#mfa2").hide();
        $('#twofactor').prop('checked', false);
        userData.showSave = true;
     } else {
        $("#cpwd").hide();
        userData.password = '';
        userData.confpassword = '';
        userData.showSave = false;
     }
    }

    function onetimePassword() {
        if ($('#twofactor').is(":checked")) {
            $("#cpwd").hide();
            $("#mfa1").show();
            $("#mfa2").show();
            $('#changepwd').prop('checked', false);
            userData.showSave = true;
        } else {
            $("#mfa1").hide();
            $("#mfa2").hide();
            userData.showSave = false;
        }
    }

    async function enableMFA(e: any) {
        e.preventDefault();
        const data =JSON.stringify({ Twofactorenabled: true });
            await api.patch(`/api/enablemfa/${userData.userId}`, data, { headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${userData.token}`
            }} )
            .then((res: any) => {
                userData.qrcodeurl = res.data.qrcode;
                userData.profileMsg = res.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
              }, (error: any) => {
                userData.profileMsg = error.response.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
            });        
    }

    async function changePwd(e: any) {
        e.preventDefault();
        if (userData.password === '') {
            userData.profileMsg = "Please enter new password.";
            window.setTimeout(() => {
                    userData.profileMsg = "";
                },3000);
            return;
        }
        if (userData.confpassword === '') {
            userData.profileMsg = "Please confirm new password.";
            window.setTimeout(() => {
                    userData.profileMsg = "";
                },3000);
            return;
        }

        if (userData.password !== '' && userData.confpassword) {
            if (userData.password !== userData.confpassword) {
                userData.profileMsg = "New Password does not matched.";
                window.setTimeout(() => {
                    userData.profileMsg = "";
                },3000);
            }
        }
        else
        {
            userData.profileMsg = 'please enter password';
            window.setTimeout(() => {
                userData.profileMsg = "";
            },3000);
        }
        const data =JSON.stringify({ password: userData.password });
            await api.patch(`/api/updatepassword/${userData.userId}`, data, { headers: {
            Authorization: `Bearer ${userData.token}`
            }} )
            .then((res) => {
                if (res.data.statuscode == 200) {
                    userData.profileMsg = res.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
                    return;
                } else {
                    userData.profileMsg = res.data.message;
                  window.setTimeout(() => {
                    userData.profileMsg = '';
                    }, 3000);
                    return;
                }
              }, (error: any) => {
                userData.profileMsg = error.response.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
            });
    }

    async function disableMFA(e: any) {
        e.preventDefault();
        const data =JSON.stringify({ Twofactorenabled: false });
            await api.patch(`/api/enablemfa/${userData.userId}`, data, { headers: {
                Authorization: `Bearer ${userData.token}`
            }} )
            .then((res: any) => {
                userData.qrcodeurl = '';
                userData.profileMsg = res.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
              }, (error: any) => {
                userData.profileMsg = error.response.data.message;
                    window.setTimeout(() => {
                        userData.profileMsg = '';
                    }, 3000);
            });              
    }

    async function changeProfilepic(event: any) {
        $("#pix").attr('src',URL.createObjectURL(event.target.files[0]));
        const file = event.target.files[0];
        const formdata = new FormData();
        formdata.append('myImage', file);

        const target = event.target as HTMLInputElement;
            if (target.files && target.files.length > 0) {
                selectedFile.value = target.files[0];
                $("#userpic").attr('src',URL.createObjectURL(selectedFile.value));
            }

            if (selectedFile.value) {
                let formdata = new FormData();
                formdata.append('Id', userData.userId);
                formdata.append('Profilepic', selectedFile.value);

                await api.post("/api/uploadpicture", formdata, { headers: {
                'Content-Type': 'multipart/form-data',
                Authorization: `Bearer ${userData.token}`
                }} )
                .then((res: any) => {
                        userData.profileMsg = res.data.message;
                        window.setTimeout(() => {
                            sessionStorage.setItem('USERPIC',res.data.profilepic);
                            userData.profileMsg = '';
                            reloadNuxtApp({path: '/'})
                        }, 3000);
                }, (error: any) => {
                    userData.profileMsg = error.response.data.message;
                        window.setTimeout(() => {
                            userData.profileMsg = '';
                        }, 3000);
                });
            }
        }
</script>

<style scoped>

.userpic {
    width: 150px;
    height: 150px;
}
.fsize-12 {
    font-size: 12px;
}
.qrcode1 {
  float: right;
  width: 200px;
  height: 200px;
}
.qrcode2 {
  float: right;
  width: 200px;
  height: 200px;
  opacity: 0.5;
}
.btn-1 {
  max-width: 100px!important;
}
.btn-2 {
  float: left!important;
}
#save {
  margin-top: 30px!important;
}
@media (max-width: 575.98px) {
  .btn-2 {
    float: right!important;
  }
  #save {
    margin-top: 30px!important;
  }

 }
</style>

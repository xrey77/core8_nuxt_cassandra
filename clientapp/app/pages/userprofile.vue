<template>
<div class="container">

    <div class="card mt-3 mb-4">

        <div class="card-header">
            USER PROFILE NO.&nbsp; {{userId}}
        </div>

        <div class="card-body">
        
            <form @submit.prevent="submitProfileForm" enctype="multipart/form-data">
            <div class="row">
                <div class="col">
                       <div class="mb-3">
                            <input type="text" v-model="user.firstname" required class="form-control" placeholder="enter First Name">
                       </div>
                       <div class="mb-3">
                            <input type="text" v-model="user.lastname" required class="form-control" placeholder="enter Last Name">
                       </div>
                       <div class="mb-3">
                            <input type="email" v-model="user.email" class="form-control" :disabled="true">
                       </div>
                       <div class="mb-3">
                            <input type="text" v-model="user.mobile" required class="form-control" placeholder="enter Mobile No.">
                       </div>                    
                </div>
                <div class="col">

                    <div class="mb-3 text-left">
                        <img id="pix" class="userpic" :src="user.profilepic" alt="profilepic" />
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
                            <input type="password" class="form-control" v-model="user.password" placeholder="enter new Password" autocomplete="off">
                          </div>
                          <div class="mb-3">
                            <input type="password" class="form-control" v-model="user.confpassword" placeholder="confirm new Password" autocomplete="off">
                          </div>
                    </div>
                    <div id="mfa1">
                        <div v-if="user.qrcodeurl">
                            <img class="qrcode1" :src="user.qrcodeurl" alt="qrcodeurl"/>
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
            <button id="save" type="submit" class="btn btn-primary">save</button>
            </form>
        </div>
        <div class="card-footer text-danger fsize-12">
            {{user.profileMsg}}
        </div>
    </div>

</div>
</template>

<script setup lang="ts">
    import nuxtStorage from 'nuxt-storage';
    import $ from 'jquery';
    import {reactive, onMounted} from 'vue'

    const userId = nuxtStorage.localStorage.getData('USERID');
    const username = nuxtStorage.localStorage.getData('USERNAME');
    const userpic = nuxtStorage.localStorage.getData('USERPIC');
    const token = nuxtStorage.localStorage.getData('TOKEN');

    const user = reactive({
        firstname: '',
        lastname: '',
        email: '',
        mobile: '',
        password: '',
        confpassword: '',
        profileMsg: 'loading, please wait..',
        qrcodeurl: '',
        profilepic: ''
    });

    function checkIfImageExists(url: any, callback: any) {
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

    async function fetchData(idno: any) {
        const data = await $fetch(`/api/user/getbyid?id=${idno}`,{
            headers: {
                Authorization:`Bearer ${token}`
            }
        }).catch((error: any) => {
            user.profileMsg = error;
            return;
        });
        user.lastname = data.lastname;
            user.firstname = data.firstname;
            user.email = data.email;
            user.mobile = data.mobile;
            user.qrcodeurl = data.qrcodeurl;
            checkIfImageExists(data.profilepic, (exists) => {
                if (exists) {
                    user.profilepic = data.profilepic;
                } else {
                    user.profilepic = '/users/pix.png';
                }
            });

            user.profileMsg = '';
    }

    onMounted(() => {
        $("#cpwd").hide();
        $("#mfa1").hide();
        $("#mfa2").hide();
        fetchData(userId);
    });

    async function submitProfileForm(e: any) {
        e.preventDefault();
        if (user.password !== '' && user.confpassword) {
            if (user.password !== user.confpassword) {
                user.profileMsg = "New Password does not matched.";
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
            }
        }
        else
        {
            user.profileMsg = 'please enter password';
            window.setTimeout(() => {
                user.profileMsg = "";
            },3000);
        }

        user.profileMsg = 'please wait...';
        const userIdno = nuxtStorage.localStorage.getData('USERID');

        const data = await $fetch('/api/user/updatebyid', {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization:`Bearer ${token}`
                },
                method: 'POST',
                body: {
                  'id': userIdno,
                  'firstname': user.firstname,
                  'lastname': user.lastname,
                  'password': user.password
                }

            }).catch( (error: any) => {
                $("#regMsg").text(error.message);
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
                return;
            });
            if (data.statuscode === 200) {
                    user.profileMsg = data.message;
            } else {
                user.profileMsg = data.message;
            }
            window.setTimeout(() => {
                user.profileMsg = "";
            },6000);            
    }

    function changePassword() {
     if ($('#changepwd').is(":checked")) {
        $("#cpwd").show();
        $("#mfa1").hide();
        $("#mfa2").hide();
        $('#twofactor').prop('checked', false);
     } else {
        $("#cpwd").hide();
        user.password = '';
        user.confpassword = '';
     }
    }

    function onetimePassword() {
        if ($('#twofactor').is(":checked")) {
            $("#cpwd").hide();
            $("#mfa1").show();
            $("#mfa2").show();
            $('#changepwd').prop('checked', false);
        } else {
            $("#mfa1").hide();
            $("#mfa2").hide();
        }
    }

    async function enableMFA(e: any) {
        e.preventDefault();
        const data = await $fetch('/api/user/enablemfa', {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization:`Bearer ${token}`
                },
                method: 'POST',
                body: {
                    'id': userId,
                    'isenabled': true,
                    'fullname': user.firstname + ' ' + user.lastname
                }

            }).catch( (error) => {
                $("#regMsg").text(error.message);
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
                return;
            });
            if (data.statuscode === 200) {
                    user.profileMsg = data.message;
            } else {
                user.profileMsg = data.message;
            }
            window.setTimeout(() => {
                user.profileMsg = "";
                window.location.reload();
            },6000);
    }

    async function disableMFA(e: any) {
        e.preventDefault();
        const data = await $fetch('/api/user/enablemfa', {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization:`Bearer ${token}`
                },
                method: 'POST',
                body: {
                    'id': userId,
                    'isenabled': false,
                    'fullname': user.firstname + ' ' + user.lastname
                }
            }).catch( (error: any) => {
                $("#regMsg").text(error.message);
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
                return;
            });

            if (data.statuscode === 200) {
                    user.profileMsg = data.message;
                    window.location.reload();
            } else {
                user.profileMsg = data.message;
            }
            window.setTimeout(() => {
                user.profileMsg = "";
            },3000);            
    }

    async function changeProfilepic(event: any) {
        $("#pix").attr('src',URL.createObjectURL(event.target.files[0]));
        const file = event.target.files[0];
        const formdata = new FormData();
        formdata.append('myImage', file);

        const data = await $fetch(`/api/user/changeprofilepic?id=${userId}`, {
                headers: {
                    Authorization:`Bearer ${token}`
                },
                body: formdata,
                method: 'POST',

            }).catch( (error: any) => {
                $("#regMsg").text(error.message);
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
                return;
            });

            if (data.statuscode === 200) {
                    user.profileMsg = data.message;
                    nuxtStorage.localStorage.setData('USERPIC', data.profilepic, 4, 'h');
                } else {
                    user.profileMsg = data.message;
                }
                window.setTimeout(() => {
                    user.profileMsg = "";
                },3000);
    
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

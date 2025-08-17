<template>
    <div class="container">
      <h2 class="top-10">Search Product</h2>

      <form class="row g-3" @submit.prevent="submitSearchForm" autocomplete="off">
          <div class="col-auto">
            <input type="text" required class="form-control-sm" v-model="vardata.search" name="search" placeholder="enter description">
          </div>
          <div class="col-auto">
            <button type="submit" class="btn btn-primary btn-sm mb-3">search</button>
          </div>
      </form>

      <div class="card-group">

        <div class="card" v-for="product in vardata.items">
          <img :src="product.prod_pic" class="card-img-top product-size" alt="...">
          <div class="card-body">
            <h5 class="card-title">Descriptions</h5>
            <p class="card-text">{{product.descriptions}}</p>
        </div>
        <div class="card-footer">
            <p class="card-text text-danger"><span class="text-dark">PRICE :</span>&nbsp;<strong>{{product.sell_price.toFixed(2) }}</strong></p>
        </div>  

        </div>

      </div>    

      <div class="align-middle text-left text-warning" v-if="vardata.isfound === true">{{ vardata.listMsg }}</div>

    </div>
</template>

<script setup lang="ts">
    import {reactive} from 'vue'

const vardata = reactive({
    search: '',
    items: [],
    isfound: false,
    listMsg: 'searching, please wait..'
});

const searchProducts = async (key: any) => {
    vardata.isfound=true;
    const data = await $fetch(`/api/product/search?search=${key}`)
    .catch((error: any) => {
      vardata.listMsg = error;
      return;
    });
    vardata.items = data.product;
    vardata.isfound=false;
    if (vardata.items) {
      vardata.listMsg = '';
    }
}

const submitSearchForm = (event: any) => {
  event.preventDefault();
  searchProducts(vardata.search);
}

</script>

<style scoped>
.top-10 {
    margin-top: 10px;
}

.product-size {
    width: 300px;
    height: auto;
}
</style>
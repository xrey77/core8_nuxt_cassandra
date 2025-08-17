<template>
<div class="container">
    <h3 class="text-center mt-3">Product Catalogs</h3>

    <div class="card-group">

        <div class="card" v-for="product in vardata.items">
            <img :src="product.prod_pic" class="card-img-top" alt="...">
            <div class="card-body">
                <h5 class="card-title">Descriptions</h5>
                <p class="card-text">{{product.descriptions}}</p>
            </div>
            <div class="card-footer">
                <p class="card-text text-danger"><span class="text-dark">PRICE :</span>&nbsp;<strong>{{product.sell_price.toFixed(2) }}</strong></p>
            </div>  
        </div>
    
    </div>    

    <div class="mt-2 text-center text-warning" v-if="vardata.listMsg">{{ vardata.listMsg }}</div>

    <div v-if="vardata.listMsg === ''">
      <nav aria-label="Page navigation example">
        <ul class="pagination mt-4">
          <li class="page-item"><a @click="lastPage" class="page-link" href="#">Last</a></li>
          <li class="page-item"><a @click="prevPage" class="page-link" href="#">Previous</a></li>
          <li class="page-item"><a @click="nextPage" class="page-link" href="#">Next</a></li>
          <li class="page-item"><a @click="firstPage" class="page-link" href="#">First</a></li>
          <li class="page-item page-link text-danger">Page&nbsp;{{vardata.page}} of&nbsp;{{vardata.totpage}}</li>
        </ul>
      </nav>
    </div>      
</div>
</template>

<script setup lang="ts">
    import {reactive, onMounted} from 'vue'

const vardata = reactive({
    page: 1,
    totpage: 0,
    items: [],
    listMsg: 'loading data, please wait..'
});

const fetchProducts = async (pg: any) => {
    const data = await $fetch(`/api/product/list?page=${pg}`)
    .catch((error: any) => {
        vardata.listMsg = error;
        return;
    });
    vardata.page = data.page;
    vardata.totpage = data.totpages;
    vardata.items = data.products;
    if (vardata.items) {
        vardata.listMsg = '';
    }
}

onMounted(() => {
    fetchProducts(vardata.page);
});

const firstPage = (event: any) => {
    event.preventDefault();    
    vardata.page = 1;
    fetchProducts(vardata.page);
    return;    
}

const nextPage = (event: any) => {
    event.preventDefault();    
    if (vardata.page == vardata.totpage) {
        return;
    }
    vardata.page = vardata.page + 1;
    fetchProducts(vardata.page);
    return;         
}

const prevPage = (event: any) => {
    event.preventDefault();    
    if (vardata.page == 1) {
        return;
    }
    vardata.page = vardata.page - 1;
    fetchProducts(vardata.page);
    return;    
}

const lastPage = (event: any) => {
    event.preventDefault();    
    vardata.page = vardata.totpage;
    fetchProducts(vardata.page);
    return;    
}

</script>
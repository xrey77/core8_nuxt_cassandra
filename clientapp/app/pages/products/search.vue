<template>
    <div class="container-fluid mb-9">
      <h2 class="top-10">Search Product</h2>

      <form class="row g-3" @submit.prevent="submitSearchForm" autocomplete="off">
          <div class="col-auto">
            <input type="text" required class="form-control-sm" v-model="vardata.search" name="search" placeholder="enter description">
          </div>
          <div class="col-auto">
            <button type="submit" class="btn btn-primary btn-sm mb-3">search</button>
          </div>
      </form>

      <div class="container-fluid mb-9">
      <div class="card-group">

        <div class="card" v-for="product in vardata.prods">
            <img :src="product.productPicture" class="card-img-top product-size" alt="...">
            <div class="card-body">
              <h5 class="card-title">Descriptions</h5>
              <p class="card-text">{{product.descriptions}}</p>
            </div>
            <div class="card-footer">
                <p class="card-text text-danger"><span class="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{{formatNumberWithCommaDecimal(product.sellPrice) }}</strong></p>
            </div>  
        </div>

      </div>    
        <!-- <div class="align-middle text-left text-warning" v-if="vardata.isfound === true">{{ vardata.listMsg }}</div> -->
      </div>
        <div v-if="vardata.totpage !== 0">
        <nav aria-label="Page navigation example">
            <ul class="pagination mt-2 mb-4">
                <li class="page-item"><a @click="firstPage($event)" class="page-link" href="#">First</a></li>
                <li class="page-item"><a @click="prevPage($event)" class="page-link" href="#">Previous</a></li>
                <li class="page-item"><a @click="nextPage($event)" class="page-link" href="#">Next</a></li>
                <li class="page-item"><a @click="lastPage($event)" class="page-link" href="#">Last</a></li>              
                <li class="page-item page-link text-danger">Page&nbsp;{{vardata.page}} of&nbsp;{{vardata.totpage}}</li>
            </ul>
          </nav>
        </div>
      <br/><br/></br>
      <!-- <Footer/> -->
    </div>
</template>

<script setup lang="ts">
    // import Footer from '../../layouts/Footer.vue';
    import {reactive} from 'vue'
    import axios from 'axios';

    const api = axios.create({
      baseURL: "https://localhost:7100",
      headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
    })

  const vardata = reactive({
      search: '',
      prods: [],
      isfound: false,
      listMsg: '',
      page: 1,
      totpage: 0,
      totRecs: 0
  });

  const formatNumberWithCommaDecimal = (number: any) => {
    const formatter = new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
    return formatter.format(number);
  };

  const searchProducts = async (pg: any, key: any) => {
      vardata.isfound=true;
      vardata.listMsg = 'searching, please wait..';
        api.get(`/api/searchproducts/${pg}/${key}`)
                .then((res) => {
                    vardata.prods = res.data.products;
                    vardata.totpage = res.data.totpage;
                    vardata.page = res.data.page;
                    vardata.isfound = true;
                    vardata.listMsg = '';
                }, (error: any) => {
                    vardata.isfound = false;
                    if (error.response) {
                      vardata.listMsg = error.response.data.message;
                    } else {
                      vardata.listMsg = error.message;
                    }
                });    
  }

  const submitSearchForm = async (event: any) => {
    event.preventDefault();
    await searchProducts(vardata.page, vardata.search);
  }

  const nextPage = async (event: any) => {
            event.preventDefault();    
            if (vardata.page === vardata.totpage) {
                return;
            }
            vardata.page = vardata.page + 1;
            await searchProducts(vardata.page, vardata.search);
            return;
  }

  const prevPage = async(event: any) => {
            event.preventDefault();    
            if (vardata.page === 1) {
            return;
            }
            vardata.page = vardata.page - 1;
            await searchProducts(vardata.page, vardata.search);
            return;    
  }
  const firstPage = async(event: any) => {
          event.preventDefault();    
          vardata.page = 1;
          await searchProducts(vardata.page, vardata.search);
          return;    
  }

  const lastPage = async (event: any) => {
      event.preventDefault();    
      vardata.page = vardata.totpage;
      await searchProducts(vardata.page, vardata.search);
      return;    
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
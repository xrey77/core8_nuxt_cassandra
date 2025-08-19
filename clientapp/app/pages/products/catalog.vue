<template>
<div class="container">
    <h3 class="text-center mt-3">Product Catalogs</h3>

    <div class="card-group">
        <div class="card" v-for="product in vardata.prods">
            <img v-bind:src="product.productPicture" class="card-img-top" alt="...">
            <div class="card-body">
                <h5 class="card-title">Descriptions</h5>
                <p class="card-text">{{product.descriptions}}</p>
            </div>
            <div class="card-footer">
                <p class="card-text text-danger"><span class="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{{formatNumberWithCommaDecimal(product.sellPrice) }}</strong></p>
            </div>  
        </div>
    
    </div>    

    <div class="mt-2 text-center text-warning" v-if="vardata.listMsg">{{ vardata.listMsg }}</div>

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
</template>


<script setup lang="ts">
    import $ from 'jquery';
    import {reactive, onMounted, ref} from 'vue'
    import axios from 'axios';

    const api = axios.create({
        baseURL: "https://localhost:7100",
        headers: {'Accept': 'application/json',
                'Content-Type': 'application/json'}
    })

const formatNumberWithCommaDecimal = (number: any) => {
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2, // Ensures at least two decimal places
    maximumFractionDigits: 2, // Limits to two decimal places
  });
  // Format the number
  return formatter.format(number);
};

export interface Proddata{
    page: number,
    totpage: number
    products: Products
}

export interface Products {
    id: string,
    category: string,
    descriptions: string,
    qty: number,
    unit: string,
    sellprice: number
}

    const vardata = reactive({
        page: 1,
        totpage: 0,
        prods: [],
        listMsg: ''
    });

    const fetchProducts = async (pg: any) => {
        api.get<Proddata>(`/api/listproducts/${pg}`)
            .then((res: any) => {        
                const data: Proddata = res.data;     
                vardata.prods = data.products;
                vardata.totpage = data.totpage;
                vardata.page = data.page;
            }, (error: any) => {
                vardata.listMsg = error.response.data.message;
                window.setTimeout(() => {
                    vardata.listMsg = '';
                }, 3000);
                
            });
    }

    onMounted(() => {
        vardata.listMsg = 'loading data, please wait..';
        fetchProducts(vardata.page);
        vardata.listMsg = '';
    });

    const firstPage = (event: any) => {
        event.preventDefault();
            vardata.page = 1;
            fetchProducts(vardata.page);
            return;
    }

    const nextPage = (event: any) => {        
        event.preventDefault();
        if (vardata.page === vardata.totpage) {
            return;
        } 
            vardata.page += 1;
            fetchProducts(vardata.page);
    }

    const prevPage = (event: any) => {
        event.preventDefault();
            vardata.page -= 1;
            ln = (vardata.page * 5) - 5;
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
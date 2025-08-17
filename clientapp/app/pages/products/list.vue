<template>
  <div class="container">
    <h3 class="text-center mt-2 mb-2">Product List</h3>

    <table class="table table-striped">
    <thead class="table-primary">
        <tr>
        <th scope="col">#</th>
        <th scope="col">Descriptions</th>
        <th scope="col">Stocks</th>
        <th scope="col">Unit</th>
        <th scope="col">Price</th>
        </tr>
    </thead>
    <tbody v-for="item in vardata.items" key="item.id">
        <tr>
        <td>{{item.id.substring(20,24)}}</td>
        <td>{{item.descriptions}}</td>
        <td>{{item.qty}}</td>
        <td>{{item.unit}}</td>
        <td>{{item.sell_price.toFixed(2)}}</td>
        </tr>
    </tbody>
    </table>

    <div class="align-middle text-center text-warning" v-if="vardata.listMsg">{{ vardata.listMsg }}</div>

    <div v-if="vardata.listMsg === ''">
     <nav aria-label="Page navigation example">
        <ul class="pagination">
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
        ctr: 0,
        items: [],
        listMsg: 'loading data, please wait..'
    });

    const fetchProducts = async (pg: any) => {
        const data = await $fetch(`/api/product/list?page=${pg}`)
        .catch((error: any) => {
            console.log("may error : " + error);
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

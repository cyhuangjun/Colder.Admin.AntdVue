﻿<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="商户">
              <a-select v-model="queryParam.TenantId" allowClear>
                <a-select-option v-for="item in TenantList" :key="item.value">{{ item.text }}</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
            <a-form-item label="币种">
              <a-select v-model="queryParam.CoinID" allowClear>
                <a-select-option v-for="item in CurrencyList" :key="item.value">{{ item.text }}</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :md="6" :sm="24">
            <a-button type="primary" @click="() => {this.pagination.current = 1; this.getDataList()}">查询</a-button>
            <a-button style="margin-left: 8px" @click="() => (queryParam = {})">重置</a-button>
          </a-col>
        </a-row>
      </a-form>
    </div>
    <a-table
      ref="table"
      :columns="columns"
      :rowKey="row => row.Id"
      :dataSource="data"
      :pagination="pagination"
      :loading="loading"
      @change="handleTableChange"
      :bordered="true"
      size="small"
    >
    </a-table>
  </a-card>
</template>

<script>
const columns = [
  { title: '商户', dataIndex: 'Tenant', width: '8%', align: 'center' },
  { title: '币种', dataIndex: 'Currency', width: '6%', align: 'center' },
  { title: '时间', dataIndex: 'CreateTime', width: '8%', align: 'center' },
  { title: '类型', dataIndex: 'AssetsWasteBookTypeStr', width: '6%', align: 'center' },
  { title: '原可用余额', dataIndex: 'OriginalBalance', width: '8%', align: 'center' },
  { title: '变动数', dataIndex: 'ChangeAmount', width: '8%', align: 'center' },
  { title: '最新余额', dataIndex: 'Balance', width: '8%', align: 'center' },
  { title: '原始冻结数', dataIndex: 'OriginalFrozenAmount', width: '8%', align: 'center' },
  { title: '变动数', dataIndex: 'ChangeFrozenAmount', width: '8%', align: 'center' },
  { title: '最新冻结数', dataIndex: 'FrozenAmount', width: '8%', align: 'center' },
  { title: '关联单号', dataIndex: 'RelateBusinessID', width: '8%', align: 'center' },
  { title: '备注', dataIndex: 'Remarks' }
]

export default {
  mounted () {
    this.init()
    this.getDataList()
  },
  data () {
    return {
      data: [],
      TenantList: [],
      CurrencyList: [],
      pagination: {
        current: 1,
        pageSize: 25,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'CreateTime', order: 'desc' },
      loading: false,
      columns,
      queryParam: {}
    }
  },
  methods: {
    init () {
      this.$http.post('/Foundation/Coin/GetCurrencyList').then(resJson => {
        this.CurrencyList = resJson.Data
      })
      this.$http.post('/Base_Manage/Base_Department/GetTenantList').then(resJson => {
        this.TenantList = resJson.Data
      })
    },
    handleTableChange (pagination, filters, sorter) {
      this.pagination = { ...pagination }
      this.filters = { ...filters }
      this.sorter = { ...sorter }
      this.getDataList()
    },
    getDataList () {
      this.loading = true
      this.$http
        .post('/Transaction/AssetsWasteBook/GetDataList', {
          PageIndex: this.pagination.current,
          PageRows: this.pagination.pageSize,
          SortField: this.sorter.field || 'Id',
          SortType: this.sorter.order,
          Search: this.queryParam,
          ...this.filters
        })
        .then(resJson => {
          this.loading = false
          this.data = resJson.Data
          const pagination = { ...this.pagination }
          pagination.total = resJson.Total
          this.pagination = pagination
        })
    }
  }
}
</script>

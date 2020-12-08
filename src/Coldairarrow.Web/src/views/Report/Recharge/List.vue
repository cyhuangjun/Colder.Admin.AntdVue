<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="20">
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
          <a-col :md="4" :sm="24">
            <a-form-item label="状态">
              <a-select v-model="queryParam.Status" allowClear>
                <a-select-option v-for="item in TransactionStatusList" :key="item.value">{{ item.text }}</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="20">
          <a-col :md="8" :sm="20">
            <a-form-item label="时间">
              <a-date-picker v-model="queryParam.startTime" showTime format="YYYY-MM-DD HH:mm:ss" />~
              <a-date-picker v-model="queryParam.endTime" showTime format="YYYY-MM-DD HH:mm:ss" />
            </a-form-item>
          </a-col>
          <a-col :md="6" :sm="24">
            <a-form-item label="关键字">
              <a-input v-model="queryParam.searchKey" placeholder />
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24" align="right">
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
      <span slot="action" slot-scope="text, record">
        <template>
          <a @click="showDetail(record.Id)">详细</a>
        </template>
      </span>
    </a-table>
    <detail-form ref="detailForm" :parentObj="this"></detail-form>
  </a-card>
</template>

<script>
import DetailForm from './Detail'
const columns = [
  { title: '商户', dataIndex: 'Tenant', width: '8%', align: 'center' },
  { title: '币种', dataIndex: 'Currency', width: '10%', align: 'center', sorter: true },
  { title: 'CUID', dataIndex: 'CUID', width: '10%', align: 'center' },
  { title: '订单号', dataIndex: 'Id', width: '10%', align: 'center' },
  { title: '到账金额', dataIndex: 'ArrivalAmount', width: '10%', align: 'center' },
  { title: '手续费', dataIndex: 'CoinInHandlingFee', width: '10%', align: 'center' },
  { title: '矿工费', dataIndex: 'Minefee', width: '10%', align: 'center' },
  { title: '状态', dataIndex: 'StatusStr', width: '5%', align: 'center', sorter: true },
  { title: '回调状态', dataIndex: 'CallBackStatusStr', width: '5%', align: 'center' },
  { title: '记录时间', dataIndex: 'CreateTime', width: '10%', align: 'center', sorter: true }
]

export default {
  components: {
    DetailForm
  },
  mounted () {
    // if (!this.$auth('admin')) {
    //   this.columns = this.columns.filter(item => item.dataIndex !== 'serveAccountName')
    // }
    this.init()
    this.getDataList()
  },
  data () {
    return {
      data: [],
      pagination: {
        current: 1,
        pageSize: 25,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'CreateTime', order: 'desc' },
      loading: false,
      columns,
      queryParam: {},
      CurrencyList: [],
      TenantList: [],
      TransactionStatusList: []
    }
  },
  methods: {
    handleTableChange (pagination, filters, sorter) {
      this.pagination = { ...pagination }
      this.filters = { ...filters }
      this.sorter = { ...sorter }
      this.getDataList()
    },
    init () {
      this.$http.post('/Foundation/Coin/GetCurrencyList').then(resJson => {
        this.CurrencyList = resJson.Data
      })
      this.$http.post('/Transaction/Transfers/GetTransactionStatusList').then(resJson => {
        this.TransactionStatusList = resJson.Data
      })
      this.$http.post('/Base_Manage/Base_Department/GetTenantList').then(resJson => {
        this.TenantList = resJson.Data
      })
    },
    getDataList () {
      this.loading = true
      this.$http
        .post('/Transaction/CoinTransactionIn/GetDataReportList', {
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
    },
    showDetail (id) {
      this.$refs.detailForm.openForm(id)
    }
  }
}
</script>

<template>
  <a-card :bordered="false">
    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="币种">
              <a-select v-model="queryParam.CoinID" allowClear>
                <a-select-option v-for="item in CurrencyList" :key="item.value">{{ item.text }}</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
            <a-button type="primary" @click="() => {this.pagination.current = 1; this.getDataList()}">查询</a-button>
            <a-button style="margin-left: 8px" @click="() => (queryParam = {})">重置</a-button>
            <a-button type="primary" style="margin-left: 8px" @click="() => {this.createSystemAddress()}">生成系统钱包</a-button>
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
  { title: '币种', dataIndex: 'Currency', width: '12%', align: 'center' },
  { title: '数量', dataIndex: 'Amount', width: '8%', align: 'center' },
  { title: '地址', dataIndex: 'Amount', width: '50%', align: 'center' },
  { title: '启用', dataIndex: 'IsEnabledStr', width: '8%', align: 'center' },
  { title: '删除', dataIndex: 'IsDeletedStr', width: '8%', align: 'center' }
]

export default {
  components: {
  },
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
        pageSize: 30,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'Enabled', order: 'desc' },
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
    },
    handleTableChange (pagination, filters, sorter) {
      this.pagination = { ...pagination }
      this.filters = { ...filters }
      this.sorter = { ...sorter }
      this.getDataList()
    },
    createSystemAddress () {
      const id = this.queryParam.CoinID
      if (typeof id === 'undefined' || id == null || id === '') {
        this.$message.warning('请选择币种.')
        return
      }
      this.loading = true
      this.$http
        .post('/Transaction/Assets/createSystemAddress', { id: id }).then(resJson => {
          this.loading = false
          if (resJson.Success) {
            this.$message.success('操作成功!')
            this.getDataList()
          } else {
            this.$message.error(resJson.Msg)
          }
        })
    },
    getDataList () {
      this.loading = true
      this.$http
        .post('/Transaction/Assets/GetWalletSystemAddressDataList', {
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

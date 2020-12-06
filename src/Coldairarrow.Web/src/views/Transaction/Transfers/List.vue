<template>
  <a-card :bordered="false">
  <!--
    <div class="table-operator">
      <a-button type="primary" icon="plus" @click="hanldleAdd()">新建</a-button>
      <a-button
        type="primary"
        icon="minus"
        @click="handleDelete(selectedRowKeys)"
        :disabled="!hasSelected()"
        :loading="loading"
      >删除</a-button>
      <a-button type="primary" icon="redo" @click="getDataList()">刷新</a-button>
  </div>-->

<div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="20">
          <a-col :md="5" :sm="24">
            <a-form-item label="关键字">
              <a-input v-model="queryParam.searchKey" placeholder />
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
            <a-form-item label="币种">
              <a-select v-model="queryParam.currency" allowClear>
                <a-select-option v-for="item in CurrencyList" :key="item.text">{{ item.text }}</a-select-option>
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
          <a-col :md="10" :sm="24">
            <a-form-item label="时间">
              <a-date-picker v-model="queryParam.startTime" showTime format="YYYY-MM-DD HH:mm:ss" />~
              <a-date-picker v-model="queryParam.endTime" showTime format="YYYY-MM-DD HH:mm:ss" />
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
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
      :rowSelection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }"
      :bordered="true"
      size="small"
    >
     <span slot="action" slot-scope="text, record">
        <template>
          <a @click="showDetail(record.Id)">详细</a>
          <a-divider type="vertical" v-if="record.Status==0" />
          <a v-if="record.Status==0" @click="handlePass(record.Id)">通过</a>
          <a-divider type="vertical" v-if="record.Status==0" />
          <a v-if="record.Status==0" @click="handleDeny(record.Id)">拒绝</a>
        </template>
      </span>
    </a-table>

    <detail-form ref="detailForm" :parentObj="this"></detail-form>
  </a-card>
</template>

<script>
import DetailForm from './Detail'

const columns = [
  { title: '订单号', dataIndex: 'Id', width: '10%', align: 'center' },
  { title: '商户单号', dataIndex: 'OrderId', width: '10%', align: 'center' },
  { title: '币种', dataIndex: 'Currency', width: '10%', align: 'center', sorter: true },
  { title: '数量', dataIndex: 'Amount', width: '10%', align: 'center' },
  { title: '手续费', dataIndex: 'HandlingFee', width: '10%', align: 'center' },
  { title: '状态', dataIndex: 'StatusStr', width: '5%', align: 'center', sorter: true },
  { title: '创建时间', dataIndex: 'CreatedAt', width: '10%', align: 'center', sorter: true },
  { title: '审核时间', dataIndex: 'ApproveTime', width: '10%', align: 'center', sorter: true },
  { title: '操作', dataIndex: 'action', align: 'center', scopedSlots: { customRender: 'action' } }
]

export default {
  components: {
    DetailForm
  },
  mounted () {
    this.init()
    this.getDataList()
  },
  data () {
    return {
      data: [],
      pagination: {
        current: 1,
        pageSize: 10,
        showTotal: (total, range) => `总数:${total} 当前:${range[0]}-${range[1]}`
      },
      filters: {},
      sorter: { field: 'Id', order: 'asc' },
      loading: false,
      columns,
      queryParam: {},
      CurrencyList: [],
      TransactionStatusList: [],
      selectedRowKeys: []
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
    },
    getDataList () {
      this.selectedRowKeys = []

      this.loading = true
      this.$http
        .post('/Transaction/Transfers/GetDataList', {
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
    onSelectChange (selectedRowKeys) {
      this.selectedRowKeys = selectedRowKeys
    },
    hasSelected () {
      return this.selectedRowKeys.length > 0
    },
    showDetail (id) {
      this.$refs.detailForm.openForm(id)
    },
    handlePass (id) {
      var thisObj = this
      this.$confirm({
        title: '确认通过吗?',
        onOk () {
          return new Promise((resolve, reject) => {
            thisObj.$http.post('/Transaction/Transfers/PassData', { id: id }).then(resJson => {
              resolve()

              if (resJson.Success) {
                thisObj.$message.success('操作成功!')

                thisObj.getDataList()
              } else {
                thisObj.$message.error(resJson.Msg)
              }
            })
          })
        }
      })
    },
    handleDeny (id) {
      var thisObj = this
      this.$confirm({
        title: '确认拒绝吗?',
        onOk () {
          return new Promise((resolve, reject) => {
            thisObj.$http.post('/Transaction/Transfers/DenyData', { id: id }).then(resJson => {
              resolve()

              if (resJson.Success) {
                thisObj.$message.success('操作成功!')

                thisObj.getDataList()
              } else {
                thisObj.$message.error(resJson.Msg)
              }
            })
          })
        }
      })
    }
  }
}
</script>

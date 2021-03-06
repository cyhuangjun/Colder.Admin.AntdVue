﻿<template>
  <a-card :bordered="false">
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
    </div>

    <div class="table-page-search-wrapper">
      <a-form layout="inline">
        <a-row :gutter="10">
          <a-col :md="4" :sm="24">
            <a-form-item label="查询类别">
              <a-select allowClear v-model="queryParam.condition">
                <a-select-option key="ID">ID</a-select-option>
                <a-select-option key="Code">Code</a-select-option>
                <a-select-option key="Name">Name</a-select-option>
                <a-select-option key="ImageUrl">ImageUrl</a-select-option>
                <a-select-option key="ApiUrl">ApiUrl</a-select-option>
                <a-select-option key="ApiSecurityKey">ApiSecurityKey</a-select-option>
                <a-select-option key="TokenCoinID">TokenCoinID</a-select-option>
                <a-select-option key="TokenCoinAddress">TokenCoinAddress</a-select-option>
                <a-select-option key="CreatorID">CreatorID</a-select-option>
                <a-select-option key="LastUpdateUserID">LastUpdateUserID</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :md="4" :sm="24">
            <a-form-item>
              <a-input v-model="queryParam.keyword" placeholder="关键字" />
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
      :rowSelection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }"
      :bordered="true"
      size="small"
    >
      <span slot="action" slot-scope="text, record">
        <template>
          <a @click="handleEdit(record.Id)">编辑</a>
          <a-divider type="vertical" />
          <a @click="handleDelete([record.Id])">删除</a>
        </template>
      </span>
    </a-table>

    <edit-form ref="editForm" :parentObj="this"></edit-form>
  </a-card>
</template>

<script>
import EditForm from './EditForm'

const columns = [
  { title: 'ID', dataIndex: 'ID', width: '10%' },
  { title: 'Code', dataIndex: 'Code', width: '10%' },
  { title: 'Name', dataIndex: 'Name', width: '10%' },
  { title: 'ProviderType', dataIndex: 'ProviderType', width: '10%' },
  { title: 'ImageUrl', dataIndex: 'ImageUrl', width: '10%' },
  { title: 'IsAvailable', dataIndex: 'IsAvailable', width: '10%' },
  { title: 'IsSupportWallet', dataIndex: 'IsSupportWallet', width: '10%' },
  { title: 'IsUseSysAccount', dataIndex: 'IsUseSysAccount', width: '10%' },
  { title: 'ApiUrl', dataIndex: 'ApiUrl', width: '10%' },
  { title: 'ApiSecurityKey', dataIndex: 'ApiSecurityKey', width: '10%' },
  { title: '单位精度，支持几位小数点', dataIndex: 'UnitPrecision', width: '10%' },
  { title: 'MinConfirms', dataIndex: 'MinConfirms', width: '10%' },
  { title: '最小可提币确认数', dataIndex: 'MinTransactionOutConfirms', width: '10%' },
  { title: 'TokenCoinID', dataIndex: 'TokenCoinID', width: '10%' },
  { title: 'TokenCoinAddress', dataIndex: 'TokenCoinAddress', width: '10%' },
  { title: 'StartSyncBlockHeight', dataIndex: 'StartSyncBlockHeight', width: '10%' },
  { title: 'TokenCoinPrecision', dataIndex: 'TokenCoinPrecision', width: '10%' },
  { title: 'SortNumber', dataIndex: 'SortNumber', width: '10%' },
  { title: 'CreatorID', dataIndex: 'CreatorID', width: '10%' },
  { title: 'LastUpdateTime', dataIndex: 'LastUpdateTime', width: '10%' },
  { title: 'LastUpdateUserID', dataIndex: 'LastUpdateUserID', width: '10%' },
  { title: '操作', dataIndex: 'action', scopedSlots: { customRender: 'action' } }
]

export default {
  components: {
    EditForm
  },
  mounted () {
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
    getDataList () {
      this.selectedRowKeys = []

      this.loading = true
      this.$http
        .post('/Foundation/Coin/GetDataList', {
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
    hanldleAdd () {
      this.$refs.editForm.openForm()
    },
    handleEdit (id) {
      this.$refs.editForm.openForm(id)
    },
    handleDelete (ids) {
      var thisObj = this
      this.$confirm({
        title: '确认删除吗?',
        onOk () {
          return new Promise((resolve, reject) => {
            thisObj.$http.post('/Foundation/Coin/DeleteData', ids).then(resJson => {
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

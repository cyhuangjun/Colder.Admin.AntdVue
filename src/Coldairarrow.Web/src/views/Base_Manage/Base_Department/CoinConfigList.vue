<template>
  <a-spin :spinning="loading">
    <div class="table-operator">
      <a-button class="editable-add-btn" icon="plus" type="default" @click="handleAdd">添加充提设置</a-button>
    </div>
    <a-table :columns="columns" :dataSource="data" bordered size="small" :pagination="false">
      <template slot="Currency" slot-scope="text, record">
        <div>
          <a-select v-if="record.editable" style="margin: -5px 0" v-model="record.CoinID" @change="e => handleChange(e, record.Id, 'Currency', CurrencyList)">
            <a-select-option v-for="item in CurrencyList" :key="item.value">{{ item.text }}</a-select-option>
          </a-select>
          <template v-else>{{ text }}</template>
        </div>
      </template>
      <template slot="CoinConfigName" slot-scope="text, record">
        <div>
          <a-select v-if="record.editable" style="margin: -5px 0" v-model="record.CoinConfigID" @change="e => handleChange(e, record.Id, 'CoinConfigName', CoinConfigList)">
            <a-select-option v-for="item in CoinConfigList" :key="item.value">{{ item.text }}</a-select-option>
          </a-select>
          <template v-else>{{ text }}</template>
        </div>
      </template>
      <template slot="operation" slot-scope="text, record">
        <div class="editable-row-operations">
          <span v-if="record.editable">
            <a-popconfirm title="确认保存吗?" @confirm="() => save(record.Id)">
              <a>保存</a>
            </a-popconfirm>
            <a-divider type="vertical"/>
            <a @click="() => cancel(record.Id)">取消</a>
          </span>
          <span v-else>
            <a @click="() => edit(record.Id)">编辑</a>
            <a-divider type="vertical"/>
            <a-popconfirm v-if="data.length" title="确认删除吗?" @confirm="() => onDelete(record.Id)">
              <a href="javascript:;">删除</a>
            </a-popconfirm>
          </span>
        </div>
      </template>
    </a-table>
  </a-spin>
</template>
<script>
var uuid = require('node-uuid')

const columns = [
  { title: '币种', dataIndex: 'Currency', width: '30%', scopedSlots: { customRender: 'Currency' } },
  { title: '策略', dataIndex: 'CoinConfigName', width: '50%', scopedSlots: { customRender: 'CoinConfigName' } },
  { title: '操作', dataIndex: 'operation', scopedSlots: { customRender: 'operation' } }
]
export default {
  data() {
    return {
      data: [],
      CurrencyList: [],
      CoinConfigList: [],
      columns,
      loading: false,
      parentId: null
    }
  },
  methods: {
    handleChange(value, key, column, list) {
      var obj = JSON.parse(JSON.stringify(list.filter(item => value === item.value)))
      value = obj[0].text      
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      if (target) {
        target[column] = value
        this.data = newData
      }
    },
    edit(key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      if (target) {
        target.editable = true
        this.data = newData
      }
    },
    save(key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      if (target) {
        delete target.editable
        this.data = newData
        this.resetCache(newData)
      }
    },
    cancel(key) {
      const newData = [...this.data]
      const target = newData.filter(item => key === item.Id)[0]
      if (target) {
        Object.assign(target, this.cacheData.filter(item => key === item.Id)[0])
        delete target.editable
        this.data = newData
      }
    },
    onDelete(key) {
      const data = [...this.data]
      this.data = data.filter(item => item.Id !== key)
    },
    handleAdd() {
      const newData = {
        Id: uuid.v4(), 
        TenantId: this.parentId
      }
      this.data = [...this.data, newData]
    },
    getCoinConfigList() {
      return this.data
    },
    resetCache(dataSource) {
        this.cacheData = dataSource.map(item => ({ ...item }))
    },
    getDataList() {
      this.loading = true
      this.$http
        .post('/Base_Manage/Base_Department/GetCoinConfigList', {
          id: this.parentId
        })
        .then(resJson => {
          this.loading = false
          resJson.Data.forEach(x => (x['Id'] = uuid.v4()))
          this.data = resJson.Data
          this.resetCache(this.data)
        })
    },
    init(parentId) {
      this.parentId = parentId
      this.data = []
      this.$http.post('/Foundation/Coin/GetCurrencyList').then(resJson => {
        this.CurrencyList = resJson.Data
      })
      this.$http.post('/Foundation/CoinConfig/GetSelectOption').then(resJson => {
        this.CoinConfigList = resJson.Data
      })      
      if (this.parentId) {
        this.getDataList()
      }
    }
  }
}
</script>
<style scoped>
.editable-row-operations a {
  margin-right: 8px;
}
.editable-add-btn {
  margin-bottom: 8px;
}
</style>

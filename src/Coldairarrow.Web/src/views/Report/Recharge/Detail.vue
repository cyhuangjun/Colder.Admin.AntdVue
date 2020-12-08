<template>
  <a-modal
    :title="title"
    width="40%"
    footer=""
    :visible="visible"
    :confirmLoading="loading"
    @cancel="()=>{this.visible=false}"
  >
    <a-spin :spinning="loading">
      <a-form ref="form" :label-col="{ span: 5 }" :wrapper-col="{ span: 12 }" >
        <a-form-item label="单号">
          {{ entity.Id }}
        </a-form-item>
        <a-form-item label="商户">
          {{ entity.Tenant }}
        </a-form-item>
        <a-form-item label="UID">
          {{ entity.CUID }}
        </a-form-item>
        <a-form-item label="币种">
          {{ entity.Currency }}
        </a-form-item>
        <a-form-item label="数量">
          {{ entity.Amount }}
        </a-form-item>
        <a-form-item label="状态">
          {{ entity.StatusStr }}
        </a-form-item>
        <a-form-item label="手续费">
          {{ entity.CoinInHandlingFee }}
        </a-form-item>
        <a-form-item label="地址">
          {{ entity.Address }}
        </a-form-item>
        <a-form-item label="TXID">
          {{ entity.TXID }}
        </a-form-item>
        <a-form-item label="确认数">
          {{ entity.Confirmations }}
        </a-form-item>
        <a-form-item label="From Address">
          {{ entity.FromAddress }}
        </a-form-item>
        <a-form-item label="转移地址">
          {{ entity.MoveToAddress }}
        </a-form-item>
        <a-form-item label="交易移动状态">
          {{ entity.MoveStatusStr }}
        </a-form-item>
        <a-form-item label="矿工费币种">
          {{ entity.MinefeeCurrency }}
        </a-form-item>
        <a-form-item label="矿工费(总)">
          {{ entity.Minefee }}
        </a-form-item>
        <a-form-item label="矿工费(用户)">
          {{ entity.MoveUserMinefee }}
        </a-form-item>
        <a-form-item label="矿工费(系统)">
          {{ entity.MoveSysMinefee }}
        </a-form-item>
        <a-form-item label="预留矿工费">
          {{ entity.ReserveMinerfees }}
        </a-form-item>
        <a-form-item label="实际到账">
          {{ entity.ArrivalAmount }}
        </a-form-item>
        <a-form-item label="回调状态">
          {{ entity.CallBackStatusStr }}
        </a-form-item>
        <a-form-item label="创建时间">
          {{ entity.CreatedAt }}
        </a-form-item>
      </a-form>
    </a-spin>
  </a-modal>
</template>

<script>
export default {
  data () {
    return {
      layout: {
        labelCol: { span: 5 },
        wrapperCol: { span: 18 }
      },
      visible: false,
      loading: false,
      entity: {},
      rules: {},
      title: '详细'
    }
  },
  methods: {
    init () {
      this.visible = true
      this.entity = {}
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
      })
    },
    openForm (id, title) {
      this.init()

      if (id) {
        this.loading = true
        this.$http.post('/Transaction/CoinTransactionIn/GetTheDataReport', { id: id }).then(resJson => {
          this.loading = false

          this.entity = resJson.Data
        })
      }
    }
  }
}
</script>

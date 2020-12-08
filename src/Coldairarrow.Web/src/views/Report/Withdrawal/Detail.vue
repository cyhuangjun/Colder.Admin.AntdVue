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
        <a-form-item label="商户单号">
          {{ entity.ClientOrderId }}
        </a-form-item>
        <a-form-item label="商户备注">
          {{ entity.OrderDescription }}
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
          {{ entity.HandlingFee }}
        </a-form-item>
        <a-form-item label="矿工费">
          {{ entity.Minefee }}
        </a-form-item>
        <a-form-item label="矿工费币种">
          {{ entity.MinefeeCurrency }}
        </a-form-item>
        <a-form-item label="回调状态">
          {{ entity.CallBackStatusStr }}
        </a-form-item>
        <a-form-item label="地址">
          {{ entity.AddressTo }}
        </a-form-item>
        <a-form-item label="地址标签">
          {{ entity.AddressTag }}
        </a-form-item>
        <a-form-item label="TXID">
          {{ entity.TXID }}
        </a-form-item>
        <a-form-item label="创建时间">
          {{ entity.CreatedAt }}
        </a-form-item>
        <a-form-item label="审批时间">
          {{ entity.ApproveTime }}
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
        this.$http.post('/Transaction/Transfers/GetTheDataReport', { id: id }).then(resJson => {
          this.loading = false

          this.entity = resJson.Data
        })
      }
    }
  }
}
</script>

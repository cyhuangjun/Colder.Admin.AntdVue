<template>
  <a-modal
    :title="title"
    width="40%"
    :visible="visible"
    :confirmLoading="loading"
    @ok="handleSubmit"
    @cancel="()=>{this.visible=false}"
  >
    <a-spin :spinning="loading">
      <a-form-model ref="form" :model="entity" :rules="rules" v-bind="layout">
        <a-form-model-item label="ID" prop="ID">
          <a-input v-model="entity.ID" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="Currency" prop="Currency">
          <a-input v-model="entity.Currency" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="说明" prop="Caption">
          <a-input v-model="entity.Caption" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="MinerFee" prop="MinerFee">
          <a-input v-model="entity.MinerFee" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="MinerFeeRate" prop="MinerFeeRate">
          <a-input v-model="entity.MinerFeeRate" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="MinerFeeModeType" prop="MinerFeeModeType">
          <a-input v-model="entity.MinerFeeModeType" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="最小充币" prop="CoinInMinAmount">
          <a-input v-model="entity.CoinInMinAmount" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="最小提币" prop="CoinOutMinAmount">
          <a-input v-model="entity.CoinOutMinAmount" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="最小充币" prop="CoinOutMaxAmount">
          <a-input v-model="entity.CoinOutMaxAmount" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="充币手续费" prop="CoinInHandlingFee">
          <a-input v-model="entity.CoinInHandlingFee" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="充币手续费率" prop="CoinInHandlingFeeRate">
          <a-input v-model="entity.CoinInHandlingFeeRate" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="充币费用类型，1固定，2比例" prop="CoinInHandlingFeeModeType">
          <a-input v-model="entity.CoinInHandlingFeeModeType" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="CoinInHandlingMinFee" prop="CoinInHandlingMinFee">
          <a-input v-model="entity.CoinInHandlingMinFee" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="提币手续费" prop="CoinOutHandlingFee">
          <a-input v-model="entity.CoinOutHandlingFee" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="提币手续费率" prop="CoinOutHandlingFeeRate">
          <a-input v-model="entity.CoinOutHandlingFeeRate" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="CoinOutHandlingFeeModeType" prop="CoinOutHandlingFeeModeType">
          <a-input v-model="entity.CoinOutHandlingFeeModeType" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="CoinOutHandlingMinFee" prop="CoinOutHandlingMinFee">
          <a-input v-model="entity.CoinOutHandlingMinFee" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="是否默认" prop="IsDefault">
          <a-input v-model="entity.IsDefault" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="CreatorID" prop="CreatorID">
          <a-input v-model="entity.CreatorID" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="LastUpdateTime" prop="LastUpdateTime">
          <a-input v-model="entity.LastUpdateTime" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="LastUpdateUserID" prop="LastUpdateUserID">
          <a-input v-model="entity.LastUpdateUserID" autocomplete="off" />
        </a-form-model-item>
      </a-form-model>
    </a-spin>
  </a-modal>
</template>

<script>
export default {
  props: {
    parentObj: Object
  },
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
      title: ''
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
        this.$http.post('/Foundation/CoinConfig/GetTheData', { id: id }).then(resJson => {
          this.loading = false

          this.entity = resJson.Data
        })
      }
    },
    handleSubmit () {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.loading = true
        this.$http.post('/Foundation/CoinConfig/SaveData', this.entity).then(resJson => {
          this.loading = false

          if (resJson.Success) {
            this.$message.success('操作成功!')
            this.visible = false

            this.parentObj.getDataList()
          } else {
            this.$message.error(resJson.Msg)
          }
        })
      })
    }
  }
}
</script>

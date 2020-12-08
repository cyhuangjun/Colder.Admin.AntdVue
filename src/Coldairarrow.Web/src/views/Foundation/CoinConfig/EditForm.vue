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
        <a-form-model-item label="名称" prop="Caption">
          <a-input v-model="entity.Caption" autocomplete="off" style="width: 360px;"/>
        </a-form-model-item>
        <a-form-model-item label="币种" prop="CoinID">
          <a-select v-model="entity.CoinID" style="width: 180px;">
            <a-select-option v-for="item in CurrencyList" :key="item.value">{{ item.text }}</a-select-option>
          </a-select>
        </a-form-model-item>
        <a-form-model-item label="默认" prop="IsDefault">
          <a-checkbox v-model="entity.IsDefault"/>
        </a-form-model-item>
        <a-card title="基本" :bordered="false">
          <a-form-model-item label="最小充币" prop="CoinInMinAmount">
            <a-input-number :min="0" v-model="entity.CoinInMinAmount" />
          </a-form-model-item>
          <a-form-model-item label="最小提币" prop="CoinOutMinAmount">
            <a-input-number :min="0" v-model="entity.CoinOutMinAmount" />
          </a-form-model-item>
          <a-form-model-item label="最大充币" prop="CoinOutMaxAmount">
            <a-input-number :min="0" v-model="entity.CoinOutMaxAmount" />
          </a-form-model-item>
        </a-card>
        <a-card title="矿工费" :bordered="false">
          <a-form-model-item label="费用" prop="MinerFee">
            <a-input-number :min="0" v-model="entity.MinerFee" />
          </a-form-model-item>
          <a-form-model-item label="费率" prop="MinerFeeRate">
            <a-input-number :min="0" :max="1" v-model="entity.MinerFeeRate" />
          </a-form-model-item>
          <a-form-model-item label="类型" prop="MinerFeeModeType">
            <a-radio-group v-model="entity.MinerFeeModeType">
              <a-radio :value="1">
                固定
              </a-radio>
              <a-radio :value="2">
                比例
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-card>
        <a-card title="充币" :bordered="false">
          <a-form-model-item label="费用" prop="CoinInHandlingFee">
            <a-input-number :min="0" v-model="entity.CoinInHandlingFee" />
          </a-form-model-item>
          <a-form-model-item label="费率" prop="CoinInHandlingFeeRate">
            <a-input-number :min="0" :max="1" v-model="entity.CoinInHandlingFeeRate" />
          </a-form-model-item>
          <a-form-model-item label="类型" prop="CoinInHandlingFeeModeType">
            <a-radio-group v-model="entity.CoinInHandlingFeeModeType">
              <a-radio :value="1">
                固定
              </a-radio>
              <a-radio :value="2">
                比例
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="最小费用" prop="CoinInHandlingMinFee">
            <a-input-number :min="0" v-model="entity.CoinInHandlingMinFee" />
          </a-form-model-item>
        </a-card>
        <a-card title="提币" :bordered="false">
          <a-form-model-item label="费用" prop="CoinOutHandlingFee">
            <a-input-number :min="0" v-model="entity.CoinOutHandlingFee" />
          </a-form-model-item>
          <a-form-model-item label="费率" prop="CoinOutHandlingFeeRate">
            <a-input-number :min="0" :max="1" v-model="entity.CoinOutHandlingFeeRate" />
          </a-form-model-item>
          <a-form-model-item label="类型" prop="CoinOutHandlingFeeModeType">
            <a-radio-group v-model="entity.CoinOutHandlingFeeModeType">
              <a-radio :value="1">
                固定
              </a-radio>
              <a-radio :value="2">
                比例
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="最小费用" prop="CoinOutHandlingMinFee">
            <a-input-number :min="0" v-model="entity.CoinOutHandlingMinFee" />
          </a-form-model-item>
        </a-card>
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
      CurrencyList: [],
      visible: false,
      loading: false,
      entity: {},
      rules: {
        Caption: [{ required: true, message: '必填' }],
        CoinID: [{ required: true, message: '必填' }],
        CoinInMinAmount: [{ required: true, message: '必填' }],
        CoinOutMinAmount: [{ required: true, message: '必填' }],
        CoinOutMaxAmount: [{ required: true, message: '必填' }],
        MinerFeeModeType: [
          { type: 'enum', enum: [1, 2], required: true, message: '必选', trigger: 'change' }
        ],
        CoinInHandlingFeeModeType: [
          { type: 'enum', enum: [1, 2], required: true, message: '必选', trigger: 'change' }
        ],
        CoinOutHandlingFeeModeType: [
          { type: 'enum', enum: [1, 2], required: true, message: '必选', trigger: 'change' }
        ]
      },
      title: ''
    }
  },
  methods: {
    init () {
      this.visible = true
      this.entity = {}
      this.$http.post('/Foundation/Coin/GetCurrencyList').then(resJson => {
        this.CurrencyList = resJson.Data
      })
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

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
        <a-form-model-item label="Code" prop="Code">
          <a-input v-model="entity.Code" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="Name" prop="Name">
          <a-input v-model="entity.Name" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ProviderType" prop="ProviderType">
          <a-input v-model="entity.ProviderType" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ImageUrl" prop="ImageUrl">
          <a-input v-model="entity.ImageUrl" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="IsAvailable" prop="IsAvailable">
          <a-input v-model="entity.IsAvailable" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="IsSupportWallet" prop="IsSupportWallet">
          <a-input v-model="entity.IsSupportWallet" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="IsUseSysAccount" prop="IsUseSysAccount">
          <a-input v-model="entity.IsUseSysAccount" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ApiUrl" prop="ApiUrl">
          <a-input v-model="entity.ApiUrl" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ApiSecurityKey" prop="ApiSecurityKey">
          <a-input v-model="entity.ApiSecurityKey" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="单位精度，支持几位小数点" prop="UnitPrecision">
          <a-input v-model="entity.UnitPrecision" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="MinConfirms" prop="MinConfirms">
          <a-input v-model="entity.MinConfirms" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="最小可提币确认数" prop="MinTransactionOutConfirms">
          <a-input v-model="entity.MinTransactionOutConfirms" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="TokenCoinID" prop="TokenCoinID">
          <a-input v-model="entity.TokenCoinID" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="TokenCoinAddress" prop="TokenCoinAddress">
          <a-input v-model="entity.TokenCoinAddress" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="StartSyncBlockHeight" prop="StartSyncBlockHeight">
          <a-input v-model="entity.StartSyncBlockHeight" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="TokenCoinPrecision" prop="TokenCoinPrecision">
          <a-input v-model="entity.TokenCoinPrecision" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="SortNumber" prop="SortNumber">
          <a-input v-model="entity.SortNumber" autocomplete="off" />
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
  data() {
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
    init() {
      this.visible = true
      this.entity = {}
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
      })
    },
    openForm(id, title) {
      this.init()

      if (id) {
        this.loading = true
        this.$http.post('/Foundation/Coin/GetTheData', { id: id }).then(resJson => {
          this.loading = false

          this.entity = resJson.Data
        })
      }
    },
    handleSubmit() {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.loading = true
        this.$http.post('/Foundation/Coin/SaveData', this.entity).then(resJson => {
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

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
        <a-form-model-item label="Symbol" prop="Symbol">
          <a-input v-model="entity.Symbol" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="Sign" prop="Sign">
          <a-input v-model="entity.Sign" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="IsAvailable" prop="IsAvailable">
          <a-input v-model="entity.IsAvailable" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="DecimalDigit" prop="DecimalDigit">
          <a-input v-model="entity.DecimalDigit" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="ImageUrl" prop="ImageUrl">
          <a-input v-model="entity.ImageUrl" autocomplete="off" />
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
        this.$http.post('/Foundation/Currency/GetTheData', { id: id }).then(resJson => {
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
        this.$http.post('/Foundation/Currency/SaveData', this.entity).then(resJson => {
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

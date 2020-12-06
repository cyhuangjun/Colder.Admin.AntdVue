<template>
  <a-modal
    title="编辑表单"
    width="40%"
    :visible="visible"
    :confirmLoading="confirmLoading"
    @ok="handleSubmit"
    @cancel="()=>{this.visible=false}"
  >
    <a-spin :spinning="confirmLoading">
      <a-form-model ref="form" :model="entity" :rules="rules" v-bind="layout">
        <a-form-model-item label="编码" prop="Code">
          <a-input v-model="entity.Code" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="名称" prop="Name">
          <a-input v-model="entity.Name" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="上级" prop="ParentId">
          <a-tree-select
            v-model="entity.ParentId"
            allowClear
            :treeData="ParentIdTreeData"
            placeholder="请选择上级部门"
            treeDefaultExpandAll
          ></a-tree-select>
        </a-form-model-item> 
        <a-form-model-item label="冻结" prop="IsFrozen">
          <a-checkbox v-model="entity.IsFrozen"/>
        </a-form-model-item>
        <a-form-model-item label="APIKEY" prop="ApiKey">
          <a-input v-model="entity.ApiKey" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="密钥" prop="SecretKey">
          <a-input v-model="entity.SecretKey" autocomplete="off" />
        </a-form-model-item> 
        <a-form-model-item label="充币回调地址" prop="PaymentCallbackUrl">
          <a-input v-model="entity.PaymentCallbackUrl" autocomplete="off" />
        </a-form-model-item>
        <a-form-model-item label="提币回调地址" prop="TransfersCallbackUrl">
          <a-input v-model="entity.TransfersCallbackUrl" autocomplete="off" />
        </a-form-model-item>
        <a-card title="充提设置" :bordered="false">
          <CoinConfig-List ref="coinConfigList" :parentObj="this"></CoinConfig-List>
        </a-card>        
      </a-form-model>
    </a-spin>
  </a-modal>
</template>

<script>
import CoinConfigList from './CoinConfigList'
export default {
  props: {
    afterSubmit: {
      type: Function,
      default: null
    }
  },
  components: {
    CoinConfigList
  },
  data() {
    return {
      layout: {
        labelCol: { span: 5 },
        wrapperCol: { span: 18 }
      },
      visible: false,
      confirmLoading: false,
      entity: {},
      ParentIdTreeData: [],
      CoinConfigData: [],
      rules: {
        Name: [{ required: true, message: '必填' }]
      }
    }
  },
  methods: {
    init(id) {
      this.visible = true
      this.entity = {}
      this.$nextTick(() => {
        this.$refs.coinConfigList.init(id)
        this.$refs['form'].clearValidate()
      })

      this.$http.post('/Base_Manage/Base_Department/GetTreeDataList', {}).then(resJson => {
        if (resJson.Success) {
          this.ParentIdTreeData = resJson.Data
        }
      })
      this.$http.post('/Foundation/CoinConfig/GetSelectOption').then(resJson => {
        this.CoinConfigData = resJson.Data
      })
    },
    openForm(id) {
      this.init(id)

      if (id) {
        this.$http.post('/Base_Manage/Base_Department/GetTheData', { id: id }).then(resJson => {
          this.entity = resJson.Data
        })
      }
    },
    handleSubmit() {
      this.$refs['form'].validate(valid => {
        if (!valid) {
          return
        }
        this.confirmLoading = true
        this.entity.coinConfigList = this.$refs.coinConfigList.getCoinConfigList()
        this.$http.post('/Base_Manage/Base_Department/SaveData', this.entity).then(resJson => {
          this.confirmLoading = false

          if (resJson.Success) {
            this.$message.success('操作成功!')
            this.afterSubmit()
            this.visible = false
          } else {
            this.$message.error(resJson.Msg)
          }
        })
      })
    }
  }
}
</script>

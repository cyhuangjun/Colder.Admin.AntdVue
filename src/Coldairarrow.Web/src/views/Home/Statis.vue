<!--需要复杂的请参考:https://preview.pro.loacg.com/dashboard/analysis-->
<template>
  <div class="page-header-index-wide">
    <a-row :gutter="24">
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="总充提笔数" total="4000">
          <a-tooltip title="总充币与提币单数量" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <trend flag="up" term="周同比" :percentage="12" style="margin-right: 16px;" />
            <trend flag="down" term="日同比" :percentage="12" />
          </div>
          <template slot="footer">当日充提笔数<span style="margin-left: 8px;">12423/9999</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="USDT ERC20 余额" :total="8846 | NumberFormat">
          <a-tooltip title="USDT ERC20 账户余额" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-area />
          </div>
          <template slot="footer">当日充币数量<span style="margin-left: 8px;">{{ '1991234' | NumberFormat }}</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="BTC 余额" total="8,000">
          <a-tooltip title="BTC 账户余额" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar />
          </div>
          <template slot="footer">当日充币数量<span style="margin-left: 8px;">3861</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="ETH 余额" total="300,000">
          <a-tooltip title="ETH 账户余额" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar />
          </div>
          <template slot="footer">当日充币数量<span style="margin-left: 8px;">9291</span></template>
        </chart-card>
      </a-col>
    </a-row>
  </div>
</template>
<script>
import ChartCard from '@/components/Charts/ChartCard'
import Trend from '@/components/Charts/Trend'
import MiniArea from '@/components/Charts/MiniArea'
import MiniBar from '@/components/Charts/MiniBar'

export default {
  components: {
    ChartCard,
    Trend,
    MiniArea,
    MiniBar
  },
  data () {
    return {
      loading: false
    }
  }
}
</script>

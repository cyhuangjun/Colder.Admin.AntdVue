<template>
  <div class="page-header-index-wide">
    <a-row :gutter="24">
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="总充币笔数" total="4000">
          <a-tooltip title="总充币单数量" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <trend flag="up" term="周同比" :percentage="12" style="margin-right: 16px;" />
            <trend flag="down" term="日同比" :percentage="12" />
          </div>
          <template slot="footer">当日充币笔数<span style="margin-left: 8px;">12423</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="USDT ERC20 余额" :total="8846 | NumberFormat">
          <a-tooltip title="USDT ERC20 账户余额, 充币每日统计" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-area :data="usdtData" />
          </div>
          <template slot="footer">当日充币数量<span style="margin-left: 8px;">{{ '1991234' | NumberFormat }}</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="BTC 余额" total="8,000">
          <a-tooltip title="BTC 账户余额, 充币每日统计" slot="action">
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
          <a-tooltip title="ETH 账户余额, 充币每日统计" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar />
          </div>
          <template slot="footer">当日充币数量<span style="margin-left: 8px;">9291</span></template>
        </chart-card>
      </a-col>
    </a-row>
    <a-row :gutter="24">
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="总提笔数" total="4000">
          <a-tooltip title="总提币单数量" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <trend flag="up" term="周同比" :percentage="12" style="margin-right: 16px;" />
            <trend flag="down" term="日同比" :percentage="12" />
          </div>
          <template slot="footer">当日提币笔数<span style="margin-left: 8px;">9999</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="USDT ERC20 提币待审总额" :total="8846 | NumberFormat">
          <a-tooltip title="USDT ERC20 提币待审核总额, 提币每日统计" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-area :data="usdtData" />
          </div>
          <template slot="footer">当日提币已审数量<span style="margin-left: 8px;">{{ '1991234' | NumberFormat }}</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="BTC 提币待审总额" total="8,000">
          <a-tooltip title="BTC 提币待审核总额, 提币每日统计" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar />
          </div>
          <template slot="footer">当日提币已审数量<span style="margin-left: 8px;">3861</span></template>
        </chart-card>
      </a-col>
      <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
        <chart-card :loading="loading" title="ETH 提币待审总额" total="300,000">
          <a-tooltip title="ETH 提币待审核总额, 提币每日统计" slot="action">
            <a-icon type="info-circle-o" />
          </a-tooltip>
          <div>
            <mini-bar />
          </div>
          <template slot="footer">当日提币已审数量<span style="margin-left: 8px;">9291</span></template>
        </chart-card>
      </a-col>
    </a-row>
    <a-card :loading="loading" :bordered="false" :body-style="{padding: '0'}">
      <div class="salesCard">
        <a-tabs default-active-key="1" size="large" :tab-bar-style="{marginBottom: '24px', paddingLeft: '16px'}">
          <div class="extra-wrapper" slot="tabBarExtraContent">
            USDT/CNY:100000
          </div>
          <a-tab-pane loading="true" tab="BTC/USDT" key="1">
            <a-row>
              <a-col :xl="24" :lg="12" :md="12" :sm="24" :xs="24">
                <candle :data="btcKlineData" title="BTC/USDT趋势" />
              </a-col>
            </a-row>
          </a-tab-pane>
          <a-tab-pane tab="ETH/USDT" key="2">
            <a-row>
              <a-col :xl="24" :lg="12" :md="12" :sm="24" :xs="24">
                <eth-candle :data="ethKlineData" title="ETH/USDT趋势" />
              </a-col>
            </a-row>
          </a-tab-pane>
        </a-tabs>
      </div>
    </a-card>
  </div>
</template>
<script>
import ChartCard from '@/components/Charts/ChartCard'
import Trend from '@/components/Charts/Trend'
import MiniArea from '@/components/Charts/MiniArea'
import MiniBar from '@/components/Charts/MiniBar'
import Candle from '@/components/Charts/Candle'
import EthCandle from '@/components/Charts/EthCandle'
import moment from 'moment'
const beginDay = new Date().getTime()
const usdtData = []
for (let i = 0; i < 12; i++) {
  usdtData.push({
    x: moment(new Date(beginDay + 1000 * 60 * 60 * 24 * i)).format('YYYY-MM-DD'),
    y: Math.round(Math.random() * 10)
  })
}

const btcKlineData = []
const ethKlineData = []
for (let i = 0; i < 12; i += 1) {
  btcKlineData.push({
    x: `${i + 1}月`,
    y: Math.floor(Math.random() * 1000) + 200
  })
  ethKlineData.push({
    x: `${i + 1}月`,
    y: Math.floor(Math.random() * 1000) + 200
  })
}
export default {
  components: {
    ChartCard,
    Trend,
    MiniArea,
    MiniBar,
    Candle,
    EthCandle
  },
  data () {
    return {
      loading: false,
      usdtData,
      btcKlineData,
      ethKlineData
    }
  }
}
</script>

<style lang="less" scoped>
  .extra-wrapper {
    line-height: 55px;
    padding-right: 24px;
    font-weight: bold;
    .extra-item {
      display: inline-block;
      margin-right: 24px;

      a {
        margin-left: 24px;
      }
    }
  }

  .antd-pro-pages-dashboard-analysis-twoColLayout {
    position: relative;
    display: flex;
    display: block;
    flex-flow: row wrap;
  }

  .antd-pro-pages-dashboard-analysis-salesCard {
    height: calc(100% - 24px);
    /deep/ .ant-card-head {
      position: relative;
    }
  }

  .dashboard-analysis-iconGroup {
    i {
      margin-left: 16px;
      color: rgba(0,0,0,.45);
      cursor: pointer;
      transition: color .32s;
      color: black;
    }
  }
  .analysis-salesTypeRadio {
    position: absolute;
    right: 54px;
    bottom: 12px;
  }
</style>

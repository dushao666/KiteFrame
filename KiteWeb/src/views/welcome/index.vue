<script setup lang="ts">
import { useUserStoreHook } from "@/store/modules/user";
import { getToken } from "@/utils/auth";

defineOptions({
  name: "Welcome"
});

const userStore = useUserStoreHook();
const tokenInfo = getToken();
</script>

<template>
  <div class="welcome-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>欢迎使用 KiteFrame 管理系统</span>
        </div>
      </template>

      <div class="user-info">
        <h2>当前用户信息</h2>
        <el-descriptions :column="2" border>
          <el-descriptions-item label="用户名">
            {{ userStore.username || '未知' }}
          </el-descriptions-item>
          <el-descriptions-item label="昵称">
            {{ userStore.nickname || '未设置' }}
          </el-descriptions-item>
          <el-descriptions-item label="头像">
            <el-avatar :src="userStore.avatar" :size="40">
              {{ userStore.username?.charAt(0)?.toUpperCase() }}
            </el-avatar>
          </el-descriptions-item>
          <el-descriptions-item label="登录状态">
            <el-tag :type="tokenInfo ? 'success' : 'danger'">
              {{ tokenInfo ? '已登录' : '未登录' }}
            </el-tag>
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <div class="token-info" v-if="tokenInfo">
        <h3>Token 信息</h3>
        <el-descriptions :column="1" border>
          <el-descriptions-item label="访问令牌">
            <el-text class="token-text" truncated>
              {{ tokenInfo.accessToken }}
            </el-text>
          </el-descriptions-item>
          <el-descriptions-item label="过期时间">
            {{ new Date(parseInt(tokenInfo.expires)).toLocaleString() }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <div class="actions">
        <el-button type="primary" @click="$router.push('/login')" v-if="!tokenInfo">
          去登录
        </el-button>
        <el-button type="danger" @click="userStore.logOut()" v-if="tokenInfo">
          退出登录
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<style scoped>
.welcome-container {
  padding: 20px;
}

.box-card {
  max-width: 800px;
  margin: 0 auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 18px;
  font-weight: bold;
}

.user-info {
  margin-bottom: 20px;
}

.token-info {
  margin-bottom: 20px;
}

.token-text {
  max-width: 300px;
}

.actions {
  text-align: center;
  margin-top: 20px;
}
</style>

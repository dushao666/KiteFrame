<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>用户管理</span>
        </div>
      </template>
      <div class="table-container">
        <p>用户管理页面</p>
        <p>这里将显示用户列表和管理功能</p>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
defineOptions({
  name: "SystemUser"
});
</script>

<style scoped>
.main {
  margin: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-container {
  padding: 20px 0;
}
</style>

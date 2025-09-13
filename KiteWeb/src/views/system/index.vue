<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>系统管理</span>
        </div>
      </template>
      <div class="content">
        <p>系统管理模块</p>
        <p>包含用户管理、角色管理、菜单管理等功能</p>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
defineOptions({
  name: "System"
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

.content {
  padding: 20px 0;
}
</style>

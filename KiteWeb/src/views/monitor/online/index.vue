<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>在线用户</span>
          <el-button type="primary" @click="handleRefresh">
            <el-icon><Refresh /></el-icon>
            刷新
          </el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="用户名">
            <el-input
              v-model="searchForm.userName"
              placeholder="请输入用户名"
              clearable
              style="width: 200px"
              @keyup.enter="handleSearch"
            />
          </el-form-item>
          <el-form-item label="IP地址">
            <el-input
              v-model="searchForm.ipAddress"
              placeholder="请输入IP地址"
              clearable
              style="width: 200px"
              @keyup.enter="handleSearch"
            />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleSearch">
              <el-icon><Search /></el-icon>
              搜索
            </el-button>
            <el-button @click="handleReset">
              <el-icon><Refresh /></el-icon>
              重置
            </el-button>
          </el-form-item>
        </el-form>
      </div>

              <!-- 表格区域 -->
        <div class="table-container">
          <el-table
            :data="tableData"
            v-loading="loading"
            style="width: 100%"
            border
          >
          <el-table-column type="index" label="序号" width="60" align="center" />
          <el-table-column prop="userName" label="用户名" min-width="120" align="center" />
          <el-table-column prop="realName" label="真实姓名" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.realName || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="deptName" label="部门" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.deptName || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="ipAddress" label="IP地址" min-width="140" align="center" />
          <el-table-column prop="ipLocation" label="登录地点" min-width="150" align="center">
            <template #default="{ row }">
              <span>{{ row.ipLocation || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="browser" label="浏览器" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.browser || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="os" label="操作系统" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.os || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="loginTime" label="登录时间" min-width="180" align="center">
            <template #default="{ row }">
              <span>{{ formatDateTime(row.loginTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="onlineDuration" label="在线时长" min-width="100" align="center">
            <template #default="{ row }">
              <el-tag type="success">{{ formatDuration(row.onlineDuration) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="statusText" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.statusText }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="120" fixed="right" align="center">
            <template #default="{ row }">
              <el-button
                type="danger"
                size="small"
                @click="handleForceLogout(row)"
                v-permission="'monitor:online:logout'"
                :disabled="row.status === 0"
              >
                <el-icon><Close /></el-icon>
                强制下线
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- 分页 -->
        <div class="pagination-container" v-if="pagination.total > 0">
          <el-pagination
            :current-page="pagination.pageIndex"
            :page-size="pagination.pageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="pagination.total"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Search, Refresh, Close } from "@element-plus/icons-vue";
import {
  getOnlineUsers,
  forceLogout,
  type OnlineUserData,
  type GetOnlineUsersRequest
} from "@/api/monitor";

defineOptions({
  name: "MonitorOnline"
});

// 表格数据
const loading = ref(false);
const tableData = ref<OnlineUserData[]>([]);

// 搜索表单
const searchForm = reactive<GetOnlineUsersRequest>({
  userName: "",
  ipAddress: "",
  pageIndex: 1,
  pageSize: 10
});

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
});

// 获取在线用户列表
const fetchOnlineUsers = async () => {
  try {
    loading.value = true;
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    };
    
    const result = await getOnlineUsers(params);
    if (result.success && result.data) {
      tableData.value = result.data.items;
      pagination.total = result.data.totalCount;
    } else {
      ElMessage.error(result.message || "获取在线用户列表失败");
    }
  } catch (error) {
    console.error("获取在线用户列表失败:", error);
    ElMessage.error("获取在线用户列表失败");
  } finally {
    loading.value = false;
  }
};

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1;
  fetchOnlineUsers();
};

// 重置
const handleReset = () => {
  searchForm.userName = "";
  searchForm.ipAddress = "";
  pagination.pageIndex = 1;
  fetchOnlineUsers();
};

// 刷新
const handleRefresh = () => {
  fetchOnlineUsers();
};

// 强制下线
const handleForceLogout = async (row: OnlineUserData) => {
  try {
    await ElMessageBox.confirm(
      `确定要强制用户 "${row.userName}" 下线吗？`,
      "确认下线",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const result = await forceLogout(row.sessionId);
    if (result.success) {
      ElMessage.success("强制下线成功");
      fetchOnlineUsers();
    } else {
      ElMessage.error(result.message || "强制下线失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("强制下线失败:", error);
      ElMessage.error("强制下线失败");
    }
  }
};

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size;
  pagination.pageIndex = 1;
  fetchOnlineUsers();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  fetchOnlineUsers();
};

// 格式化日期时间
const formatDateTime = (dateTime: string) => {
  if (!dateTime) return "-";
  return new Date(dateTime).toLocaleString("zh-CN");
};

// 格式化在线时长
const formatDuration = (minutes: number) => {
  if (minutes < 60) {
    return `${minutes}分钟`;
  } else if (minutes < 1440) { // 24小时
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}小时${mins}分钟`;
  } else {
    const days = Math.floor(minutes / 1440);
    const hours = Math.floor((minutes % 1440) / 60);
    return `${days}天${hours}小时`;
  }
};

// 页面加载时获取数据
onMounted(() => {
  fetchOnlineUsers();
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

.search-container {
  margin-bottom: 20px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
}

.table-container {
  margin-top: 20px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}
</style> 
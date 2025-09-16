<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>登录日志</span>
          <div class="header-buttons">
            <el-button
              type="danger"
              @click="handleBatchDelete"
              :disabled="selectedRows.length === 0"
              v-permission="'monitor:loginlog:delete'"
            >
              <el-icon><Delete /></el-icon>
              批量删除
            </el-button>
            <el-button
              type="danger"
              @click="handleClearAll"
              v-permission="'monitor:loginlog:clear'"
            >
                             <el-icon><Delete /></el-icon>
              清空日志
            </el-button>
            <el-button type="primary" @click="handleRefresh">
              <el-icon><Refresh /></el-icon>
              刷新
            </el-button>
          </div>
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
          <el-form-item label="状态">
            <el-select
              v-model="searchForm.status"
              placeholder="请选择状态"
              clearable
              style="width: 120px"
            >
              <el-option label="成功" :value="1" />
              <el-option label="失败" :value="0" />
            </el-select>
          </el-form-item>
          <el-form-item label="登录时间">
            <el-date-picker
              v-model="dateRange"
              type="datetimerange"
              range-separator="至"
              start-placeholder="开始时间"
              end-placeholder="结束时间"
              format="YYYY-MM-DD HH:mm:ss"
              value-format="YYYY-MM-DD HH:mm:ss"
              @change="handleDateChange"
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
          ref="tableRef"
          :data="tableData"
          v-loading="loading"
          style="width: 100%"
          border
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column type="index" label="序号" width="60" align="center" />
          <el-table-column prop="userName" label="用户名" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.userName || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="ipAddress" label="IP地址" min-width="140" align="center">
            <template #default="{ row }">
              <span>{{ row.ipAddress || '-' }}</span>
            </template>
          </el-table-column>
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
          <el-table-column prop="statusText" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.statusText }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="message" label="提示消息" min-width="200" align="center">
            <template #default="{ row }">
              <span>{{ row.message || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="loginTime" label="登录时间" min-width="180" align="center">
            <template #default="{ row }">
              <span>{{ formatDateTime(row.loginTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" fixed="right" align="center">
            <template #default="{ row }">
              <el-button
                type="danger"
                size="small"
                @click="handleDelete(row)"
                v-permission="'monitor:loginlog:delete'"
              >
                <el-icon><Delete /></el-icon>
                删除
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
import { Search, Refresh, Delete } from "@element-plus/icons-vue";
import {
  getLoginLogs,
  deleteLoginLogs,
  clearLoginLogs,
  type LoginLogData,
  type GetLoginLogsRequest
} from "@/api/monitor";

defineOptions({
  name: "MonitorLoginLog"
});

// 表格数据
const loading = ref(false);
const tableData = ref<LoginLogData[]>([]);
const tableRef = ref();
const selectedRows = ref<LoginLogData[]>([]);

// 搜索表单
const searchForm = reactive<GetLoginLogsRequest>({
  userName: "",
  ipAddress: "",
  status: undefined,
  startTime: "",
  endTime: "",
  pageIndex: 1,
  pageSize: 10
});

// 日期范围
const dateRange = ref<[string, string] | null>(null);

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
});

// 获取登录日志列表
const fetchLoginLogs = async () => {
  try {
    loading.value = true;
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    };
    
    const result = await getLoginLogs(params);
    if (result.success && result.data) {
      tableData.value = result.data.items;
      pagination.total = result.data.totalCount;
    } else {
      ElMessage.error(result.message || "获取登录日志列表失败");
    }
  } catch (error) {
    console.error("获取登录日志列表失败:", error);
    ElMessage.error("获取登录日志列表失败");
  } finally {
    loading.value = false;
  }
};

// 日期范围改变
const handleDateChange = (value: [string, string] | null) => {
  if (value) {
    searchForm.startTime = value[0];
    searchForm.endTime = value[1];
  } else {
    searchForm.startTime = "";
    searchForm.endTime = "";
  }
};

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1;
  fetchLoginLogs();
};

// 重置
const handleReset = () => {
  searchForm.userName = "";
  searchForm.ipAddress = "";
  searchForm.status = undefined;
  searchForm.startTime = "";
  searchForm.endTime = "";
  dateRange.value = null;
  pagination.pageIndex = 1;
  fetchLoginLogs();
};

// 刷新
const handleRefresh = () => {
  fetchLoginLogs();
};

// 表格选择改变
const handleSelectionChange = (selection: LoginLogData[]) => {
  selectedRows.value = selection;
};

// 删除单条记录
const handleDelete = async (row: LoginLogData) => {
  try {
    await ElMessageBox.confirm(
      "确定要删除这条登录日志吗？",
      "确认删除",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const result = await deleteLoginLogs([row.id]);
    if (result.success) {
      ElMessage.success("删除成功");
      fetchLoginLogs();
    } else {
      ElMessage.error(result.message || "删除失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("删除失败:", error);
      ElMessage.error("删除失败");
    }
  }
};

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(
      `确定要删除选中的 ${selectedRows.value.length} 条登录日志吗？`,
      "确认删除",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const ids = selectedRows.value.map(row => row.id);
    const result = await deleteLoginLogs(ids);
    if (result.success) {
      ElMessage.success("删除成功");
      selectedRows.value = [];
      fetchLoginLogs();
    } else {
      ElMessage.error(result.message || "删除失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("批量删除失败:", error);
      ElMessage.error("批量删除失败");
    }
  }
};

// 清空日志
const handleClearAll = async () => {
  try {
    await ElMessageBox.confirm(
      "确定要清空所有登录日志吗？此操作不可恢复！",
      "确认清空",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "error"
      }
    );

    const result = await clearLoginLogs();
    if (result.success) {
      ElMessage.success("清空成功");
      selectedRows.value = [];
      fetchLoginLogs();
    } else {
      ElMessage.error(result.message || "清空失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("清空失败:", error);
      ElMessage.error("清空失败");
    }
  }
};

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size;
  pagination.pageIndex = 1;
  fetchLoginLogs();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  fetchLoginLogs();
};

// 格式化日期时间
const formatDateTime = (dateTime: string) => {
  if (!dateTime) return "-";
  return new Date(dateTime).toLocaleString("zh-CN");
};

// 页面加载时获取数据
onMounted(() => {
  fetchLoginLogs();
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

.header-buttons {
  display: flex;
  gap: 10px;
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
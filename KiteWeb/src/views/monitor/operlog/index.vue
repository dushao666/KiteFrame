<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>操作日志</span>
          <div class="header-buttons">
            <el-button
              type="danger"
              @click="handleBatchDelete"
              :disabled="selectedRows.length === 0"
              v-permission="'monitor:operlog:delete'"
            >
              <el-icon><Delete /></el-icon>
              批量删除
            </el-button>
            <el-button
              type="danger"
              @click="handleClearAll"
              v-permission="'monitor:operlog:clear'"
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
          <el-form-item label="模块">
            <el-input
              v-model="searchForm.module"
              placeholder="请输入模块名称"
              clearable
              style="width: 200px"
              @keyup.enter="handleSearch"
            />
          </el-form-item>
          <el-form-item label="业务类型">
            <el-input
              v-model="searchForm.businessType"
              placeholder="请输入业务类型"
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
              <el-option label="正常" :value="1" />
              <el-option label="异常" :value="0" />
            </el-select>
          </el-form-item>
          <el-form-item label="操作时间">
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
          <el-table-column prop="module" label="模块" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.module || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="businessType" label="业务类型" min-width="120" align="center">
            <template #default="{ row }">
              <span>{{ row.businessType || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="method" label="方法名称" min-width="200" align="center">
            <template #default="{ row }">
              <span>{{ row.method || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="requestMethod" label="请求方式" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getMethodTagType(row.requestMethod)">
                {{ row.requestMethod || '-' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="operatorTypeText" label="操作类别" width="100" align="center" />
          <el-table-column prop="operIp" label="操作IP" min-width="140" align="center">
            <template #default="{ row }">
              <span>{{ row.operIp || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="operLocation" label="操作地点" min-width="150" align="center">
            <template #default="{ row }">
              <span>{{ row.operLocation || '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="statusText" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.statusText }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="costTime" label="消耗时间" width="100" align="center">
            <template #default="{ row }">
              <span>{{ row.costTime ? `${row.costTime}ms` : '-' }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="operTime" label="操作时间" min-width="180" align="center">
            <template #default="{ row }">
              <span>{{ formatDateTime(row.operTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="150" fixed="right" align="center">
            <template #default="{ row }">
              <el-button
                type="primary"
                size="small"
                @click="handleViewDetail(row)"
              >
                <el-icon><View /></el-icon>
                详情
              </el-button>
              <el-button
                type="danger"
                size="small"
                @click="handleDelete(row)"
                v-permission="'monitor:operlog:delete'"
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

    <!-- 详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="操作详情"
      width="900px"
      :close-on-click-modal="false"
    >
      <div class="detail-container" v-if="currentRow">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="用户名">{{ currentRow.userName || '-' }}</el-descriptions-item>
          <el-descriptions-item label="模块">{{ currentRow.module || '-' }}</el-descriptions-item>
          <el-descriptions-item label="业务类型">{{ currentRow.businessType || '-' }}</el-descriptions-item>
          <el-descriptions-item label="操作类别">{{ currentRow.operatorTypeText }}</el-descriptions-item>
          <el-descriptions-item label="请求方式">{{ currentRow.requestMethod || '-' }}</el-descriptions-item>
          <el-descriptions-item label="操作IP">{{ currentRow.operIp || '-' }}</el-descriptions-item>
          <el-descriptions-item label="操作地点">{{ currentRow.operLocation || '-' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="currentRow.status === 1 ? 'success' : 'danger'">
              {{ currentRow.statusText }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="消耗时间">{{ currentRow.costTime ? `${currentRow.costTime}ms` : '-' }}</el-descriptions-item>
          <el-descriptions-item label="操作时间">{{ formatDateTime(currentRow.operTime) }}</el-descriptions-item>
        </el-descriptions>

        <div class="detail-section" v-if="currentRow.operUrl">
          <h4>请求URL</h4>
          <el-input
            :model-value="currentRow.operUrl"
            readonly
            type="textarea"
            :rows="2"
          />
        </div>

        <div class="detail-section" v-if="currentRow.method">
          <h4>方法名称</h4>
          <el-input
            :model-value="currentRow.method"
            readonly
            type="textarea"
            :rows="2"
          />
        </div>

        <div class="detail-section" v-if="currentRow.operParam">
          <h4>请求参数</h4>
          <el-input
            :model-value="currentRow.operParam"
            readonly
            type="textarea"
            :rows="6"
          />
        </div>

        <div class="detail-section" v-if="currentRow.jsonResult">
          <h4>返回参数</h4>
          <el-input
            :model-value="currentRow.jsonResult"
            readonly
            type="textarea"
            :rows="6"
          />
        </div>

        <div class="detail-section" v-if="currentRow.errorMsg">
          <h4>错误消息</h4>
          <el-input
            :model-value="currentRow.errorMsg"
            readonly
            type="textarea"
            :rows="4"
          />
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="detailDialogVisible = false">关闭</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Search, Refresh, Delete, View } from "@element-plus/icons-vue";
import {
  getOperationLogs,
  deleteOperationLogs,
  clearOperationLogs,
  type OperationLogData,
  type GetOperationLogsRequest
} from "@/api/monitor";

defineOptions({
  name: "MonitorOperLog"
});

// 表格数据
const loading = ref(false);
const tableData = ref<OperationLogData[]>([]);
const tableRef = ref();
const selectedRows = ref<OperationLogData[]>([]);

// 搜索表单
const searchForm = reactive<GetOperationLogsRequest>({
  userName: "",
  module: "",
  businessType: "",
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

// 详情对话框
const detailDialogVisible = ref(false);
const currentRow = ref<OperationLogData | null>(null);

// 获取操作日志列表
const fetchOperationLogs = async () => {
  try {
    loading.value = true;
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    };
    
    const result = await getOperationLogs(params);
    if (result.success && result.data) {
      tableData.value = result.data.items;
      pagination.total = result.data.totalCount;
    } else {
      ElMessage.error(result.message || "获取操作日志列表失败");
    }
  } catch (error) {
    console.error("获取操作日志列表失败:", error);
    ElMessage.error("获取操作日志列表失败");
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
  fetchOperationLogs();
};

// 重置
const handleReset = () => {
  searchForm.userName = "";
  searchForm.module = "";
  searchForm.businessType = "";
  searchForm.status = undefined;
  searchForm.startTime = "";
  searchForm.endTime = "";
  dateRange.value = null;
  pagination.pageIndex = 1;
  fetchOperationLogs();
};

// 刷新
const handleRefresh = () => {
  fetchOperationLogs();
};

// 表格选择改变
const handleSelectionChange = (selection: OperationLogData[]) => {
  selectedRows.value = selection;
};

// 查看详情
const handleViewDetail = (row: OperationLogData) => {
  currentRow.value = row;
  detailDialogVisible.value = true;
};

// 删除单条记录
const handleDelete = async (row: OperationLogData) => {
  try {
    await ElMessageBox.confirm(
      "确定要删除这条操作日志吗？",
      "确认删除",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const result = await deleteOperationLogs([row.id]);
    if (result.success) {
      ElMessage.success("删除成功");
      fetchOperationLogs();
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
      `确定要删除选中的 ${selectedRows.value.length} 条操作日志吗？`,
      "确认删除",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const ids = selectedRows.value.map(row => row.id);
    const result = await deleteOperationLogs(ids);
    if (result.success) {
      ElMessage.success("删除成功");
      selectedRows.value = [];
      fetchOperationLogs();
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
      "确定要清空所有操作日志吗？此操作不可恢复！",
      "确认清空",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "error"
      }
    );

    const result = await clearOperationLogs();
    if (result.success) {
      ElMessage.success("清空成功");
      selectedRows.value = [];
      fetchOperationLogs();
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
  fetchOperationLogs();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  fetchOperationLogs();
};

// 格式化日期时间
const formatDateTime = (dateTime: string) => {
  if (!dateTime) return "-";
  return new Date(dateTime).toLocaleString("zh-CN");
};

// 获取请求方式标签类型
const getMethodTagType = (method: string) => {
  switch (method?.toUpperCase()) {
    case 'GET':
      return 'success';
    case 'POST':
      return 'primary';
    case 'PUT':
      return 'warning';
    case 'DELETE':
      return 'danger';
    default:
      return 'info';
  }
};

// 页面加载时获取数据
onMounted(() => {
  fetchOperationLogs();
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

.detail-container {
  max-height: 600px;
  overflow-y: auto;
}

.detail-section {
  margin-top: 20px;
}

.detail-section h4 {
  margin-bottom: 10px;
  color: #409eff;
  font-size: 14px;
}

.dialog-footer {
  text-align: center;
}
</style> 
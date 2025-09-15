<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>用户管理</span>
          <el-button 
            type="primary" 
            @click="handleAdd"
            v-permission="'system:user:add'"
          >
            <el-icon><Plus /></el-icon>
            新增用户
          </el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="关键词">
            <el-input
              v-model="searchForm.keyword"
              placeholder="请输入用户名、姓名或邮箱"
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
          v-loading="loading"
          :data="tableData"
          style="width: 100%"
          border
        >
          <el-table-column prop="id" label="ID" width="80" align="center" />
          <el-table-column prop="userName" label="用户名" width="120" align="center" />
          <el-table-column prop="realName" label="真实姓名" width="120" align="center" />
          <el-table-column prop="email" label="邮箱" width="180" align="center" />
          <el-table-column prop="phone" label="手机号" width="130" align="center" />
          <el-table-column prop="status" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="createTime" label="创建时间" width="160" align="center">
            <template #default="{ row }">
              {{ formatDateTime(row.createTime) }}
            </template>
          </el-table-column>
          <el-table-column prop="remark" label="备注" min-width="120" align="center" />
          <el-table-column label="操作" width="240" fixed="right" align="center">
            <template #default="{ row }">
              <div class="action-buttons">
                <el-button
                  type="primary"
                  size="small"
                  @click="handleEdit(row)"
                  v-permission="'system:user:edit'"
                >
                  <el-icon><Edit /></el-icon>
                  编辑
                </el-button>
                <el-button
                  type="warning"
                  size="small"
                  @click="handleAssignRole(row)"
                  v-permission="'system:user:assign'"
                >
                  <el-icon><Key /></el-icon>
                  分配角色
                </el-button>
                <el-button
                  type="danger"
                  size="small"
                  @click="handleDelete(row)"
                  v-permission="'system:user:delete'"
                >
                  <el-icon><Delete /></el-icon>
                  删除
                </el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>

        <!-- 分页 -->
        <div class="pagination-container">
          <el-pagination
            v-model:current-page="pagination.pageIndex"
            v-model:page-size="pagination.pageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="pagination.total"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="用户名" prop="userName">
          <el-input v-model="formData.userName" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item v-if="!isEdit" label="密码" prop="password">
          <el-input
            v-model="formData.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="真实姓名" prop="realName">
          <el-input v-model="formData.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="formData.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="手机号" prop="phone">
          <el-input v-model="formData.phone" placeholder="请输入手机号" />
        </el-form-item>
        <el-form-item label="钉钉ID" prop="dingTalkId">
          <el-input v-model="formData.dingTalkId" placeholder="请输入钉钉用户ID" />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="formData.status">
            <el-radio :value="1">启用</el-radio>
            <el-radio :value="0">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="3"
            placeholder="请输入备注"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 角色分配对话框 -->
    <el-dialog
      v-model="roleDialogVisible"
      title="分配角色"
      width="500px"
      :close-on-click-modal="false"
    >
      <div class="role-assignment">
        <div class="user-info">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="用户名">{{ currentUser?.userName }}</el-descriptions-item>
            <el-descriptions-item label="真实姓名">{{ currentUser?.realName }}</el-descriptions-item>
          </el-descriptions>
        </div>
        
        <div class="role-selection" style="margin-top: 20px;">
          <el-transfer
            v-model="selectedRoleIds"
            :data="allRoles"
            :titles="['可选角色', '已分配角色']"
            :button-texts="['移除', '分配']"
            :props="{ key: 'id', label: 'roleName' }"
            filterable
            filter-placeholder="搜索角色"
          />
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="roleDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSaveUserRoles" :loading="roleAssignLoading">
            保存
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from "element-plus";
import { Plus, Search, Refresh, Edit, Delete, Key } from "@element-plus/icons-vue";
import {
  getUsers,
  getUserById,
  createUser,
  updateUser,
  deleteUser,
  type UserData,
  type GetUsersRequest,
  type CreateUserRequest,
  type UpdateUserRequest
} from "@/api/user";

defineOptions({
  name: "SystemUser"
});

// 响应式数据
const loading = ref(false);
const submitLoading = ref(false);
const tableData = ref<UserData[]>([]);
const dialogVisible = ref(false);
const isEdit = ref(false);
const currentUserId = ref<number>();
const formRef = ref<FormInstance>();

// 角色分配相关
const roleDialogVisible = ref(false);
const roleAssignLoading = ref(false);
const currentUser = ref<UserData | null>(null);
const allRoles = ref<any[]>([]);
const selectedRoleIds = ref<number[]>([]);

// 搜索表单
const searchForm = reactive<GetUsersRequest>({
  keyword: ""
});

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
});

// 表单数据
const formData = reactive<CreateUserRequest & UpdateUserRequest & { id?: number }>({
  userName: "",
  password: "",
  email: "",
  phone: "",
  realName: "",
  dingTalkId: "",
  status: 1,
  remark: ""
});

// 表单验证规则
const formRules: FormRules = {
  userName: [
    { required: true, message: "请输入用户名", trigger: "blur" },
    { min: 2, max: 50, message: "用户名长度在 2 到 50 个字符", trigger: "blur" }
  ],
  password: [
    { required: true, message: "请输入密码", trigger: "blur" },
    { min: 6, max: 100, message: "密码长度在 6 到 100 个字符", trigger: "blur" }
  ],
  email: [
    {
      validator: (rule: any, value: string, callback: any) => {
        if (!value) {
          callback(); // 允许为空
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
          callback(new Error("请输入正确的邮箱地址"));
        } else {
          callback();
        }
      },
      trigger: "blur"
    }
  ],
  phone: [
    {
      validator: (rule: any, value: string, callback: any) => {
        if (!value) {
          callback(); // 允许为空
        } else if (!/^1[3-9]\d{9}$/.test(value)) {
          callback(new Error("请输入正确的手机号"));
        } else {
          callback();
        }
      },
      trigger: "blur"
    }
  ],
  realName: [
    { max: 50, message: "真实姓名长度不能超过 50 个字符", trigger: "blur" }
  ],
  dingTalkId: [
    { max: 100, message: "钉钉ID长度不能超过 100 个字符", trigger: "blur" }
  ],
  remark: [
    { max: 500, message: "备注长度不能超过 500 个字符", trigger: "blur" }
  ]
};

// 计算属性
const dialogTitle = computed(() => isEdit.value ? "编辑用户" : "新增用户");

// 格式化日期时间
const formatDateTime = (dateTime: string) => {
  if (!dateTime) return "";
  return new Date(dateTime).toLocaleString("zh-CN");
};

// 获取用户列表
const fetchUsers = async () => {
  try {
    loading.value = true;
    const params: GetUsersRequest = {
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      keyword: searchForm.keyword || undefined
    };

    const response = await getUsers(params);
    if (response.success) {
      tableData.value = response.data.items;
      pagination.total = response.data.totalCount;
    } else {
      ElMessage.error(response.message || "获取用户列表失败");
    }
  } catch (error) {
    console.error("获取用户列表失败:", error);
    ElMessage.error("获取用户列表失败");
  } finally {
    loading.value = false;
  }
};

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1;
  fetchUsers();
};

// 重置搜索
const handleReset = () => {
  searchForm.keyword = "";
  pagination.pageIndex = 1;
  fetchUsers();
};

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size;
  pagination.pageIndex = 1;
  fetchUsers();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  fetchUsers();
};

// 新增用户
const handleAdd = () => {
  isEdit.value = false;
  currentUserId.value = undefined;
  resetForm();
  dialogVisible.value = true;
};

// 编辑用户
const handleEdit = async (row: UserData) => {
  try {
    isEdit.value = true;
    currentUserId.value = row.id;

    // 获取用户详情
    const response = await getUserById(row.id);
    if (response.success) {
      Object.assign(formData, response.data);
      dialogVisible.value = true;
    } else {
      ElMessage.error(response.message || "获取用户信息失败");
    }
  } catch (error) {
    console.error("获取用户信息失败:", error);
    ElMessage.error("获取用户信息失败");
  }
};

// 删除用户
const handleDelete = async (row: UserData) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除用户 "${row.userName}" 吗？`,
      "删除确认",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const response = await deleteUser(row.id);
    if (response.success) {
      ElMessage.success("删除成功");
      fetchUsers();
    } else {
      ElMessage.error(response.message || "删除失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("删除用户失败:", error);
      ElMessage.error("删除失败");
    }
  }
};

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    userName: "",
    password: "",
    email: "",
    phone: "",
    realName: "",
    dingTalkId: "",
    status: 1,
    remark: ""
  });
  formRef.value?.clearValidate();
};

// 关闭对话框
const handleDialogClose = () => {
  resetForm();
};

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    const valid = await formRef.value.validate();
    if (!valid) return;

    submitLoading.value = true;

    if (isEdit.value && currentUserId.value) {
      // 更新用户
      const updateData: UpdateUserRequest = {
        userName: formData.userName,
        email: formData.email,
        phone: formData.phone,
        realName: formData.realName,
        dingTalkId: formData.dingTalkId,
        status: formData.status,
        remark: formData.remark
      };

      const response = await updateUser(currentUserId.value, updateData);
      if (response.success) {
        ElMessage.success("更新成功");
        dialogVisible.value = false;
        fetchUsers();
      } else {
        ElMessage.error(response.message || "更新失败");
      }
    } else {
      // 创建用户
      const createData: CreateUserRequest = {
        userName: formData.userName,
        password: formData.password,
        email: formData.email,
        phone: formData.phone,
        realName: formData.realName,
        dingTalkId: formData.dingTalkId,
        status: formData.status,
        remark: formData.remark
      };

      const response = await createUser(createData);
      if (response.success) {
        ElMessage.success("创建成功");
        dialogVisible.value = false;
        fetchUsers();
      } else {
        ElMessage.error(response.message || "创建失败");
      }
    }
  } catch (error) {
    console.error("提交失败:", error);
    ElMessage.error("操作失败");
  } finally {
    submitLoading.value = false;
  }
};

// 分配角色
const handleAssignRole = async (row: UserData) => {
  try {
    currentUser.value = row;
    // 获取所有角色列表
    await fetchAllRoles();
    // 获取用户已分配的角色
    await fetchUserRoles(row.id);
    roleDialogVisible.value = true;
  } catch (error) {
    console.error("获取角色信息失败:", error);
    ElMessage.error("获取角色信息失败");
  }
};

// 获取所有角色
const fetchAllRoles = async () => {
  // 这里需要调用角色API，暂时使用模拟数据
  allRoles.value = [
    { id: 1, roleName: "超级管理员", disabled: false },
    { id: 2, roleName: "系统管理员", disabled: false },
    { id: 3, roleName: "普通用户", disabled: false }
  ];
};

// 获取用户已分配的角色
const fetchUserRoles = async (userId: number) => {
  // 这里需要调用用户角色关联API，暂时使用模拟数据
  selectedRoleIds.value = [3]; // 假设用户已分配普通用户角色
};

// 保存用户角色分配
const handleSaveUserRoles = async () => {
  if (!currentUser.value) return;
  
  try {
    roleAssignLoading.value = true;
    // 这里需要调用保存用户角色的API
    console.log("保存用户角色:", {
      userId: currentUser.value.id,
      roleIds: selectedRoleIds.value
    });
    
    ElMessage.success("角色分配成功");
    roleDialogVisible.value = false;
  } catch (error) {
    console.error("保存用户角色失败:", error);
    ElMessage.error("保存用户角色失败");
  } finally {
    roleAssignLoading.value = false;
  }
};

// 组件挂载时获取数据
onMounted(() => {
  fetchUsers();
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
  background-color: #f5f7fa;
  border-radius: 4px;
}

.table-container {
  padding: 0;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.dialog-footer {
  text-align: right;
}

.el-form-item {
  margin-bottom: 18px;
}

.el-table {
  margin-bottom: 20px;
}

.el-tag {
  font-weight: bold;
}

.action-buttons {
  display: flex;
  gap: 8px;
  justify-content: center;
  align-items: center;
}

.action-buttons .el-button {
  margin: 0;
  min-width: 70px;
}
</style>

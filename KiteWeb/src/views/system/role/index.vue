<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>角色管理</span>
          <el-button 
            type="primary" 
            @click="handleAdd"
            v-permission="'system:role:add'"
          >
            <el-icon><Plus /></el-icon>
            新增角色
          </el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="关键词">
            <el-input
              v-model="searchForm.keyword"
              placeholder="请输入角色名称或角色编码"
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
              <el-option label="启用" :value="1" />
              <el-option label="禁用" :value="0" />
            </el-select>
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
          <el-table-column prop="roleName" label="角色名称" width="150" align="center" />
          <el-table-column prop="roleCode" label="角色编码" width="150" align="center" />
          <el-table-column prop="sort" label="排序" width="80" align="center" />
          <el-table-column prop="status" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="dataScope" label="数据权限" width="130" align="center">
            <template #default="{ row }">
              {{ getDataScopeLabel(row.dataScope) }}
            </template>
          </el-table-column>
          <el-table-column prop="createTime" label="创建时间" width="160" align="center">
            <template #default="{ row }">
              {{ formatDateTime(row.createTime) }}
            </template>
          </el-table-column>
          <el-table-column prop="remark" label="备注" min-width="120" align="center" />
          <el-table-column label="操作" width="280" fixed="right" align="center">
            <template #default="{ row }">
              <div class="action-buttons">
                <el-button
                  type="primary"
                  size="small"
                  @click="handleEdit(row)"
                  v-permission="'system:role:edit'"
                >
                  <el-icon><Edit /></el-icon>
                  编辑
                </el-button>
                <el-button
                  type="warning"
                  size="small"
                  @click="handleAssignPermission(row)"
                  v-permission="'system:role:assign'"
                >
                  <el-icon><Setting /></el-icon>
                  分配权限
                </el-button>
                <el-button
                  type="danger"
                  size="small"
                  @click="handleDelete(row)"
                  v-permission="'system:role:delete'"
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

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="角色名称" prop="roleName">
              <el-input
                v-model="formData.roleName"
                placeholder="请输入角色名称"
                maxlength="50"
                show-word-limit
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="角色编码" prop="roleCode">
              <el-input
                v-model="formData.roleCode"
                placeholder="请输入角色编码"
                maxlength="50"
                show-word-limit
                :disabled="isEdit"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="排序" prop="sort">
              <el-input-number
                v-model="formData.sort"
                :min="0"
                :max="999"
                controls-position="right"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="formData.status">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="数据权限" prop="dataScope">
              <el-select
                v-model="formData.dataScope"
                placeholder="请选择数据权限范围"
                style="width: 100%"
              >
                <el-option
                  v-for="item in dataScopeOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input
                v-model="formData.remark"
                type="textarea"
                placeholder="请输入备注"
                :rows="3"
                maxlength="200"
                show-word-limit
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button
            type="primary"
            :loading="submitLoading"
            @click="handleSubmit"
          >
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 权限分配对话框 -->
    <el-dialog
      v-model="permissionDialogVisible"
      title="分配权限"
      width="700px"
      :close-on-click-modal="false"
    >
      <div class="permission-assignment">
        <div class="role-info" style="background: #f8f9fa; padding: 16px; border-radius: 8px; margin-bottom: 20px;">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="角色名称" align="center" label-align="center">
              <span style="font-weight: 500;">{{ currentRole?.roleName }}</span>
            </el-descriptions-item>
            <el-descriptions-item label="角色编码" align="center" label-align="center">
              <span style="font-weight: 500;">{{ currentRole?.roleCode }}</span>
            </el-descriptions-item>
          </el-descriptions>
        </div>
        
        <div class="menu-selection">
          <div style="display: flex; align-items: center; margin-bottom: 16px; padding: 12px; background: #e8f4fd; border-radius: 6px;">
            <el-icon style="margin-right: 8px; color: #409eff;"><Setting /></el-icon>
            <h4 style="margin: 0; color: #409eff;">菜单权限配置</h4>
          </div>
          <div style="border: 1px solid #e4e7ed; border-radius: 8px; padding: 16px; background: #fafafa;">
            <el-tree
              ref="menuTreeRef"
              :data="menuTreeData"
              show-checkbox
              node-key="id"
              :default-checked-keys="checkedMenuIds"
              :props="{ children: 'children', label: 'menuName' }"
              check-strictly
              style="background: transparent;"
            />
          </div>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer" style="padding: 16px 0; border-top: 1px solid #e4e7ed; margin-top: 20px;">
          <el-button @click="permissionDialogVisible = false" size="large">取消</el-button>
          <el-button type="primary" @click="handleSaveRolePermissions" :loading="permissionAssignLoading" size="large">
            <el-icon style="margin-right: 4px;"><Check /></el-icon>
            保存
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { Plus, Search, Refresh, Edit, Delete, Setting, Check } from "@element-plus/icons-vue";
import type { FormInstance, FormRules } from "element-plus";
import {
  getRoles,
  createRole,
  updateRole,
  deleteRole,
  checkRoleCodeExists,
  dataScopeOptions,
  type RoleData,
  type GetRolesRequest,
  type CreateRoleRequest,
  type UpdateRoleRequest
} from "@/api/role";

defineOptions({
  name: "SystemRole"
});

// 表格数据
const loading = ref(false);
const tableData = ref<RoleData[]>([]);

// 搜索表单
const searchForm = reactive<GetRolesRequest>({
  keyword: "",
  status: undefined,
  pageIndex: 1,
  pageSize: 10
});

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
});

// 对话框相关
const dialogVisible = ref(false);
const dialogTitle = ref("");
const isEdit = ref(false);
const submitLoading = ref(false);
const formRef = ref<FormInstance>();

// 权限分配相关
const permissionDialogVisible = ref(false);
const permissionAssignLoading = ref(false);
const currentRole = ref<RoleData | null>(null);
const menuTreeData = ref<any[]>([]);
const checkedMenuIds = ref<number[]>([]);
const menuTreeRef = ref();

// 表单数据
const formData = reactive<CreateRoleRequest & { id?: number }>({
  roleName: "",
  roleCode: "",
  sort: 0,
  status: 1,
  dataScope: 1,
  remark: ""
});

// 表单验证规则
const formRules: FormRules = {
  roleName: [
    { required: true, message: "请输入角色名称", trigger: "blur" },
    { min: 2, max: 50, message: "角色名称长度在 2 到 50 个字符", trigger: "blur" }
  ],
  roleCode: [
    { required: true, message: "请输入角色编码", trigger: "blur" },
    { min: 2, max: 50, message: "角色编码长度在 2 到 50 个字符", trigger: "blur" },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: "角色编码只能包含字母、数字和下划线，且以字母开头", trigger: "blur" }
  ],
  sort: [
    { required: true, message: "请输入排序号", trigger: "blur" }
  ],
  status: [
    { required: true, message: "请选择状态", trigger: "change" }
  ],
  dataScope: [
    { required: true, message: "请选择数据权限范围", trigger: "change" }
  ]
};

// 获取数据权限范围标签
const getDataScopeLabel = (value: number) => {
  const option = dataScopeOptions.find(item => item.value === value);
  return option ? option.label : "未知";
};

// 格式化日期时间
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return "";
  const date = new Date(dateStr);
  return date.toLocaleString("zh-CN", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit"
  });
};

// 获取角色列表
const fetchRoles = async () => {
  loading.value = true;
  try {
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    };
    const data = await getRoles(params);
    if (data.success) {
      tableData.value = data.data.items;
      pagination.total = data.data.totalCount;
    } else {
      ElMessage.error(data.message || "获取角色列表失败");
    }
  } catch (error) {
    console.error("获取角色列表失败:", error);
    ElMessage.error("获取角色列表失败");
  } finally {
    loading.value = false;
  }
};

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1;
  fetchRoles();
};

// 重置搜索
const handleReset = () => {
  searchForm.keyword = "";
  searchForm.status = undefined;
  pagination.pageIndex = 1;
  fetchRoles();
};

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size;
  pagination.pageIndex = 1;
  fetchRoles();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  fetchRoles();
};

// 新增角色
const handleAdd = () => {
  isEdit.value = false;
  dialogTitle.value = "新增角色";
  resetForm();
  dialogVisible.value = true;
};

// 编辑角色
const handleEdit = (row: RoleData) => {
  isEdit.value = true;
  dialogTitle.value = "编辑角色";
  Object.assign(formData, {
    id: row.id,
    roleName: row.roleName,
    roleCode: row.roleCode,
    sort: row.sort,
    status: row.status,
    dataScope: row.dataScope,
    remark: row.remark || ""
  });
  dialogVisible.value = true;
};

// 删除角色
const handleDelete = async (row: RoleData) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除角色"${row.roleName}"吗？`,
      "删除确认",
      {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }
    );

    const data = await deleteRole(row.id);
    if (data.success) {
      ElMessage.success("删除成功");
      fetchRoles();
    } else {
      ElMessage.error(data.message || "删除失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("删除角色失败:", error);
      ElMessage.error("删除失败");
    }
  }
};

// 重置表单
const resetForm = () => {
  formData.id = undefined;
  formData.roleName = "";
  formData.roleCode = "";
  formData.sort = 0;
  formData.status = 1;
  formData.dataScope = 1;
  formData.remark = "";
  formRef.value?.clearValidate();
};

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return;
  
  try {
    await formRef.value.validate();
    
    // 检查角色编码是否已存在
    if (!isEdit.value || (isEdit.value && formData.roleCode)) {
      const checkResult = await checkRoleCodeExists(
        formData.roleCode,
        isEdit.value ? formData.id : undefined
      );
      if (checkResult.success && checkResult.data) {
        ElMessage.error("角色编码已存在");
        return;
      }
    }
    
    submitLoading.value = true;
    
    if (isEdit.value) {
      // 更新角色
      const updateData: UpdateRoleRequest = {
        roleName: formData.roleName,
        roleCode: formData.roleCode,
        sort: formData.sort,
        status: formData.status,
        dataScope: formData.dataScope,
        remark: formData.remark
      };
      const data = await updateRole(formData.id!, updateData);
      if (data.success) {
        ElMessage.success("更新成功");
        dialogVisible.value = false;
        fetchRoles();
      } else {
        ElMessage.error(data.message || "更新失败");
      }
    } else {
      // 创建角色
      const createData: CreateRoleRequest = {
        roleName: formData.roleName,
        roleCode: formData.roleCode,
        sort: formData.sort,
        status: formData.status,
        dataScope: formData.dataScope,
        remark: formData.remark
      };
      const data = await createRole(createData);
      if (data.success) {
        ElMessage.success("创建成功");
        dialogVisible.value = false;
        fetchRoles();
      } else {
        ElMessage.error(data.message || "创建失败");
      }
    }
  } catch (error) {
    console.error("提交失败:", error);
    ElMessage.error("操作失败");
  } finally {
    submitLoading.value = false;
  }
};

// 分配权限
const handleAssignPermission = async (row: RoleData) => {
  try {
    currentRole.value = row;
    // 获取菜单树数据
    await fetchMenuTree();
    // 获取角色已分配的权限
    await fetchRolePermissions(row.id);
    permissionDialogVisible.value = true;
  } catch (error) {
    console.error("获取权限信息失败:", error);
    ElMessage.error("获取权限信息失败");
  }
};

// 获取菜单树数据
const fetchMenuTree = async () => {
  // 这里需要调用菜单树API，暂时使用模拟数据
  menuTreeData.value = [
    {
      id: 1,
      menuName: "系统管理",
      menuType: 1,
      icon: "Setting",
      children: [
        { id: 2, menuName: "用户管理", menuType: 2, icon: "User" },
        { id: 3, menuName: "角色管理", menuType: 2, icon: "Avatar" },
        { id: 4, menuName: "菜单管理", menuType: 2, icon: "Menu" },
        { id: 5, menuName: "新增用户", menuType: 3, parentId: 2 },
        { id: 6, menuName: "编辑用户", menuType: 3, parentId: 2 },
        { id: 7, menuName: "删除用户", menuType: 3, parentId: 2 }
      ]
    }
  ];
};

// 获取角色已分配的权限
const fetchRolePermissions = async (roleId: number) => {
  // 这里需要调用角色权限关联API，暂时使用模拟数据
  checkedMenuIds.value = [2, 3, 5, 6]; // 假设角色已分配这些权限
};

// 保存角色权限分配
const handleSaveRolePermissions = async () => {
  if (!currentRole.value || !menuTreeRef.value) return;
  
  try {
    permissionAssignLoading.value = true;
    
    // 获取选中的菜单ID
    const checkedKeys = menuTreeRef.value.getCheckedKeys();
    const halfCheckedKeys = menuTreeRef.value.getHalfCheckedKeys();
    const allSelectedKeys = [...checkedKeys, ...halfCheckedKeys];
    
    // 这里需要调用保存角色权限的API
    console.log("保存角色权限:", {
      roleId: currentRole.value.id,
      menuIds: allSelectedKeys
    });
    
    ElMessage.success("权限分配成功");
    permissionDialogVisible.value = false;
  } catch (error) {
    console.error("保存角色权限失败:", error);
    ElMessage.error("保存角色权限失败");
  } finally {
    permissionAssignLoading.value = false;
  }
};

// 页面加载时获取数据
onMounted(() => {
  fetchRoles();
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
  padding: 20px 0;
  border-bottom: 1px solid #f0f0f0;
  margin-bottom: 20px;
}

.table-container {
  padding: 20px 0;
}

.pagination-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
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

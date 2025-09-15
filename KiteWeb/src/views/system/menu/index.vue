<template>
  <div class="main">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>菜单管理</span>
          <div class="header-buttons">
            <el-button @click="expandAll">
              <el-icon><ArrowDown /></el-icon>
              展开全部
            </el-button>
            <el-button @click="collapseAll">
              <el-icon><ArrowUp /></el-icon>
              折叠全部
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              新增菜单
            </el-button>
          </div>
        </div>
      </template>
      
      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="菜单名称">
            <el-input
              v-model="searchForm.menuName"
              placeholder="请输入菜单名称"
              clearable
              style="width: 200px"
            />
          </el-form-item>
          <el-form-item label="菜单编码">
            <el-input
              v-model="searchForm.menuCode"
              placeholder="请输入菜单编码"
              clearable
              style="width: 200px"
            />
          </el-form-item>
          <el-form-item label="菜单类型">
            <el-select
              v-model="searchForm.menuType"
              placeholder="请选择菜单类型"
              clearable
              style="width: 150px"
            >
              <el-option label="目录" :value="1" />
              <el-option label="菜单" :value="2" />
              <el-option label="按钮" :value="3" />
            </el-select>
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
          :data="menuList"
          row-key="id"
          :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
          :default-expand-all="false"
          :expand-row-keys="expandedKeys"
          border
          @expand-change="handleExpandChange"
        >
          <el-table-column prop="menuName" label="菜单名称" min-width="150" />
          <el-table-column prop="menuCode" label="菜单编码" min-width="120" />
          <el-table-column prop="menuType" label="菜单类型" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getMenuTypeTagType(row.menuType)">
                {{ getMenuTypeText(row.menuType) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="icon" label="图标" width="80" align="center">
            <template #default="{ row }">
              <el-icon v-if="row.icon">
                <component :is="row.icon" />
              </el-icon>
              <span v-else>-</span>
            </template>
          </el-table-column>
          <el-table-column prop="path" label="路由路径" min-width="150" />
          <el-table-column prop="component" label="组件路径" min-width="150" />
          <el-table-column prop="sort" label="排序" width="80" align="center" />
          <el-table-column prop="isVisible" label="显示" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.isVisible ? 'success' : 'danger'">
                {{ row.isVisible ? '是' : '否' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="status" label="状态" width="80" align="center">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="createTime" label="创建时间" width="180" />
          <el-table-column label="操作" width="200" fixed="right">
            <template #default="{ row }">
              <el-button type="primary" size="small" @click="handleEdit(row)">
                编辑
              </el-button>
              <el-button type="success" size="small" @click="handleAddChild(row)">
                新增子菜单
              </el-button>
              <el-button type="danger" size="small" @click="handleDelete(row)">
                删除
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :total="pagination.totalCount"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
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
            <el-form-item label="父菜单" prop="parentId">
              <el-tree-select
                v-model="formData.parentId"
                :data="menuTreeOptions"
                :props="{ value: 'id', label: 'menuName', children: 'children' }"
                placeholder="请选择父菜单"
                check-strictly
                clearable
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单类型" prop="menuType">
              <el-radio-group v-model="formData.menuType">
                <el-radio :label="1">目录</el-radio>
                <el-radio :label="2">菜单</el-radio>
                <el-radio :label="3">按钮</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单名称" prop="menuName">
              <el-input v-model="formData.menuName" placeholder="请输入菜单名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单编码" prop="menuCode">
              <el-input v-model="formData.menuCode" placeholder="请输入菜单编码" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20" v-if="formData.menuType !== 3">
          <el-col :span="12">
            <el-form-item label="路由路径" prop="path">
              <el-input v-model="formData.path" placeholder="请输入路由路径" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="formData.menuType === 2">
            <el-form-item label="组件路径" prop="component">
              <el-input v-model="formData.component" placeholder="请输入组件路径" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单图标" prop="icon">
              <IconSelector v-model="formData.icon" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="排序号" prop="sort">
              <el-input-number
                v-model="formData.sort"
                :min="0"
                :max="9999"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="是否显示">
              <el-switch v-model="formData.isVisible" />
            </el-form-item>
          </el-col>
          <el-col :span="8" v-if="formData.menuType === 2">
            <el-form-item label="是否缓存">
              <el-switch v-model="formData.isCache" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="是否外链">
              <el-switch v-model="formData.isFrame" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="状态">
              <el-radio-group v-model="formData.status">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="formData.menuType === 3">
            <el-form-item label="权限标识" prop="permissions">
              <el-input v-model="formData.permissions" placeholder="请输入权限标识" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="备注">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="3"
            placeholder="请输入备注"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick } from "vue";
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from "element-plus";
import { Plus, Search, Refresh, ArrowDown, ArrowUp } from "@element-plus/icons-vue";
import IconSelector from "@/components/IconSelector/index.vue";
import {
  getMenus,
  getMenuTree,
  createMenu,
  updateMenu,
  deleteMenu,
  type MenuData,
  type GetMenusRequest,
  type CreateMenuRequest,
  type UpdateMenuRequest,
  MenuType
} from "@/api/menu";

defineOptions({
  name: "SystemMenu"
});

// 响应式数据
const loading = ref(false);
const submitLoading = ref(false);
const dialogVisible = ref(false);
const dialogTitle = ref("");
const isEdit = ref(false);
const currentEditId = ref<number>(0);

// 搜索表单
const searchForm = reactive<GetMenusRequest>({
  pageIndex: 1,
  pageSize: 20,
  menuName: "",
  menuCode: "",
  menuType: undefined,
  status: undefined
});

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
});

// 树选择器选项类型
type TreeSelectOption = {
  id: number;
  menuName: string;
  children?: TreeSelectOption[];
};

// 菜单列表
const menuList = ref<MenuData[]>([]);
const menuTreeOptions = ref<TreeSelectOption[]>([]);
const expandedKeys = ref<string[]>([]);

// 表单相关
const formRef = ref<FormInstance>();
const formData = reactive<CreateMenuRequest | UpdateMenuRequest>({
  parentId: 0,
  menuName: "",
  menuCode: "",
  menuType: MenuType.Directory,
  path: "",
  component: "",
  icon: "",
  sort: 0,
  isVisible: true,
  isCache: false,
  isFrame: false,
  status: 1,
  permissions: "",
  remark: ""
});

// 表单验证规则
const formRules: FormRules = {
  menuName: [
    { required: true, message: "请输入菜单名称", trigger: "blur" },
    { min: 1, max: 50, message: "菜单名称长度在 1 到 50 个字符", trigger: "blur" }
  ],
  menuCode: [
    { required: true, message: "请输入菜单编码", trigger: "blur" },
    { min: 1, max: 50, message: "菜单编码长度在 1 到 50 个字符", trigger: "blur" }
  ],
  menuType: [
    { required: true, message: "请选择菜单类型", trigger: "change" }
  ],
  path: [
    { required: true, message: "请输入路由路径", trigger: "blur" }
  ],
  component: [
    { required: true, message: "请输入组件路径", trigger: "blur" }
  ],
  sort: [
    { required: true, message: "请输入排序号", trigger: "blur" }
  ]
};

// 获取菜单类型文本
const getMenuTypeText = (type: number) => {
  switch (type) {
    case 1: return "目录";
    case 2: return "菜单";
    case 3: return "按钮";
    default: return "未知";
  }
};

// 获取菜单类型标签类型
const getMenuTypeTagType = (type: number) => {
  switch (type) {
    case 1: return "warning";
    case 2: return "success";
    case 3: return "info";
    default: return "primary";
  }
};

// 获取菜单列表 - 使用树形结构
const fetchMenuList = async () => {
  try {
    loading.value = true;
    // 如果有搜索条件，使用分页接口
    if (searchForm.menuName || searchForm.menuCode || searchForm.menuType || searchForm.status !== undefined) {
      const response = await getMenus(searchForm);
      if (response.success) {
        menuList.value = response.data.items;
        pagination.totalCount = response.data.totalCount;
        pagination.pageIndex = response.data.pageIndex;
        pagination.pageSize = response.data.pageSize;
      } else {
        ElMessage.error(response.message || "获取菜单列表失败");
      }
    } else {
      // 没有搜索条件时，使用树形接口
      const response = await getMenuTree();
      if (response.success) {
        menuList.value = response.data;
        pagination.totalCount = response.data.length;
        pagination.pageIndex = 1;
        pagination.pageSize = response.data.length;
      } else {
        ElMessage.error(response.message || "获取菜单树失败");
      }
    }
  } catch (error) {
    console.error("获取菜单列表失败:", error);
    ElMessage.error("获取菜单列表失败");
  } finally {
    loading.value = false;
  }
};

// 转换菜单数据为树选择器格式
const convertToTreeOptions = (menus: MenuData[]): TreeSelectOption[] => {
  return menus.map(menu => ({
    id: menu.id,
    menuName: menu.menuName,
    children: menu.children ? convertToTreeOptions(menu.children) : undefined
  }));
};

// 获取菜单树
const fetchMenuTree = async () => {
  try {
    const response = await getMenuTree();
    if (response.success) {
      menuTreeOptions.value = [
        { 
          id: 0, 
          menuName: "根目录", 
          children: convertToTreeOptions(response.data)
        }
      ];
    }
  } catch (error) {
    console.error("获取菜单树失败:", error);
  }
};

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1;
  searchForm.pageIndex = 1;
  fetchMenuList();
};

// 重置搜索
const handleReset = () => {
  Object.assign(searchForm, {
    pageIndex: 1,
    pageSize: 20,
    menuName: "",
    menuCode: "",
    menuType: undefined,
    status: undefined
  });
  fetchMenuList();
};

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size;
  searchForm.pageSize = size;
  fetchMenuList();
};

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page;
  searchForm.pageIndex = page;
  fetchMenuList();
};

// 处理表格行展开/折叠
const handleExpandChange = (row: MenuData, expandedRows: MenuData[]) => {
  const isExpanded = expandedRows.some(item => item.id === row.id);
  const rowKey = row.id.toString();
  if (isExpanded) {
    // 展开时添加到展开列表
    if (!expandedKeys.value.includes(rowKey)) {
      expandedKeys.value.push(rowKey);
    }
  } else {
    // 折叠时从展开列表移除
    const index = expandedKeys.value.indexOf(rowKey);
    if (index > -1) {
      expandedKeys.value.splice(index, 1);
    }
  }
};

// 获取所有菜单ID（递归）
const getAllMenuIds = (menus: MenuData[]): string[] => {
  const ids: string[] = [];
  const traverse = (items: MenuData[]) => {
    items.forEach(item => {
      ids.push(item.id.toString());
      if (item.children && item.children.length > 0) {
        traverse(item.children);
      }
    });
  };
  traverse(menus);
  return ids;
};

// 展开全部
const expandAll = () => {
  expandedKeys.value = getAllMenuIds(menuList.value);
};

// 折叠全部
const collapseAll = () => {
  expandedKeys.value = [];
};

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    parentId: 0,
    menuName: "",
    menuCode: "",
    menuType: MenuType.Directory,
    path: "",
    component: "",
    icon: "",
    sort: 0,
    isVisible: true,
    isCache: false,
    isFrame: false,
    status: 1,
    permissions: "",
    remark: ""
  });
  nextTick(() => {
    formRef.value?.clearValidate();
  });
};

// 新增菜单
const handleAdd = () => {
  resetForm();
  isEdit.value = false;
  dialogTitle.value = "新增菜单";
  dialogVisible.value = true;
  fetchMenuTree();
};

// 新增子菜单
const handleAddChild = (row: MenuData) => {
  resetForm();
  formData.parentId = row.id;
  isEdit.value = false;
  dialogTitle.value = "新增子菜单";
  dialogVisible.value = true;
  fetchMenuTree();
};

// 编辑菜单
const handleEdit = (row: MenuData) => {
  resetForm();
  Object.assign(formData, {
    parentId: row.parentId,
    menuName: row.menuName,
    menuCode: row.menuCode,
    menuType: row.menuType,
    path: row.path || "",
    component: row.component || "",
    icon: row.icon || "",
    sort: row.sort,
    isVisible: row.isVisible,
    isCache: row.isCache,
    isFrame: row.isFrame,
    status: row.status,
    permissions: row.permissions || "",
    remark: row.remark || ""
  });
  isEdit.value = true;
  currentEditId.value = row.id;
  dialogTitle.value = "编辑菜单";
  dialogVisible.value = true;
  fetchMenuTree();
};

// 删除菜单
const handleDelete = async (row: MenuData) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除菜单"${row.menuName}"吗？`,
      "确认删除",
      {
        type: "warning"
      }
    );

    const response = await deleteMenu(row.id);
    if (response.success) {
      ElMessage.success("删除成功");
      fetchMenuList();
    } else {
      ElMessage.error(response.message || "删除失败");
    }
  } catch (error) {
    if (error !== "cancel") {
      console.error("删除菜单失败:", error);
      ElMessage.error("删除失败");
    }
  }
};

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    await formRef.value.validate();
    submitLoading.value = true;

    let response;
    if (isEdit.value) {
      response = await updateMenu(currentEditId.value, formData as UpdateMenuRequest);
    } else {
      response = await createMenu(formData as CreateMenuRequest);
    }

    if (response.success) {
      ElMessage.success(isEdit.value ? "更新成功" : "创建成功");
      dialogVisible.value = false;
      fetchMenuList();
    } else {
      ElMessage.error(response.message || (isEdit.value ? "更新失败" : "创建失败"));
    }
  } catch (error) {
    console.error("提交表单失败:", error);
  } finally {
    submitLoading.value = false;
  }
};

// 组件挂载时获取数据
onMounted(() => {
  fetchMenuList();
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
  gap: 8px;
  align-items: center;
}

.search-container {
  margin-bottom: 20px;
  padding: 20px;
  background-color: #f5f5f5;
  border-radius: 4px;
}

.table-container {
  margin-bottom: 20px;
}

.pagination-container {
  display: flex;
  justify-content: flex-end;
}
</style>

<template>
  <div class="icon-selector">
    <el-input
      v-model="selectedIcon"
      placeholder="请选择图标"
      readonly
      @click="dialogVisible = true"
    >
      <template #prefix>
        <el-icon v-if="selectedIcon">
          <component :is="iconComponents[selectedIcon]" />
        </el-icon>
      </template>
      <template #suffix>
        <el-icon class="cursor-pointer" @click="dialogVisible = true">
          <Search />
        </el-icon>
      </template>
    </el-input>

    <el-dialog
      v-model="dialogVisible"
      title="选择图标"
      width="800px"
      :close-on-click-modal="false"
    >
      <div class="icon-dialog-content">
        <!-- 搜索框 -->
        <el-input
          v-model="searchKeyword"
          placeholder="搜索图标..."
          clearable
          class="search-input"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>

        <!-- 图标分类 -->
        <el-tabs v-model="activeTab" class="icon-tabs">
          <el-tab-pane label="Element Plus" name="element">
            <div class="icon-grid">
              <div
                v-for="icon in filteredElementIcons"
                :key="icon.name"
                class="icon-item"
                :class="{ active: selectedIcon === icon.component }"
                @click="selectIcon(icon.component, icon.name)"
              >
                <el-icon :size="24">
                  <component :is="iconComponents[icon.component]" />
                </el-icon>
                <span class="icon-name">{{ icon.name }}</span>
              </div>
            </div>
          </el-tab-pane>
          
          <el-tab-pane label="系统图标" name="system">
            <div class="icon-grid">
              <div
                v-for="icon in filteredSystemIcons"
                :key="icon.name"
                class="icon-item"
                :class="{ active: selectedIcon === icon.component }"
                @click="selectIcon(icon.component, icon.name)"
              >
                <el-icon :size="24">
                  <component :is="iconComponents[icon.component]" />
                </el-icon>
                <span class="icon-name">{{ icon.name }}</span>
              </div>
            </div>
          </el-tab-pane>
        </el-tabs>
      </div>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button @click="clearIcon">清空</el-button>
        <el-button type="primary" @click="confirmSelection">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import { 
  Search,
  User,
  Setting,
  Menu,
  House,
  Document,
  Folder,
  Files,
  Edit,
  Delete,
  Plus,
  Minus,
  Close,
  Check,
  Warning,
  InfoFilled,
  SuccessFilled,
  CircleCloseFilled,
  QuestionFilled,
  Star,
  StarFilled,
  Location,
  Clock,
  Calendar,
  Phone,
  Message,
  ChatDotRound,
  Bell,
  ShoppingCart,
  Wallet,
  Present,
  Camera,
  Picture,
  VideoCamera,
  Tools,
  Link,
  Share,
  Download,
  Upload,
  Refresh,
  Loading,
  Lock,
  Unlock,
  View,
  Hide,
  ArrowUp,
  ArrowDown,
  ArrowLeft,
  ArrowRight,
  Expand,
  Fold,
  FullScreen,
  Aim,
  CopyDocument,
  DocumentCopy,
  Scissor,
  Printer,
  Odometer,
  TrendCharts,
  DataAnalysis,
  Monitor,
  Coin,
  Connection,
  Notebook,
  List,
  Operation,
  Checked,
  Key,
  Avatar,
  OfficeBuilding,
  Coordinate,
  SetUp,
  Platform,
  Grid,
  Memo,
  MagicStick
} from "@element-plus/icons-vue";

// 图标组件映射
const iconComponents = {
  Search,
  User,
  Setting,
  Menu,
  House,
  Document,
  Folder,
  Files,
  Edit,
  Delete,
  Plus,
  Minus,
  Close,
  Check,
  Warning,
  InfoFilled,
  SuccessFilled,
  CircleCloseFilled,
  QuestionFilled,
  Star,
  StarFilled,
  Location,
  Clock,
  Calendar,
  Phone,
  Message,
  ChatDotRound,
  Bell,
  ShoppingCart,
  Wallet,
  Present,
  Camera,
  Picture,
  VideoCamera,
  Tools,
  Link,
  Share,
  Download,
  Upload,
  Refresh,
  Loading,
  Lock,
  Unlock,
  View,
  Hide,
  ArrowUp,
  ArrowDown,
  ArrowLeft,
  ArrowRight,
  Expand,
  Fold,
  FullScreen,
  Aim,
  CopyDocument,
  DocumentCopy,
  Scissor,
  Printer,
  Odometer,
  TrendCharts,
  DataAnalysis,
  Monitor,
  Coin,
  Connection,
  Notebook,
  List,
  Operation,
  Checked,
  Key,
  Avatar,
  OfficeBuilding,
  Coordinate,
  SetUp,
  Platform,
  Grid,
  Memo,
  MagicStick
};

// Props
interface Props {
  modelValue?: string;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: ""
});

// Emits
interface Emits {
  (e: "update:modelValue", value: string): void;
}

const emit = defineEmits<Emits>();

// 响应式数据
const dialogVisible = ref(false);
const selectedIcon = ref(props.modelValue);
const searchKeyword = ref("");
const activeTab = ref("element");

// Element Plus 图标列表
const elementIcons = ref([
  { name: "用户", component: "User" },
  { name: "设置", component: "Setting" },
  { name: "菜单", component: "Menu" },
  { name: "首页", component: "House" },
  { name: "文档", component: "Document" },
  { name: "文件夹", component: "Folder" },
  { name: "文件", component: "Files" },
  { name: "编辑", component: "Edit" },
  { name: "删除", component: "Delete" },
  { name: "搜索", component: "Search" },
  { name: "添加", component: "Plus" },
  { name: "减少", component: "Minus" },
  { name: "关闭", component: "Close" },
  { name: "检查", component: "Check" },
  { name: "警告", component: "Warning" },
  { name: "信息", component: "InfoFilled" },
  { name: "成功", component: "SuccessFilled" },
  { name: "错误", component: "CircleCloseFilled" },
  { name: "问号", component: "QuestionFilled" },
  { name: "星星", component: "Star" },
  { name: "收藏", component: "StarFilled" },
  { name: "位置", component: "Location" },
  { name: "时间", component: "Clock" },
  { name: "日历", component: "Calendar" },
  { name: "电话", component: "Phone" },
  { name: "邮件", component: "Message" },
  { name: "聊天", component: "ChatDotRound" },
  { name: "通知", component: "Bell" },
  { name: "购物车", component: "ShoppingCart" },
  { name: "钱包", component: "Wallet" },
  { name: "礼物", component: "Present" },
  { name: "相机", component: "Camera" },
  { name: "图片", component: "Picture" },
  { name: "视频", component: "VideoCamera" },
  { name: "工具", component: "Tools" },
  { name: "链接", component: "Link" },
  { name: "分享", component: "Share" },
  { name: "下载", component: "Download" },
  { name: "上传", component: "Upload" },
  { name: "刷新", component: "Refresh" },
  { name: "加载", component: "Loading" },
  { name: "锁定", component: "Lock" },
  { name: "解锁", component: "Unlock" },
  { name: "眼睛", component: "View" },
  { name: "隐藏", component: "Hide" },
  { name: "箭头上", component: "ArrowUp" },
  { name: "箭头下", component: "ArrowDown" },
  { name: "箭头左", component: "ArrowLeft" },
  { name: "箭头右", component: "ArrowRight" },
  { name: "展开", component: "Expand" },
  { name: "折叠", component: "Fold" },
  { name: "全屏", component: "FullScreen" },
  { name: "退出全屏", component: "Aim" },
  { name: "复制", component: "CopyDocument" },
  { name: "粘贴", component: "DocumentCopy" },
  { name: "剪切", component: "Scissor" },
  { name: "打印", component: "Printer" }
]);

// 系统常用图标
const systemIcons = ref([
  { name: "仪表盘", component: "Odometer" },
  { name: "数据分析", component: "TrendCharts" },
  { name: "报表", component: "DataAnalysis" },
  { name: "监控", component: "Monitor" },
  { name: "数据库", component: "Coin" },
  { name: "API", component: "Connection" },
  { name: "日志", component: "Notebook" },
  { name: "任务", component: "List" },
  { name: "流程", component: "Operation" },
  { name: "审批", component: "Checked" },
  { name: "权限", component: "Key" },
  { name: "角色", component: "Avatar" },
  { name: "部门", component: "OfficeBuilding" },
  { name: "组织", component: "Coordinate" },
  { name: "配置", component: "SetUp" },
  { name: "系统", component: "Platform" },
  { name: "模块", component: "Grid" },
  { name: "组件", component: "Memo" },
  { name: "插件", component: "MagicStick" }
]);

// 过滤后的图标列表
const filteredElementIcons = computed(() => {
  if (!searchKeyword.value) return elementIcons.value;
  return elementIcons.value.filter(icon => 
    icon.name.toLowerCase().includes(searchKeyword.value.toLowerCase()) ||
    icon.component.toLowerCase().includes(searchKeyword.value.toLowerCase())
  );
});

const filteredSystemIcons = computed(() => {
  if (!searchKeyword.value) return systemIcons.value;
  return systemIcons.value.filter(icon => 
    icon.name.toLowerCase().includes(searchKeyword.value.toLowerCase()) ||
    icon.component.toLowerCase().includes(searchKeyword.value.toLowerCase())
  );
});

// 选择图标
const selectIcon = (component: string, name: string) => {
  selectedIcon.value = component;
};

// 确认选择
const confirmSelection = () => {
  emit("update:modelValue", selectedIcon.value);
  dialogVisible.value = false;
};

// 清空图标
const clearIcon = () => {
  selectedIcon.value = "";
  emit("update:modelValue", "");
  dialogVisible.value = false;
};

// 监听 props 变化
watch(() => props.modelValue, (newValue) => {
  selectedIcon.value = newValue;
});
</script>

<style scoped>
.icon-selector {
  width: 100%;
}

.cursor-pointer {
  cursor: pointer;
}

.icon-dialog-content {
  max-height: 500px;
}

.search-input {
  margin-bottom: 20px;
}

.icon-tabs {
  height: 400px;
}

.icon-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 12px;
  max-height: 320px;
  overflow-y: auto;
  padding: 10px;
}

.icon-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 12px 8px;
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
  background-color: #fff;
}

.icon-item:hover {
  border-color: #409eff;
  background-color: #ecf5ff;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(64, 158, 255, 0.15);
}

.icon-item.active {
  border-color: #409eff;
  background-color: #409eff;
  color: #fff;
}

.icon-item.active .icon-name {
  color: #fff;
}

.icon-name {
  margin-top: 8px;
  font-size: 12px;
  color: #606266;
  text-align: center;
  line-height: 1.2;
  word-break: break-all;
}

/* 滚动条样式 */
.icon-grid::-webkit-scrollbar {
  width: 6px;
}

.icon-grid::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.icon-grid::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.icon-grid::-webkit-scrollbar-thumb:hover {
  background: #a1a1a1;
}
</style>

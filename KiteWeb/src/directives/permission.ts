import type { Directive, DirectiveBinding } from "vue";
import { useUserStore } from "@/store/modules/user";

interface PermissionValue {
  permission?: string;
  role?: string;
  permissions?: string[];
  roles?: string[];
  mode?: "and" | "or";
}

// 检查权限的函数
function checkPermission(value: string | PermissionValue): boolean {
  const userStore = useUserStore();
  const userPermissions = userStore.permissions || [];
  const userRoles = userStore.roles || [];

  // 如果传入的是字符串，直接检查权限
  if (typeof value === "string") {
    return userPermissions.includes(value);
  }

  // 如果传入的是对象，进行复杂权限检查
  const { permission, role, permissions, roles, mode = "or" } = value;

  let permissionChecks: boolean[] = [];
  let roleChecks: boolean[] = [];

  // 检查单个权限
  if (permission) {
    permissionChecks.push(userPermissions.includes(permission));
  }

  // 检查多个权限
  if (permissions?.length) {
    const hasAnyPermission = permissions.some(p => userPermissions.includes(p));
    permissionChecks.push(hasAnyPermission);
  }

  // 检查单个角色
  if (role) {
    roleChecks.push(userRoles.includes(role));
  }

  // 检查多个角色
  if (roles?.length) {
    const hasAnyRole = roles.some(roleCode => userRoles.includes(roleCode));
    roleChecks.push(hasAnyRole);
  }

  const allChecks = [...permissionChecks, ...roleChecks];

  if (allChecks.length === 0) {
    return true;
  }

  // 根据模式返回结果
  if (mode === "and") {
    return allChecks.every(check => check);
  } else {
    return allChecks.some(check => check);
  }
}

// 权限指令
export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const hasPermission = checkPermission(binding.value);
    if (!hasPermission) {
      // 移除元素
      el.remove();
    }
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const hasPermission = checkPermission(binding.value);
    if (!hasPermission) {
      // 隐藏元素
      el.style.display = "none";
    } else {
      // 显示元素
      el.style.display = "";
    }
  }
};

// 权限禁用指令（不移除元素，只是禁用）
export const permissionDisabled: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const hasPermission = checkPermission(binding.value);
    if (!hasPermission) {
      el.setAttribute("disabled", "true");
      el.style.opacity = "0.5";
      el.style.cursor = "not-allowed";
    }
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const hasPermission = checkPermission(binding.value);
    if (!hasPermission) {
      el.setAttribute("disabled", "true");
      el.style.opacity = "0.5";
      el.style.cursor = "not-allowed";
    } else {
      el.removeAttribute("disabled");
      el.style.opacity = "";
      el.style.cursor = "";
    }
  }
};

export default {
  permission,
  permissionDisabled
};

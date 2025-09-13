<script setup lang="ts">
import { computed } from "vue";
import { isUrl } from "@pureadmin/utils";
import { menuType } from "@/layout/types";

const props = defineProps<{
  to: menuType;
}>();

const isExternalLink = computed(() => isUrl(props.to.name));
const getLinkProps = (item: menuType) => {
  if (isExternalLink.value) {
    return {
      href: item.name,
      target: "_blank",
      rel: "noopener"
    };
  }
  // 只传递路由需要的属性，避免传递额外的属性导致路由解析失败
  const routeLocation: any = {};

  // 优先使用path，如果没有path则使用name
  if (item.path) {
    routeLocation.path = item.path;
  } else if (item.name) {
    routeLocation.name = item.name;
  }

  // 如果有query参数，也传递过去
  if (item.query) {
    routeLocation.query = item.query;
  }

  // 如果有params参数，也传递过去
  if (item.params) {
    routeLocation.params = item.params;
  }

  return {
    to: routeLocation
  };
};
</script>

<template>
  <component
    :is="isExternalLink ? 'a' : 'router-link'"
    v-bind="getLinkProps(to)"
  >
    <slot />
  </component>
</template>

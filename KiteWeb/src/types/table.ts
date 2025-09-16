// 表格路由配置类型
export interface RouteConfigsTable {
  id?: number;
  parentId?: number;
  title?: string;
  name?: string;
  path: string;
  component?: string | (() => Promise<any>);
  redirect?: string;
  meta?: {
    title?: string;
    icon?: string;
    showLink?: boolean;
    savedPosition?: boolean;
    auths?: string[];
    roles?: string[];
    rank?: number;
  };
  children?: RouteConfigsTable[];
}

// 表格数据类型
export interface TableDataInfo<T = any> {
  /** 表格数据 */
  data: Array<T>;
  /** 当前页码 */
  currentPage: number;
  /** 每页显示条数 */
  pageSize: number;
  /** 总条数 */
  total: number;
  /** 总页数 */
  totalPages?: number;
}

// 表格列配置
export interface TableColumnList {
  label: string;
  prop?: string;
  width?: string | number;
  minWidth?: string | number;
  slot?: string;
  headerSlot?: string;
  hide?: boolean;
  children?: TableColumnList[];
}

// 表格分页配置
export interface PaginationProps {
  total?: number;
  pageSize?: number;
  defaultPageSize?: number;
  currentPage?: number;
  pageSizes?: number[];
  layout?: string;
  background?: boolean;
  small?: boolean;
} 
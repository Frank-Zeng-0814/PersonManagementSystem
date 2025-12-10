================================
① 必须支持的 API（完整列表）
================================
Employee APIs

已存在，不需要创建，只需要调用：

GET /api/employees

GET /api/employees/{id}

POST /api/employees

PUT /api/employees/{id}

DELETE /api/employees/{id}

状态操作（可选）：

POST /api/employees/{id}/set-active

POST /api/employees/{id}/set-on-leave

Department APIs

GET /api/departments

POST /api/departments

PUT /api/departments/{id}

DELETE /api/departments/{id}

（提示：前端不一定需要 UI 支持所有操作，可按需调用）

Position APIs

GET /api/positions

POST /api/positions

PUT /api/positions/{id}

DELETE /api/positions/{id}

EmploymentContract APIs

GET /api/employees/{employeeId}/contracts

POST /api/employees/{employeeId}/contracts

PUT /api/contracts/{id}（可选）

DELETE /api/contracts/{id}（可选）

创建合同时后端会执行：

Active/Ended 状态

日期验证

重叠合同检查

LeaveRequest APIs

GET /api/employees/{employeeId}/leave-requests

POST /api/employees/{employeeId}/leave-requests → 创建 Draft

状态机流转操作：

POST /api/leave-requests/{id}/submit

POST /api/leave-requests/{id}/approve

POST /api/leave-requests/{id}/reject

POST /api/leave-requests/{id}/cancel

（Completed 状态由后台任务自动设置，不需要前端操作）

==========================================
② Person 页面必须支持的业务功能（前端要做什么）
==========================================

Person 页面是 单页管理后台，需要支持以下逻辑：

1️⃣ 员工列表区（Employee List）

必须实现：

显示所有员工

点击某个员工 → 加载他的详细数据

创建新员工（Modal 或简单表单即可）

编辑员工信息

删除员工

展示 employee.status（Active / OnLeave / Inactive）

2️⃣ 员工详情区（Employee Detail）

选中员工后，右侧显示员工详情。布局 Claude 自己决定，但需要支持以下 Tab：

A. Profile Tab

显示：

FullName

Email

Phone

Status

Department / Position

允许：

编辑员工信息

修改 Department / Position（如果你需要这些字段）

B. Contracts Tab

必须实现：

GET /api/employees/{employeeId}/contracts

列表展示：

StartDate

EndDate

EmploymentType

Salary

Status (Active / Ended)

按钮：“Add Contract”

表单字段：

StartDate

EndDate

EmploymentType

BaseSalary

提交至：
POST /api/employees/{employeeId}/contracts

错误展示（必须支持）：

合同重叠（后端会返回 400/422）

非法日期（后端会返回错误）

C. Leave Requests Tab

必须实现：

GET /api/employees/{employeeId}/leave-requests

列表展示：

Type

StartDate / EndDate

Reason

Status

ApproverName

操作按钮逻辑（根据状态）：

Draft：显示 Submit 按钮
→ 调用：POST /api/leave-requests/{id}/submit

Submitted：显示

Approve → POST /api/leave-requests/{id}/approve

Reject → POST /api/leave-requests/{id}/reject

Cancel → POST /api/leave-requests/{id}/cancel

Approved / Rejected / Completed：只读，无操作

创建 Draft：

按钮：“Create Leave Draft”

表单字段：

Type

StartDate

EndDate

Reason

提交至：
POST /api/employees/{employeeId}/leave-requests

错误展示（必须支持）：

重叠假期 → 后端会返回错误

不在合同期间 → 后端会返回错误

======================================
③ SignalR 前端需要实现的功能
======================================

必须连接以下 Hub：

/hubs/notifications


SignalR 客户端地址：

本地：

ws://localhost:3000/hubs/notifications


部署环境例子：

wss://your-domain/hubs/notifications


必须监听以下事件：

EmployeeUpdated

LeaveRequestUpdated

ContractUpdated

ContractExpiringSoon

UpcomingLeave

收到事件时：

将事件 append 到一个前端状态数组 notifications[]

在界面右下角展示一个“实时通知区域”

展示格式 Claude 可以自己决定（简单 div 列表即可）

======================================
④ 前端行为总结（便于 Claude 理解）
======================================

你需要为 Person 页面实现一个 员工管理后台控制台，包括：

员工列表

员工详情（3 个 Tab：Profile / Contracts / Leave Requests）

所有后端 API 对应的操作按钮

所有业务错误（如合同重叠、非法状态流转）的提示信息

一个实时通知面板（基于 SignalR）

布局和 UI 风格 Claude 自由发挥，功能优先。

======================================
⑤ 特别注意事项（避免 Claude 误解）
======================================

不要创建登录系统

不要区分角色（只有 HR 管理员使用）

所有 API 都是无鉴权的公开调用

SignalR 事件全部使用 Clients.All，不需要房间或分组

我不需要特别复杂的 UI，功能优先

Home / About 页面不需要修改

Person 页面是唯一需要增强的页面

✅ 你现在可以根据这些需求，按以下顺序为我生成代码：

创建 SignalR 前端连接模块（notifications listener）

扩展 Person 页面状态结构（employee list + selected employee + tabs + notifications）

为 Employee CRUD 生成前端代码

为 Contracts tab 生成代码（list + add form）

为 Leave Requests tab 生成代码（list + create draft + status buttons）

整合所有 UI + 测试用例 + 错误展示
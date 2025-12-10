// Employee Status
export const EmployeeStatus = {
  ACTIVE: 0,
  ON_LEAVE: 1,
  INACTIVE: 2,
};

export const EmployeeStatusLabels = {
  [EmployeeStatus.ACTIVE]: 'Active',
  [EmployeeStatus.ON_LEAVE]: 'On Leave',
  [EmployeeStatus.INACTIVE]: 'Inactive',
};

export const EmployeeStatusColors = {
  [EmployeeStatus.ACTIVE]: 'bg-green-100 text-green-800 border-green-200',
  [EmployeeStatus.ON_LEAVE]: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  [EmployeeStatus.INACTIVE]: 'bg-gray-100 text-gray-800 border-gray-200',
};

// Employment Type
export const EmploymentType = {
  FULL_TIME: 0,
  PART_TIME: 1,
  CONTRACT: 2,
  INTERNSHIP: 3,
};

export const EmploymentTypeLabels = {
  [EmploymentType.FULL_TIME]: 'Full Time',
  [EmploymentType.PART_TIME]: 'Part Time',
  [EmploymentType.CONTRACT]: 'Contract',
  [EmploymentType.INTERNSHIP]: 'Internship',
};

// Contract Status
export const ContractStatus = {
  ACTIVE: 0,
  ENDED: 1,
};

export const ContractStatusLabels = {
  [ContractStatus.ACTIVE]: 'Active',
  [ContractStatus.ENDED]: 'Ended',
};

export const ContractStatusColors = {
  [ContractStatus.ACTIVE]: 'bg-green-100 text-green-800 border-green-200',
  [ContractStatus.ENDED]: 'bg-gray-100 text-gray-800 border-gray-200',
};

// Leave Type
export const LeaveType = {
  ANNUAL: 0,
  SICK: 1,
  PERSONAL: 2,
  MATERNITY: 3,
  PATERNITY: 4,
  UNPAID: 5,
};

export const LeaveTypeLabels = {
  [LeaveType.ANNUAL]: 'Annual',
  [LeaveType.SICK]: 'Sick',
  [LeaveType.PERSONAL]: 'Personal',
  [LeaveType.MATERNITY]: 'Maternity',
  [LeaveType.PATERNITY]: 'Paternity',
  [LeaveType.UNPAID]: 'Unpaid',
};

// Leave Request Status
export const LeaveRequestStatus = {
  DRAFT: 0,
  SUBMITTED: 1,
  APPROVED: 2,
  REJECTED: 3,
  CANCELLED: 4,
  COMPLETED: 5,
};

export const LeaveRequestStatusLabels = {
  [LeaveRequestStatus.DRAFT]: 'Draft',
  [LeaveRequestStatus.SUBMITTED]: 'Submitted',
  [LeaveRequestStatus.APPROVED]: 'Approved',
  [LeaveRequestStatus.REJECTED]: 'Rejected',
  [LeaveRequestStatus.CANCELLED]: 'Cancelled',
  [LeaveRequestStatus.COMPLETED]: 'Completed',
};

export const LeaveRequestStatusColors = {
  [LeaveRequestStatus.DRAFT]: 'bg-gray-100 text-gray-800 border-gray-200',
  [LeaveRequestStatus.SUBMITTED]: 'bg-blue-100 text-blue-800 border-blue-200',
  [LeaveRequestStatus.APPROVED]: 'bg-green-100 text-green-800 border-green-200',
  [LeaveRequestStatus.REJECTED]: 'bg-red-100 text-red-800 border-red-200',
  [LeaveRequestStatus.CANCELLED]: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  [LeaveRequestStatus.COMPLETED]: 'bg-purple-100 text-purple-800 border-purple-200',
};

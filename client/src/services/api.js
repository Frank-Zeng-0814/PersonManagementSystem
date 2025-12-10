import axios from 'axios';

const BASE_URL = import.meta.env.VITE_BASE_API_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Employee APIs
export const employeeApi = {
  getAll: () => api.get('/api/employees'),
  getById: (id) => api.get(`/api/employees/${id}`),
  create: (data) => api.post('/api/employees', data),
  update: (id, data) => api.put(`/api/employees/${id}`, data),
  delete: (id) => api.delete(`/api/employees/${id}`),
  setActive: (id) => api.post(`/api/employees/${id}/set-active`),
  setOnLeave: (id) => api.post(`/api/employees/${id}/set-on-leave`),
};

// Department APIs
export const departmentApi = {
  getAll: () => api.get('/api/departments'),
  create: (data) => api.post('/api/departments', data),
  update: (id, data) => api.put(`/api/departments/${id}`, data),
  delete: (id) => api.delete(`/api/departments/${id}`),
};

// Position APIs
export const positionApi = {
  getAll: () => api.get('/api/positions'),
  create: (data) => api.post('/api/positions', data),
  update: (id, data) => api.put(`/api/positions/${id}`, data),
  delete: (id) => api.delete(`/api/positions/${id}`),
};

// Employment Contract APIs
export const contractApi = {
  getByEmployee: (employeeId) => api.get(`/api/employees/${employeeId}/contracts`),
  create: (employeeId, data) => api.post(`/api/employees/${employeeId}/contracts`, data),
  update: (id, data) => api.put(`/api/contracts/${id}`, data),
  delete: (id) => api.delete(`/api/contracts/${id}`),
};

// Leave Request APIs
export const leaveRequestApi = {
  getByEmployee: (employeeId) => api.get(`/api/employees/${employeeId}/leave-requests`),
  create: (employeeId, data) => api.post(`/api/employees/${employeeId}/leave-requests`, data),
  submit: (id) => api.post(`/api/leave-requests/${id}/submit`),
  approve: (id) => api.post(`/api/leave-requests/${id}/approve`),
  reject: (id) => api.post(`/api/leave-requests/${id}/reject`),
  cancel: (id) => api.post(`/api/leave-requests/${id}/cancel`),
};

export default api;

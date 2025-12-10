import { useState, useEffect } from 'react';
import { employeeApi, departmentApi, positionApi } from '../../services/api';
import EmployeeList from '../employee/EmployeeList';
import EmployeeDetail from '../employee/EmployeeDetail';
import EmployeeFormModal from '../employee/EmployeeFormModal';
import NotificationPanel from '../NotificationPanel';
import useNotifications from '../../hooks/useNotifications';
import toast from 'react-hot-toast';

const NewPerson = () => {
  const [employees, setEmployees] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [positions, setPositions] = useState([]);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingEmployee, setEditingEmployee] = useState(null);

  const { notifications, isConnected, clearNotifications, removeNotification } = useNotifications();

  useEffect(() => {
    loadAllData();
  }, []);

  const loadAllData = async () => {
    setLoading(true);
    try {
      const [employeesRes, departmentsRes, positionsRes] = await Promise.all([
        employeeApi.getAll(),
        departmentApi.getAll(),
        positionApi.getAll(),
      ]);
      setEmployees(employeesRes.data || []);
      setDepartments(departmentsRes.data || []);
      setPositions(positionsRes.data || []);
    } catch (error) {
      console.error('Load data error:', error);
      toast.error('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const loadEmployees = async () => {
    try {
      const response = await employeeApi.getAll();
      setEmployees(response.data || []);
      if (selectedEmployee) {
        const updated = response.data.find(emp => emp.id === selectedEmployee.id);
        setSelectedEmployee(updated || null);
      }
    } catch (error) {
      console.error('Load employees error:', error);
      toast.error('Failed to load employees');
    }
  };

  const handleCreateNew = () => {
    setEditingEmployee(null);
    setShowModal(true);
  };

  const handleEdit = (employee) => {
    setEditingEmployee(employee);
    setShowModal(true);
  };

  const handleDelete = async (employee) => {
    if (!confirm(`Are you sure you want to delete ${employee.fullName}?`)) {
      return;
    }

    setLoading(true);
    try {
      await employeeApi.delete(employee.id);
      toast.success('Employee deleted successfully');
      setEmployees(prev => prev.filter(emp => emp.id !== employee.id));
      if (selectedEmployee?.id === employee.id) {
        setSelectedEmployee(null);
      }
    } catch (error) {
      console.error('Delete error:', error);
      toast.error(error.response?.data?.message || 'Failed to delete employee');
    } finally {
      setLoading(false);
    }
  };

  const handleFormSubmit = async (formData) => {
    setLoading(true);
    try {
      if (editingEmployee) {
        await employeeApi.update(editingEmployee.id, formData);
        toast.success('Employee updated successfully');
      } else {
        const response = await employeeApi.create(formData);
        toast.success('Employee created successfully');
        setEmployees(prev => [...prev, response.data]);
      }
      setShowModal(false);
      setEditingEmployee(null);
      await loadEmployees();
    } catch (error) {
      console.error('Form submit error:', error);
      toast.error(error.response?.data?.message || 'Failed to save employee');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectEmployee = (employee) => {
    setSelectedEmployee(employee);
  };

  const handleEmployeeUpdate = async () => {
    await loadEmployees();
  };

  const handleAvatarUpdate = (employeeId, avatarUrl) => {
    setEmployees(prevEmployees =>
      prevEmployees.map(emp => emp.id === employeeId ? { ...emp, avatarUrl } : emp)
    );
    if (selectedEmployee?.id === employeeId) {
      setSelectedEmployee(prev => ({ ...prev, avatarUrl }));
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
            Employee Management System
          </h1>
          <p className="text-gray-600 mt-2">Manage employees, contracts, and leave requests</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6" style={{ height: 'calc(100vh - 200px)' }}>
          <div className="lg:col-span-1">
            <EmployeeList
              employees={employees}
              selectedEmployee={selectedEmployee}
              onSelect={handleSelectEmployee}
              onEdit={handleEdit}
              onDelete={handleDelete}
              onCreateNew={handleCreateNew}
              loading={loading}
              onAvatarUpdate={handleAvatarUpdate}
            />
          </div>

          <div className="lg:col-span-2">
            <EmployeeDetail
              employee={selectedEmployee}
              departments={departments}
              positions={positions}
              onEmployeeUpdate={handleEmployeeUpdate}
            />
          </div>
        </div>

        <EmployeeFormModal
          isOpen={showModal}
          onClose={() => {
            setShowModal(false);
            setEditingEmployee(null);
          }}
          employee={editingEmployee}
          departments={departments}
          positions={positions}
          onSubmit={handleFormSubmit}
          loading={loading}
        />

        <NotificationPanel
          notifications={notifications}
          isConnected={isConnected}
          onClear={clearNotifications}
          onRemove={removeNotification}
        />
      </div>
    </div>
  );
};

export default NewPerson;

import { useState } from 'react';
import { Edit, Save, X, Mail, Phone, Briefcase } from 'lucide-react';
import { employeeApi } from '../../services/api';
import { EmployeeStatusLabels, EmployeeStatusColors, EmployeeStatus } from '../../constants/enums';
import toast from 'react-hot-toast';

const EmployeeProfile = ({ employee, departments, positions, onUpdate }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    fullName: employee.fullName,
    email: employee.email,
    phone: employee.phone || '',
    departmentId: employee.departmentId || '',
    positionId: employee.positionId || '',
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await employeeApi.update(employee.id, {
        ...formData,
        departmentId: formData.departmentId ? parseInt(formData.departmentId) : null,
        positionId: formData.positionId ? parseInt(formData.positionId) : null,
      });
      toast.success('Employee updated successfully');
      setIsEditing(false);
      onUpdate();
    } catch (error) {
      console.error('Update error:', error);
      toast.error(error.response?.data?.message || 'Failed to update employee');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    setFormData({
      fullName: employee.fullName,
      email: employee.email,
      phone: employee.phone || '',
      departmentId: employee.departmentId || '',
      positionId: employee.positionId || '',
    });
    setIsEditing(false);
  };

  const handleSetStatus = async (status) => {
    setLoading(true);
    try {
      if (status === EmployeeStatus.ACTIVE) {
        await employeeApi.setActive(employee.id);
      } else if (status === EmployeeStatus.ON_LEAVE) {
        await employeeApi.setOnLeave(employee.id);
      }
      toast.success('Status updated successfully');
      onUpdate();
    } catch (error) {
      console.error('Status update error:', error);
      toast.error(error.response?.data?.message || 'Failed to update status');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-lg font-semibold text-gray-900">Profile Information</h3>
        {!isEditing && (
          <button
            onClick={() => setIsEditing(true)}
            className="flex items-center gap-1 px-3 py-1.5 text-sm text-blue-600 hover:bg-blue-50 rounded-md transition-colors"
          >
            <Edit className="w-4 h-4" />
            Edit
          </button>
        )}
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Full Name */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Full Name
          </label>
          {isEditing ? (
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          ) : (
            <p className="text-gray-900">{employee.fullName}</p>
          )}
        </div>

        {/* Email */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            <Mail className="w-4 h-4 inline mr-1" />
            Email
          </label>
          {isEditing ? (
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          ) : (
            <p className="text-gray-900">{employee.email}</p>
          )}
        </div>

        {/* Phone */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            <Phone className="w-4 h-4 inline mr-1" />
            Phone
          </label>
          {isEditing ? (
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          ) : (
            <p className="text-gray-900">{employee.phone || 'N/A'}</p>
          )}
        </div>

        {/* Department */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            <Briefcase className="w-4 h-4 inline mr-1" />
            Department
          </label>
          {isEditing ? (
            <select
              name="departmentId"
              value={formData.departmentId}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">No Department</option>
              {departments.map(dept => (
                <option key={dept.id} value={dept.id}>{dept.name}</option>
              ))}
            </select>
          ) : (
            <p className="text-gray-900">{employee.departmentName || 'N/A'}</p>
          )}
        </div>

        {/* Position */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Position
          </label>
          {isEditing ? (
            <select
              name="positionId"
              value={formData.positionId}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">No Position</option>
              {positions.map(pos => (
                <option key={pos.id} value={pos.id}>{pos.title}</option>
              ))}
            </select>
          ) : (
            <p className="text-gray-900">{employee.positionName || 'N/A'}</p>
          )}
        </div>

        {/* Status */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Status
          </label>
          <div className="flex items-center gap-3">
            <span className={`inline-block px-3 py-1 rounded-full border text-sm ${
              EmployeeStatusColors[employee.status]
            }`}>
              {EmployeeStatusLabels[employee.status]}
            </span>
            {!isEditing && (
              <div className="flex gap-2">
                {employee.status !== EmployeeStatus.ACTIVE && (
                  <button
                    type="button"
                    onClick={() => handleSetStatus(EmployeeStatus.ACTIVE)}
                    disabled={loading}
                    className="text-xs px-2 py-1 text-green-600 hover:bg-green-50 rounded border border-green-300"
                  >
                    Set Active
                  </button>
                )}
                {employee.status !== EmployeeStatus.ON_LEAVE && (
                  <button
                    type="button"
                    onClick={() => handleSetStatus(EmployeeStatus.ON_LEAVE)}
                    disabled={loading}
                    className="text-xs px-2 py-1 text-yellow-600 hover:bg-yellow-50 rounded border border-yellow-300"
                  >
                    Set On Leave
                  </button>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Action Buttons */}
        {isEditing && (
          <div className="flex gap-3 pt-4">
            <button
              type="submit"
              disabled={loading}
              className="flex items-center gap-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Save className="w-4 h-4" />
              {loading ? 'Saving...' : 'Save Changes'}
            </button>
            <button
              type="button"
              onClick={handleCancel}
              disabled={loading}
              className="flex items-center gap-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              <X className="w-4 h-4" />
              Cancel
            </button>
          </div>
        )}
      </form>
    </div>
  );
};

export default EmployeeProfile;

import { Users, Plus, Edit, Trash2, Upload, User } from 'lucide-react';
import { useState } from 'react';
import { EmployeeStatusLabels, EmployeeStatusColors } from '../../constants/enums';
import axios from 'axios';
import toast from 'react-hot-toast';

const EmployeeList = ({ employees, selectedEmployee, onSelect, onEdit, onDelete, onCreateNew, loading, onAvatarUpdate }) => {
  const BASE_URL = import.meta.env.VITE_BASE_API_URL || 'http://localhost:3000';
  const [uploadingId, setUploadingId] = useState(null);

  const handleAvatarUpload = async (employeeId, file) => {
    if (!file) return;

    const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    if (!validTypes.includes(file.type)) {
      toast.error('Please upload a valid image file (JPEG, PNG, GIF, or WebP)');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      toast.error('File size must be less than 5MB');
      return;
    }

    setUploadingId(employeeId);
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await axios.post(`${BASE_URL}/api/employees/${employeeId}/upload-avatar`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });

      toast.success('Avatar uploaded successfully!');
      onAvatarUpdate(employeeId, response.data.avatarUrl);
    } catch (error) {
      console.error(error);
      toast.error('Failed to upload avatar');
    } finally {
      setUploadingId(null);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md border border-gray-200 h-full flex flex-col">
      {/* Header */}
      <div className="p-4 border-b border-gray-200 flex items-center justify-between bg-gray-50">
        <div className="flex items-center gap-2">
          <Users className="w-5 h-5 text-gray-700" />
          <h2 className="text-lg font-semibold text-gray-900">Employees</h2>
          {employees.length > 0 && (
            <span className="bg-blue-100 text-blue-800 text-xs rounded-full px-2 py-0.5">
              {employees.length}
            </span>
          )}
        </div>
        <button
          onClick={onCreateNew}
          className="flex items-center gap-1 px-3 py-1.5 bg-blue-600 text-white text-sm rounded-md hover:bg-blue-700 transition-colors"
        >
          <Plus className="w-4 h-4" />
          New
        </button>
      </div>

      {/* List */}
      <div className="flex-1 overflow-y-auto">
        {loading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          </div>
        ) : employees.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            <Users className="w-12 h-12 mx-auto mb-3 text-gray-300" />
            <p className="text-sm">No employees yet</p>
            <button
              onClick={onCreateNew}
              className="mt-3 text-sm text-blue-600 hover:underline"
            >
              Create your first employee
            </button>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {employees.map((employee) => (
              <div
                key={employee.id}
                onClick={() => onSelect(employee)}
                className={`p-4 cursor-pointer hover:bg-gray-50 transition-colors ${
                  selectedEmployee?.id === employee.id ? 'bg-blue-50 border-l-4 border-blue-600' : ''
                }`}
              >
                <div className="flex items-start gap-3">
                  {/* Avatar */}
                  <div className="flex-shrink-0" onClick={(e) => e.stopPropagation()}>
                    <div className="relative group">
                      {employee.avatarUrl ? (
                        <img
                          src={employee.avatarUrl}
                          alt={employee.fullName}
                          className="w-12 h-12 rounded-full object-cover border-2 border-gray-200"
                        />
                      ) : (
                        <div className="w-12 h-12 rounded-full bg-gradient-to-r from-blue-400 to-purple-400 flex items-center justify-center">
                          <User className="w-6 h-6 text-white" />
                        </div>
                      )}
                      <label className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 rounded-full opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer">
                        <input
                          type="file"
                          accept="image/*"
                          className="hidden"
                          onChange={(e) => handleAvatarUpload(employee.id, e.target.files[0])}
                          disabled={uploadingId === employee.id}
                        />
                        <Upload className="w-5 h-5 text-white" />
                      </label>
                    </div>
                    {uploadingId === employee.id && (
                      <div className="text-xs text-center text-gray-600 mt-1">Uploading...</div>
                    )}
                  </div>

                  {/* Info */}
                  <div className="flex-1 min-w-0">
                    <h3 className="font-medium text-gray-900 truncate">
                      {employee.fullName}
                    </h3>
                    <p className="text-sm text-gray-600 truncate mt-0.5">
                      {employee.email}
                    </p>
                    {employee.departmentName && (
                      <p className="text-xs text-gray-500 mt-1">
                        {employee.departmentName} • {employee.positionTitle || 'No Position'}
                      </p>
                    )}
                    <div className="mt-2">
                      <span className={`inline-block text-xs px-2 py-0.5 rounded-full border ${
                        EmployeeStatusColors[employee.status]
                      }`}>
                        {EmployeeStatusLabels[employee.status]}
                      </span>
                    </div>
                  </div>

                  {/* Actions */}
                  <div className="flex gap-1" onClick={(e) => e.stopPropagation()}>
                    <button
                      onClick={() => onEdit(employee)}
                      className="p-1.5 hover:bg-gray-200 rounded text-gray-600 hover:text-blue-600"
                      title="Edit"
                    >
                      <Edit className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => onDelete(employee)}
                      className="p-1.5 hover:bg-gray-200 rounded text-gray-600 hover:text-red-600"
                      title="Delete"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default EmployeeList;

import { useState, useEffect } from 'react';
import { Plus, Calendar, CheckCircle, XCircle, Send, Ban } from 'lucide-react';
import { leaveRequestApi } from '../../services/api';
import {
  LeaveRequestStatusLabels,
  LeaveRequestStatusColors,
  LeaveRequestStatus,
  LeaveTypeLabels,
  LeaveType,
} from '../../constants/enums';
import toast from 'react-hot-toast';

const EmployeeLeaveRequests = ({ employeeId, employeeName }) => {
  const [leaveRequests, setLeaveRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showAddForm, setShowAddForm] = useState(false);
  const [formData, setFormData] = useState({
    type: LeaveType.ANNUAL,
    startDate: '',
    endDate: '',
    reason: '',
  });

  useEffect(() => {
    loadLeaveRequests();
  }, [employeeId]);

  const loadLeaveRequests = async () => {
    setLoading(true);
    try {
      const response = await leaveRequestApi.getByEmployee(employeeId);
      setLeaveRequests(response.data || []);
    } catch (error) {
      console.error('Load leave requests error:', error);
      toast.error('Failed to load leave requests');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await leaveRequestApi.create(employeeId, {
        ...formData,
        type: parseInt(formData.type),
      });
      toast.success('Leave request draft created successfully');
      setShowAddForm(false);
      setFormData({
        type: LeaveType.ANNUAL,
        startDate: '',
        endDate: '',
        reason: '',
      });
      loadLeaveRequests();
    } catch (error) {
      console.error('Create leave request error:', error);
      const message = error.response?.data?.message || error.response?.data?.title || 'Failed to create leave request';
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  const handleStatusAction = async (leaveRequestId, action) => {
    setLoading(true);
    try {
      switch (action) {
        case 'submit':
          await leaveRequestApi.submit(leaveRequestId);
          toast.success('Leave request submitted');
          break;
        case 'approve':
          await leaveRequestApi.approve(leaveRequestId);
          toast.success('Leave request approved');
          break;
        case 'reject':
          await leaveRequestApi.reject(leaveRequestId);
          toast.success('Leave request rejected');
          break;
        case 'cancel':
          await leaveRequestApi.cancel(leaveRequestId);
          toast.success('Leave request cancelled');
          break;
      }
      loadLeaveRequests();
    } catch (error) {
      console.error('Status action error:', error);
      const message = error.response?.data?.message || error.response?.data?.title || `Failed to ${action} leave request`;
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  };

  const getActionButtons = (leaveRequest) => {
    const { id, status } = leaveRequest;

    switch (status) {
      case LeaveRequestStatus.DRAFT:
        return (
          <button
            onClick={() => handleStatusAction(id, 'submit')}
            disabled={loading}
            className="flex items-center gap-1 px-3 py-1.5 text-sm bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
          >
            <Send className="w-3.5 h-3.5" />
            Submit
          </button>
        );

      case LeaveRequestStatus.SUBMITTED:
        return (
          <div className="flex gap-2">
            <button
              onClick={() => handleStatusAction(id, 'approve')}
              disabled={loading}
              className="flex items-center gap-1 px-3 py-1.5 text-sm bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
            >
              <CheckCircle className="w-3.5 h-3.5" />
              Approve
            </button>
            <button
              onClick={() => handleStatusAction(id, 'reject')}
              disabled={loading}
              className="flex items-center gap-1 px-3 py-1.5 text-sm bg-red-600 text-white rounded-md hover:bg-red-700 disabled:opacity-50"
            >
              <XCircle className="w-3.5 h-3.5" />
              Reject
            </button>
            <button
              onClick={() => handleStatusAction(id, 'cancel')}
              disabled={loading}
              className="flex items-center gap-1 px-3 py-1.5 text-sm bg-yellow-600 text-white rounded-md hover:bg-yellow-700 disabled:opacity-50"
            >
              <Ban className="w-3.5 h-3.5" />
              Cancel
            </button>
          </div>
        );

      case LeaveRequestStatus.APPROVED:
      case LeaveRequestStatus.REJECTED:
      case LeaveRequestStatus.CANCELLED:
      case LeaveRequestStatus.COMPLETED:
        return (
          <span className="text-sm text-gray-500">No actions available</span>
        );

      default:
        return null;
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-lg font-semibold text-gray-900">Leave Requests</h3>
        <button
          onClick={() => setShowAddForm(!showAddForm)}
          className="flex items-center gap-1 px-3 py-1.5 bg-blue-600 text-white text-sm rounded-md hover:bg-blue-700 transition-colors"
        >
          <Plus className="w-4 h-4" />
          Create Leave Draft
        </button>
      </div>

      {/* Add Leave Request Form */}
      {showAddForm && (
        <div className="mb-6 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <h4 className="font-medium text-gray-900 mb-4">New Leave Request (Draft)</h4>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Leave Type *
                </label>
                <select
                  name="type"
                  value={formData.type}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {Object.entries(LeaveTypeLabels).map(([value, label]) => (
                    <option key={value} value={value}>{label}</option>
                  ))}
                </select>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Start Date *
                </label>
                <input
                  type="date"
                  name="startDate"
                  value={formData.startDate}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  End Date *
                </label>
                <input
                  type="date"
                  name="endDate"
                  value={formData.endDate}
                  onChange={handleChange}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Reason *
              </label>
              <textarea
                name="reason"
                value={formData.reason}
                onChange={handleChange}
                required
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Please provide a reason for your leave request"
              />
            </div>

            <div className="flex gap-3 pt-2">
              <button
                type="submit"
                disabled={loading}
                className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? 'Creating...' : 'Create Draft'}
              </button>
              <button
                type="button"
                onClick={() => setShowAddForm(false)}
                className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Leave Requests List */}
      {loading && !showAddForm ? (
        <div className="flex justify-center items-center py-12">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      ) : leaveRequests.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          <Calendar className="w-12 h-12 mx-auto mb-3 text-gray-300" />
          <p>No leave requests yet</p>
          <button
            onClick={() => setShowAddForm(true)}
            className="mt-3 text-sm text-blue-600 hover:underline"
          >
            Create first leave request
          </button>
        </div>
      ) : (
        <div className="space-y-3">
          {leaveRequests.map((leaveRequest) => (
            <div
              key={leaveRequest.id}
              className="p-4 border border-gray-200 rounded-lg hover:shadow-md transition-shadow"
            >
              <div className="flex items-start justify-between mb-3">
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-2">
                    <Calendar className="w-4 h-4 text-gray-600" />
                    <span className="font-medium text-gray-900">
                      {LeaveTypeLabels[leaveRequest.type]}
                    </span>
                    <span className={`text-xs px-2 py-0.5 rounded-full border ${
                      LeaveRequestStatusColors[leaveRequest.status]
                    }`}>
                      {LeaveRequestStatusLabels[leaveRequest.status]}
                    </span>
                  </div>
                  <div className="text-sm text-gray-600 space-y-1">
                    <div className="flex items-center gap-1">
                      <span className="font-medium">Period:</span>
                      <span>{formatDate(leaveRequest.startDate)} - {formatDate(leaveRequest.endDate)}</span>
                    </div>
                    {leaveRequest.reason && (
                      <div>
                        <span className="font-medium">Reason:</span>
                        <span className="ml-1">{leaveRequest.reason}</span>
                      </div>
                    )}
                    {leaveRequest.approverName && (
                      <div>
                        <span className="font-medium">Approver:</span>
                        <span className="ml-1">{leaveRequest.approverName}</span>
                      </div>
                    )}
                  </div>
                </div>
              </div>
              <div className="pt-3 border-t border-gray-200">
                {getActionButtons(leaveRequest)}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default EmployeeLeaveRequests;
